using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundScript : MonoBehaviour
{
    // Start is called before the first frame update

    private void OnCollisionEnter(Collision other) 
    {
        Debug.Log("HITTED BOUND!");
    }

    private void OnCollisionExit(Collision other) 
    {
        Debug.Log("Exit BOUND!");
    }

    private void OnTriggerExit(Collider other) 
    {
        Debug.Log("BOUND ENTER!");    
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
