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

    private Color _attractionColor;
    private Color _repulsionColor;

    private Renderer _renderer;
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

    public int MadeOfComponents
    {
        get {return _madeOfComponents;}
        set {_madeOfComponents = value;}
    }

    public void SwitchColor(int flag)
    {
        if (flag == 1)
        {
            _renderer.material.SetColor("_EmissionColor", _attractionColor);
        }
        else 
        {
            _renderer.material.SetColor("_EmissionColor", _repulsionColor);
        }
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

        if (collision.gameObject.layer == 8)
        {
            //this.transform.localScale = 2 * this.transform.localScale;
        }
    }

    void OnTriggerEnter(Collider other)
    {

    }
    void Awake()
    {
        // setup rigid body of the ball in awake 
        _rigidbody = this.GetComponent<Rigidbody>();
        _collider = this.GetComponent<Collider>();
        _rigidbody.useGravity = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
        _rigidbody.mass = _mass;

        //setup render
        _renderer = this.GetComponent<Renderer>();
        _renderer.material.EnableKeyword("_EMISSION");
        _attractionColor = new Color(1.220106f, 0.9760846f + Random.Range(0f, 1f), 2.118547f, 0.25f);
        _repulsionColor = new Color(2.22f, 0.1f + Random.Range(0f, 1f), 0.2f, 0.3f);
        
    }


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
