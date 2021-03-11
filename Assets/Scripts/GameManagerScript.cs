using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField] private int _maxNumOfBalls;
    [SerializeField] private int _numOfBallsToSplit;
    private int _numOfActiveBalls = 0;
    private int _numOfDeactivatedBalls = 0;
    public GameObject ballPrefab;
    public GameObject upperBound;
    public GameObject lowerBound;
    public GameObject rightBound;
    public GameObject leftBound;
    private List<BallScript> _listOfActiveBalls;
    private List<BallScript> _listOfDeactivatedBalls;  
    private Camera cam;
    private float _gravityConstant = .05f;

    private void PrintInfo()
    {
        Debug.Log("Number of active balls: " + _numOfActiveBalls.ToString());
    
    }

    private void BoundSetup()
    {   
        // Set up bounding scene depending on your screen resolution / ratio.
        // Bounds are build out of 4 initial cubes placed in the center of the scene
        // Assumption: all initial bounds are cubes. Thus, they have the same scales

        Renderer rend = upperBound.GetComponent<Renderer>();
        Vector3 boundSize = rend.bounds.size;
        Vector3 boundScale;

        if (cam)
        {
            float xScale = cam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
            float yScale = cam.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;

            float worldUp = cam.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
            float worldDown = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
            float worldLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
            float worldRight = cam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;

            boundScale = upperBound.transform.localScale;
            boundScale.x = xScale;
            upperBound.transform.localScale = boundScale;
            upperBound.transform.position = new Vector3(0, worldUp + boundSize.y / 2, 0);

            boundScale = lowerBound.transform.localScale;
            boundScale.x = xScale;
            lowerBound.transform.localScale = boundScale;
            lowerBound.transform.position = new Vector3(0, worldDown - boundSize.y / 2, 0);

            boundScale = leftBound.transform.localScale;
            boundScale.y = yScale;
            leftBound.transform.localScale = boundScale;
            leftBound.transform.position = new Vector3(worldLeft - boundSize.x / 2, 0 , 0);

            boundScale = rightBound.transform.localScale;
            boundScale.y = yScale;
            rightBound.transform.localScale = boundScale;
            rightBound.transform.position = new Vector3(worldRight + boundSize.x / 2, 0 , 0);


        }
    }

    private Vector3 PartialGravity(Rigidbody ballRb, Rigidbody attractorRb)
    {
        // Adding extra term to magnitute for safe keeping. With collisions turned off the distance
        // between two objects can get to close (not possible for real physical objects - no material points).
        // Without this the adding force becomes extremely strong and we have jumps over colliders
        // because of discrete collision detection
        
        Vector3 distVec = attractorRb.position - ballRb.position;
        float magnitude = distVec.sqrMagnitude + 0.001f; 
        return (attractorRb.mass / (magnitude * Mathf.Sqrt(magnitude))) * distVec;
        
        
    }


    private Vector3 TotalGravity(Rigidbody ballRb)
    {
        Vector3 totalForce = Vector3.zero;
        foreach ( BallScript otherBallRb in  _listOfActiveBalls)
        {

            totalForce += PartialGravity(ballRb, otherBallRb.BallRigidBody);
            
        }
        totalForce = _gravityConstant * ballRb.mass * totalForce;
        return totalForce;
    }

    private void SystemGravity()
    {
        foreach (BallScript ballRb in _listOfActiveBalls)
        {
            Rigidbody rb = ballRb.BallRigidBody;
            rb.AddForce(TotalGravity(rb));
            //ballRb.BallRigidBody.AddForce(TotalGravity(ballRb.BallRigidBody));
            //ballRb.Interact(TotalGravity(ballRb.BallRigidBody));
        }
    }

    void EnableCollision(bool type)
    {
        if (type)
        {
            foreach (BallScript item in _listOfDeactivatedBalls)
            {
                item.BallCollider.isTrigger = false;
            }
        }
        else 
        {
            foreach (BallScript item in _listOfDeactivatedBalls)
            {
                item.BallCollider.isTrigger = true;
            }            
        }
    }

    private void GenerateBall()
    {
        if (_numOfActiveBalls < _maxNumOfBalls)
        {
            float rx = Random.Range(0f, 1f);
            float ry = Random.Range(0f, 1f);
            Vector3 ballpos = cam.ViewportToWorldPoint(new Vector3(rx, ry, 0));
            ballpos.z = 0f;
            GameObject ball = Instantiate(ballPrefab, ballpos, Quaternion.identity);
            BallScript ballRb = ball.GetComponent<BallScript>();
            _listOfActiveBalls.Add(ballRb);
            _numOfActiveBalls++;
            //ballRb.transform.parent.gameObject.SetActive(false);
            //ballRb.transform.gameObject.SetActive(false);
        }    
    }

    private void GenerateBalls()
    {
        while (_numOfDeactivatedBalls < _maxNumOfBalls)
        {
            float rx = Random.Range(0f, 1f);
            float ry = Random.Range(0f, 1f);
            Vector3 ballpos = cam.ViewportToWorldPoint(new Vector3(rx, ry, 0));
            ballpos.z = 0f;
            GameObject ball = Instantiate(ballPrefab, ballpos, Quaternion.identity);
            BallScript ballRb = ball.GetComponent<BallScript>();
            ballRb.SetStatusActive(false);
            _listOfDeactivatedBalls.Add(ballRb);
            _numOfDeactivatedBalls++;

        }    
    }

    private void ActivateBalls()
    {
        if (_listOfDeactivatedBalls.Count > 0)
        {
            BallScript bs = _listOfDeactivatedBalls[0];
            bs.SetStatusActive(true);
            _listOfActiveBalls.Add(bs);
            _listOfDeactivatedBalls.RemoveAt(0);
            _numOfActiveBalls ++;
            _numOfDeactivatedBalls --;
        }
    }

    void Awake() 
    {
        cam = Camera.main;
        _listOfActiveBalls = new List<BallScript>();
        _listOfDeactivatedBalls = new List<BallScript>();
        GenerateBalls();
        //EnableCollision(false);
        BoundSetup();
    }

    // Start is called before the first frame update
    void Start()
    {
        
        InvokeRepeating("ActivateBalls", 0.5f, 0.1f);
        InvokeRepeating("PrintInfo", 0f, 4f);
        Physics.IgnoreLayerCollision(8, 8);

    }

    // Update is called once per frame
    void Update()
    {
        //EnableCollision(false);
        //BoundSetup();
        
        
    }

    void FixedUpdate()
    {
        
        SystemGravity();
    }
}
