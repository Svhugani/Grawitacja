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
    public GameObject bound;
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
        Renderer rend = bound.GetComponent<Renderer>();
        Vector3 boundsize = rend.bounds.size;
        Debug.Log("Bound size: " + boundsize.ToString());

        if (cam)
        {
            float xScale = cam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
            float yScale = cam.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
            Vector3 boundScale = bound.transform.localScale;
            boundScale.x = xScale;
            boundScale.y = yScale;
            bound.transform.localScale = boundScale;
        }
    }

    private Vector3 PartialGravity(Rigidbody ballRb, Rigidbody attractorRb)
    {
        Vector3 distVec = attractorRb.transform.position - ballRb.transform.position;
        float magnitude = distVec.sqrMagnitude + 0.00001f; 
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
            ballRb.BallRigidBody.AddForce(TotalGravity(ballRb.BallRigidBody));
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
        
        InvokeRepeating("ActivateBalls", 0.5f, 0.25f);
        InvokeRepeating("PrintInfo", 0f, 2f);

    }

    // Update is called once per frame
    void Update()
    {
        EnableCollision(false);
        
        
    }

    void FixedUpdate()
    {
        
        SystemGravity();
    }
}
