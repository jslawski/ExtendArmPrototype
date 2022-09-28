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
    
    [SerializeField]
    private LayerMask wallLayer;

    public bool isStationary = false;

    private Rigidbody handRb;

    private float forceCollisionPercentage = 0.3f;
    private float bodyForceCollisionPercentage = 0.75f;

    private Vector3 spawnPoint = Vector3.zero;

    private SphereCollider objectCollider;

    [SerializeField]
    private AudioSource wallBounceSound;
    [SerializeField]
    private AudioSource objectBounceSound;
    [SerializeField]
    private AudioSource destroyObjectSound;

    // Start is called before the first frame update
    void Start()
    {
        this.objectRb = GetComponent<Rigidbody>();
        this.grabbedLayer = LayerMask.NameToLayer("Grabbed");
        this.grabbableLayer = LayerMask.NameToLayer("Grabbable");

        this.gameObject.layer = this.grabbableLayer;

        this.spawnPoint = this.transform.position;

        this.objectCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    
    void Update()
    {
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
        this.objectRb.isKinematic = true;        
    }

    public void GetReleased(Vector2 releaseDirection, float releaseSpeed)
    {
        this.grabbed = false;
        this.objectRb.isKinematic = false;

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

    private void PreventObjectMovement(ContactPoint impedingContact)
    {
        if (impedingContact.separation < 0)
        {
            this.objectRb.position += (impedingContact.normal.normalized * Mathf.Abs(impedingContact.separation));           
            this.objectRb.velocity = Vector3.zero;
            
            if (this.handRb != null)
            {
                this.handRb.position += (impedingContact.normal.normalized * Mathf.Abs(impedingContact.separation));
                this.handRb.velocity = Vector3.zero;
            }
        }
    }

    private void ReflectObject(Vector3 collisionNormal)
    {
        this.objectRb.velocity = this.objectRb.velocity - (2 * Vector3.Dot(this.objectRb.velocity, collisionNormal) * collisionNormal);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (this.isStationary == false && collision.collider.tag == "GrabbableObject")
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
                forceMagnitude = (this.objectRb.velocity.magnitude * this.bodyForceCollisionPercentage) * this.objectRb.mass;
            }

            otherRb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);

            //StartCoroutine(this.DrawThingy(forceDirection));
            //float raycastLength = ((this.objectCollider.radius * this.gameObject.transform.localScale.x) + 0.5f);
            //if (Physics.Raycast(this.transform.position, forceDirection, raycastLength, this.wallLayer))
            // {
            //     Debug.LogError("NotApplyingForce!");
            // }
            // 

            this.objectBounceSound.pitch = Mathf.Lerp(1.8f, 1.0f, this.objectRb.mass / 3.0f);
            this.objectBounceSound.Play();
        }

        if (collision.collider.tag == "Wall" && this.grabbed == false)
        {
            this.ReflectObject(collision.GetContact(0).normal);

            this.wallBounceSound.pitch = Mathf.Lerp(1.8f, 1.0f, this.objectRb.mass / 3.0f);
            this.wallBounceSound.Play();
        }
    }

    private IEnumerator DrawThingy(Vector3 forceDirection)
    {
        float duration = 0f;
        while (duration < 10f)
        {
            float magnitude = ((this.objectCollider.radius * this.gameObject.transform.localScale.x) + 0.5f);
            Debug.DrawRay(this.transform.position, -forceDirection * magnitude, Color.green);
            duration += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (this.grabbed == false && collision.collider.tag == "DestroyCollider")
        {
            this.gameObject.transform.position = this.spawnPoint;
            this.objectRb.velocity = Vector3.zero;

            this.destroyObjectSound.Play();
        }

        if (collision.gameObject.tag == "Wall")
        {
            if (this.grabbed == true)
            {
                this.PreventObjectMovement(collision.GetContact(0));
            }

        }

    }
}
