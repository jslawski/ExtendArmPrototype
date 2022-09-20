using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    public bool grabbed = false;

    [HideInInspector]
    public Rigidbody objectRb;

    // Start is called before the first frame update
    void Start()
    {
        this.objectRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetGrabbed()
    {
        this.grabbed = true;
    }


}
