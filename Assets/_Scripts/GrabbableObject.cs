using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    [HideInInspector]
    public bool grabbed = false;

    [HideInInspector]
    public Rigidbody objectRb;

    private float maxSpeed = 50f;

    private int grabbedLayer;
    private int grabbableLayer;

    public bool isStationary = false;

    private Rigidbody handRb;

    private float forceCollisionPercentage = 0.75f;

    private Vector3 spawnPoint = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        this.objectRb = GetComponent<Rigidbody>();
        this.grabbedLayer = LayerMask.NameToLayer("Grabbed");
        this.grabbableLayer = LayerMask.NameToLayer("Grabbable");

        this.gameObject.layer = this.grabbableLayer;

        this.spawnPoint = this.transform.position;
    }

    // Update is called once per frame
    
    void Update()
    {
        /*
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
        */

        if (this.objectRb.velocity.magnitude > this.maxSpeed)
        {
            this.objectRb.velocity = this.objectRb.velocity.normalized * this.maxSpeed;
        }
    }
    
    public void GetGrabbed(Rigidbody hand)
    {
        this.grabbed = true;
        this.gameObject.layer = this.grabbedLayer;
        this.handRb = hand;
    }

    public void GetReleased(Vector2 releaseDirection, float releaseSpeed)
    {
        this.grabbed = false;

        if (this.isStationary == false)
        {
            this.objectRb.velocity = new Vector3(releaseDirection.x, releaseDirection.y, 0.0f).normalized * releaseSpeed;

            if (this.objectRb.velocity.magnitude > this.maxSpeed)
            {
                this.objectRb.velocity = releaseDirection.normalized * this.maxSpeed;
            }

            Invoke("LayerReset", 0.2f);
        }
        else
        {
            this.LayerReset();
        }
    }

    private void LayerReset()
    {
        this.gameObject.layer = this.grabbableLayer;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "GrabbableObject")
        {
            Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>();

            Vector3 forceDirection = Vector3.zero;
            float forceMagnitude = 0.0f;

            if (this.grabbed == true)
            {                
                forceDirection = (otherRb.position - this.objectRb.position).normalized;
                forceMagnitude = (this.handRb.velocity.magnitude * this.forceCollisionPercentage) * this.objectRb.mass;
            }
            else
            {                
                forceDirection = (otherRb.position - this.objectRb.position).normalized;
                forceMagnitude = (this.objectRb.velocity.magnitude * this.forceCollisionPercentage) * this.objectRb.mass;
            }

            otherRb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (this.grabbed == false && collision.collider.tag == "DestroyCollider")
        {
            this.gameObject.transform.position = this.spawnPoint;
            this.objectRb.velocity = Vector3.zero;
        }

        if (collision.collider.tag == "Wall")
        {
            if (this.grabbed == true)
            { 
            
            }
        }
    }
}
