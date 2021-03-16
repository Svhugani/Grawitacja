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
    public GameObject background;
    private List<BallScript> _listOfActiveBalls;
    private List<BallScript> _listOfDeactivatedBalls;  
    private Camera cam;
    private float _gravityConstant = .15f;
    private float _initMass;
    private Vector3 _initScale;
    private float _randomForce = 100f;
    private bool _isConnectionEnabled = true;
    private int _forceType = 1;
    private float _randomPhysics;


    private void PrintInfo()
    {
        Debug.Log("Number of active balls: " + _numOfActiveBalls.ToString());
        Debug.Log("Number of deactivated balls: " + _numOfDeactivatedBalls.ToString());
    
    }

    private void EnableConnecitons()
    {
        _isConnectionEnabled = true;
        //Debug.Log("CONNECTIONS ENABLED !!!!!!");
    }

    private void RandomSwitchForceType()
    {
        _randomPhysics = Random.Range(0f, 1f);
        // on average switch physics every 5 sec
        if (_randomPhysics < 0.004)
        {
            _forceType *= -1;
            foreach (BallScript ballSc in _listOfActiveBalls)
            {
                ballSc.SwitchColor(_forceType);
            }
            foreach (BallScript ballSc in _listOfDeactivatedBalls)
            {
                ballSc.SwitchColor(_forceType);
            }
        }
    }

    private void BoundSetup()
    {   
        // Set up bounding scene depending on your screen resolution / ratio.
        // Bounds are build out of 4 initial cubes placed in the center of the scene
        // Assumption: all initial bounds are cubes. Thus, they have the same scales

        Renderer rend = upperBound.GetComponent<Renderer>();
        Vector3 boundSize = rend.bounds.size;
        Vector3 boundScale;

        upperBound.GetComponent<MeshRenderer>().enabled = false;
        lowerBound.GetComponent<MeshRenderer>().enabled = false;
        rightBound.GetComponent<MeshRenderer>().enabled = false;
        leftBound.GetComponent<MeshRenderer>().enabled = false;

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

    private void SetUpBackground()
    {
        float xScale = cam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        float yScale = cam.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        background.transform.localScale = new Vector3(xScale, yScale ,1);
    }

    private Vector3 PartialGravity(Rigidbody ballRb, Rigidbody attractorRb)
    {
        
        Vector3 distVec = attractorRb.position - ballRb.position;
        float magnitude = distVec.sqrMagnitude; 
        return (attractorRb.mass / (magnitude * Mathf.Sqrt(magnitude))) * distVec;
        
        
    }

    private void SplitBall(BallScript Ballsc)
    {   
        Debug.Log("SPLITTING BALL");
        _isConnectionEnabled = false;
        Ballsc.BallRigidBody.mass = _initMass;
        Vector3 randomDirection;
        randomDirection = Random.onUnitSphere;
        randomDirection.z = 0;
        Ballsc.gameObject.transform.localScale = _initScale;
        Ballsc.Interact(_randomForce * randomDirection);
        Vector3 targetPos = Ballsc.gameObject.transform.position;

        int ballsToAdd = Mathf.Min(_numOfDeactivatedBalls, Ballsc.MadeOfComponents) - 1;
        int i = 0;

        Ballsc.MadeOfComponents = 1;

        while (i < ballsToAdd)
        {
            BallScript otherBallsc = _listOfDeactivatedBalls[0];
            otherBallsc.SetStatusActive(true);
            _listOfActiveBalls.Add(otherBallsc);
            _listOfDeactivatedBalls.Remove(otherBallsc);
            _numOfActiveBalls ++;
            _numOfDeactivatedBalls --;
            otherBallsc.gameObject.transform.localScale = _initScale;
            otherBallsc.gameObject.transform.position = targetPos;
            otherBallsc.BallRigidBody.mass = _initMass;
            randomDirection = Random.onUnitSphere;
            randomDirection.z = 0;
            otherBallsc.Interact(_randomForce * randomDirection);
            otherBallsc.MadeOfComponents = 1;
            i ++;
        }
        Invoke("EnableConnecitons", .2f);
    }

    private Vector3 TotalGravity(BallScript ballSc)
    {
        Vector3 totalForce = Vector3.zero;
        Rigidbody ballRb = ballSc.BallRigidBody;
        float size = ballSc.transform.localScale.x;
        bool returnBall = true;
        _listOfActiveBalls.Remove(ballSc);
        Vector3 pos = ballSc.gameObject.transform.position;

        foreach ( BallScript otherBallSc in  _listOfActiveBalls.ToArray())
        {
            Vector3 distVec = otherBallSc.gameObject.transform.position - pos;
            float magnitude = distVec.magnitude;
            float otherSize = otherBallSc.transform.localScale.x;
            


            if( magnitude <= 0.501 * (size + otherSize)  )
            {
                if(ballRb.mass >= otherBallSc.BallRigidBody.mass)
                {
                    float f = Mathf.Sqrt(1 + otherSize / size);
                    ballRb.mass = Mathf.Pow(f, 3) * ballRb.mass;
                    ballSc.transform.localScale = f * ballSc.transform.localScale;
                    _listOfActiveBalls.Remove(otherBallSc);
                    _listOfDeactivatedBalls.Add(otherBallSc);
                    otherBallSc.SetStatusActive(false);    
                    ballSc.MadeOfComponents ++;
                    _numOfActiveBalls --;
                    _numOfDeactivatedBalls ++;          
                }
                
                else
                {
                    float f = Mathf.Sqrt(1 + size / otherSize);
                    otherBallSc.BallRigidBody.mass = Mathf.Pow(f, 3) * otherBallSc.BallRigidBody.mass;
                    otherBallSc.transform.localScale = f * otherBallSc.transform.localScale;
                    _listOfDeactivatedBalls.Add(ballSc);
                    ballSc.SetStatusActive(false);
                    returnBall = false;    
                    otherBallSc.MadeOfComponents ++;
                    _numOfActiveBalls --;
                    _numOfDeactivatedBalls ++;
                }
                break;
            }

            else
            {
                totalForce += _forceType * (otherBallSc.BallRigidBody.mass / ( Mathf.Pow(magnitude, 3))) * distVec;
            }
                
        }

        if (returnBall)
        {
            _listOfActiveBalls.Add(ballSc);
        }

        totalForce = _gravityConstant * ballRb.mass * totalForce;
        return totalForce;
    }

    private void SystemGravity()
    {
        foreach (BallScript ballSc in _listOfActiveBalls.ToArray())
        {
            if(ballSc.transform.gameObject.activeSelf)
            {
                if (ballSc.MadeOfComponents > _numOfBallsToSplit)
                {
                    SplitBall(ballSc);
                }
                else
                {
                    if (_isConnectionEnabled)
                    {
                        ballSc.Interact(TotalGravity(ballSc));
                    }   
                }
            }


            
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

    private void ActivateBall()
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

    public void DectivateBall( BallScript bs)
    {
        _listOfActiveBalls.Remove(bs);
        bs.SetStatusActive(false);
        _listOfDeactivatedBalls.Add(bs);
        _numOfActiveBalls --;
        _numOfDeactivatedBalls ++;
    }

    void Awake() 
    {
        cam = Camera.main;
        _listOfActiveBalls = new List<BallScript>();
        _listOfDeactivatedBalls = new List<BallScript>();
        GenerateBalls();
        BoundSetup();
        SetUpBackground();
        _initMass = _listOfDeactivatedBalls[0].BallRigidBody.mass;
        _initScale = _listOfDeactivatedBalls[0].gameObject.transform.localScale;
    }

    // Start is called before the first frame update
    void Start()
    {
        
        InvokeRepeating("ActivateBall", 0.5f, 0.05f);
        InvokeRepeating("PrintInfo", 0f, 4f);
        //Physics.IgnoreLayerCollision(8, 8);

    }

    // Update is called once per frame
    void Update()
    {
   
    }

    void FixedUpdate()
    {
        
        SystemGravity();
        RandomSwitchForceType();
    }
}
