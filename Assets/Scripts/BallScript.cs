using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Vector3 _globalForce = Vector3.zero;
    private float _mass = 1f;
    private int _madeOfComponents = 1;

    public void Interact()
    {
        _rigidbody.AddForce(_globalForce);
    }

    void Awake()
    {
        // setup rigid body of the ball in awake 
        _rigidbody = this.GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
        _rigidbody.mass = _mass;

        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {

    }
}
