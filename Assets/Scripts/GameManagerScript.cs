using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField] private int _maxNumOfBalls;
    private int _numOfActiveBalls = 0;
    public GameObject ballPrefab;
    public GameObject bound;
    private GameObject[] _ballsArray; 
    private Camera cam;
    private float _gravityConstant = .5f;

    private Vector3 PartialGravity(GameObject ball, GameObject attractor)
    {
        Vector3 distVec = attractor.transform.position - ball.transform.position;
        float magnitude = distVec.sqrMagnitude;

        if (magnitude > 0)
        {
            return (attractor.GetComponent<Rigidbody>().mass / magnitude) * distVec.normalized;
        }

        else 
        {
            return Vector3.zero;
        }

    }
    private Vector3 TotalGravity(GameObject ball)
    {
        Vector3 totalForce = Vector3.zero;
        foreach ( GameObject otherBall in  _ballsArray)
        {
            if (otherBall.activeInHierarchy)
            {
                totalForce += PartialGravity(ball, otherBall);
            }
        }
        totalForce = _gravityConstant * ball.GetComponent<Rigidbody>().mass * totalForce;
        return totalForce;
    }

    private void SystemGravity()
    {
        foreach (GameObject ball in _ballsArray)
        {
            ball.GetComponent<Rigidbody>().AddForce(TotalGravity(ball));
        }
    }
    private void GenerateBalls()
    {
        Vector3 pointMap = new Vector3();
        for (int i = 0; i < _maxNumOfBalls; i++)
        {
            float rx = Random.Range(0f, 1f);
            float ry = Random.Range(0f, 1f);
            pointMap = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * rx, Screen.height * ry, 0));
            //Vector3 viewPos = cam.WorldToViewportPoint(position)
            Vector3 ballpos = cam.ViewportToWorldPoint(new Vector3(rx, ry, 0));
            ballpos.z = 0f;
            pointMap = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            GameObject ball = Instantiate(ballPrefab, ballpos, Quaternion.identity);
            ball.SetActive(false);
            _ballsArray[i] = ball;

        }
    }

    private void ActivateBall()
    {
        if (_numOfActiveBalls < _maxNumOfBalls)
        {
            _ballsArray[_numOfActiveBalls].SetActive(true);
            _numOfActiveBalls ++;
        }
    }

    void Awake() 
    {
        cam = Camera.main;
       _ballsArray = new GameObject[_maxNumOfBalls];
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateBalls();
        InvokeRepeating("ActivateBall", 1f, 0.25f);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Number of balls: " + _numOfActiveBalls.ToString());
        
    }

    void FixedUpdate()
    {
        SystemGravity();
    }
}
