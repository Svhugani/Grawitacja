using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField] private int _maxNumOfBalls;
    [SerializeField] private int _numOfBallsToSplit;
    private int _numOfActiveBalls = 0;
    public GameObject ballPrefab;
    public GameObject bound;
    private List<Rigidbody> _listOfActiveBalls;
    private List<Rigidbody> _listOfDeactivatedBalls;  
    private Camera cam;
    private float _gravityConstant = .05f;

    private void PrintInfo()
    {
        Debug.Log("Number of active balls: " + _numOfActiveBalls.ToString());
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
        foreach ( Rigidbody otherBallRb in  _listOfActiveBalls)
        {

            totalForce += PartialGravity(ballRb, otherBallRb);
            
        }
        totalForce = _gravityConstant * ballRb.mass * totalForce;
        return totalForce;
    }

    private void SystemGravity()
    {
        foreach (Rigidbody ballRb in _listOfActiveBalls)
        {
            ballRb.AddForce(TotalGravity(ballRb));
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
            Rigidbody ballRb = ball.GetComponent<Rigidbody>();
            _listOfActiveBalls.Add(ballRb);
            _numOfActiveBalls++;
            ballRb.transform.parent.gameObject.SetActive(false);
        }    
    }

    void Awake() 
    {
        cam = Camera.main;
        _listOfActiveBalls = new List<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("GenerateBall", 0.5f, 0.25f);
        InvokeRepeating("PrintInfo", 0f, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    void FixedUpdate()
    {
        SystemGravity();
    }
}
