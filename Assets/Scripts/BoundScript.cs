using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundScript : MonoBehaviour
{
    private float[] _lista = new float[]{1,2,3,4,56};
    private List<float> _lista2 = new List<float>();
    void Iterate()
    {
        foreach (float item in _lista2.ToArray())
        {
            if (item == 7)
            {
                _lista2.Remove(item);
            }
            Debug.Log("ELEMENT: " + item.ToString());
        }
    }
    private void OnCollisionEnter(Collision other) 
    {

    }

    private void OnCollisionExit(Collision other) 
    {

    }

    private void OnTriggerEnter(Collider other) 
    {
  
    }

    void Awake() 
    {
        _lista2.Add(5);
        _lista2.Add(7);
        _lista2.Add(10);
    }

    void Start()
    {
        Iterate();
        Iterate();
    }

    void Update()
    {
        
    }
}
