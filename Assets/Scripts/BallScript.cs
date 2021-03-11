using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Collider _collider;
    private Vector3 _globalForce = Vector3.zero;
    private float _mass = 1f;
    private int _madeOfComponents = 1;

    public Rigidbody BallRigidBody
    {
        get {return _rigidbody;}
        set {_rigidbody = value;}

    }

    public Collider BallCollider
    {
        get {return _collider;}
        set {_collider = value;}

    }

    public float Mass
    {
        get {return _mass;}
        set {_mass = value;}
    }

    public void Interact(Vector3 force)
    {
        _rigidbody.AddForce(force);
    }

    public void SetStatusActive(bool status)
    {
        this.transform.gameObject.SetActive(status);
    }
    
    void OnCollisionEnter(Collision collision)
    {
        //if ( _rigidbody.mass > collision.collider.gameObject.GetComponent<Rigidbody>().mass)
        //{

        //}
        
        if (collision.collider.gameObject.layer == 0)
        {
            Debug.Log("COLLISION !");   
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("TRIGGERRRED !");
    }
    void Awake()
    {
        // setup rigid body of the ball in awake 
        _rigidbody = this.GetComponent<Rigidbody>();
        _collider = this.GetComponent<Collider>();
        _rigidbody.useGravity = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
        _rigidbody.mass = _mass;

        
    }

    // Start is called before the first frame update
    void Start()
    {
        //transform.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {

    }
}
