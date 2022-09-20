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

    // Start is called before the first frame update
    void Start()
    {
        this.objectRb = GetComponent<Rigidbody>();
        this.objectCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(this.gameObject.transform.position);

        if (viewportPos.x <= 0.0f || viewportPos.x > 1.0f)
        {
            Destroy(this.gameObject);
        }
        if (viewportPos.y <= 0.0f || viewportPos.y > 1.0f)
        {
            Destroy(this.gameObject);
        }
    }

    public void GetGrabbed()
    {
        this.grabbed = true;
        this.objectCollider.enabled = false;
    }

    public void GetReleased(Vector3 handVelocity)
    {
        this.grabbed = false;        
        this.objectRb.velocity = handVelocity;

        if (this.objectRb.velocity.magnitude > this.maxSpeed)
        {
            this.objectRb.velocity = handVelocity.normalized * this.maxSpeed;
        }

        Invoke("DelayedColliderReEnable", 0.2f);
    }

    private void DelayedColliderReEnable()
    {
        this.objectCollider.enabled = true;
    }
}
