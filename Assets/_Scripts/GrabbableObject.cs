using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    public bool grabbed = false;

    [HideInInspector]
    public Rigidbody objectRb;

    private Collider objectCollider;

    public float maxSpeed;

    private int grabbedLayer;
    private int grabbableLayer;

    // Start is called before the first frame update
    void Start()
    {
        this.objectRb = GetComponent<Rigidbody>();
        this.grabbedLayer = LayerMask.NameToLayer("Grabbed");
        this.grabbableLayer = LayerMask.NameToLayer("Grabbable");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(this.gameObject.transform.position);

        if (this.objectRb.velocity.magnitude <= 1 && this.grabbed == false)
        {
            if (viewportPos.x <= 0.0f || viewportPos.x > 1.0f)
            {
                Destroy(this.gameObject);
            }
            if (viewportPos.y <= 0.0f || viewportPos.y > 1.0f)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void GetGrabbed()
    {
        this.grabbed = true;
        this.gameObject.layer = this.grabbedLayer;
    }

    public void GetReleased(Vector3 handVelocity)
    {
        this.grabbed = false;        
        this.objectRb.velocity = handVelocity;
        

        if (this.objectRb.velocity.magnitude > this.maxSpeed)
        {
            this.objectRb.velocity = handVelocity.normalized * this.maxSpeed;
        }

        Invoke("DelayedLayerReset", 0.2f);
    }

    private void DelayedLayerReset()
    {
        this.gameObject.layer = this.grabbableLayer;
    }
}
