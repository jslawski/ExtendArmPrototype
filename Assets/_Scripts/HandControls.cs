using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandControls : MonoBehaviour
{
    private PlayerControls controls;

    [SerializeField]
    private Rigidbody handRb;
    [SerializeField]
    private SpriteRenderer handSpriteRenderer;
    [SerializeField]
    private Sprite openHandSprite;
    [SerializeField]
    private Sprite grabHandSprite;
    [SerializeField]
    private BoxCollider handCollider;

    private GrabbableObject grabbedObject;

    [SerializeField]
    private LayerMask detectableLayer;

    private Queue<Vector3> directionHistory;
    private Queue<Vector3> velocityHistory;
    [SerializeField, Range(0,30)]
    private int maxQueueSize = 0;

    private int frameDelay = 3;

    private Vector2 metaDirection = Vector2.zero;

    private Coroutine throwingCoroutine = null;

    private PlayerMovement player;

    private bool grabbing = false;

    private float forceCollisionPercentage = 1.0f;

    public float analogDeadZoneMagnitude = 0.3f;

    private void Awake()
    {
        this.controls = new PlayerControls();
        this.directionHistory = new Queue<Vector3>();
        this.velocityHistory = new Queue<Vector3>();

        this.player = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.controls.PlayerMap.Grab.performed += context => this.InitiateGrab();
        this.controls.PlayerMap.Grab.canceled += context => this.CancelGrab();

        this.controls.PlayerMap.Grab2.performed += context => this.InitiateGrab();
        this.controls.PlayerMap.Grab2.canceled += context => this.CancelGrab();

        this.controls.PlayerMap.Arm.performed += context => this.metaDirection = context.ReadValue<Vector2>();
        this.controls.PlayerMap.Arm.canceled += context => this.metaDirection = Vector2.zero;

        /*if (this.controls.PlayerMap.Grab.() || this.controls.PlayerMap.Grab2.WasPerformedThisFrame())
        {
            this.InitiateGrab();
        }
        */
    }    

    private void OnDestroy()
    {
        this.controls.PlayerMap.Grab.performed -= context => this.InitiateGrab();
        this.controls.PlayerMap.Grab.canceled -= context => this.CancelGrab();

        this.controls.PlayerMap.Grab2.performed -= context => this.InitiateGrab();
        this.controls.PlayerMap.Grab2.canceled -= context => this.CancelGrab();

        this.controls.PlayerMap.Arm.performed -= context => this.metaDirection = context.ReadValue<Vector2>();
        this.controls.PlayerMap.Arm.canceled -= context => this.metaDirection = Vector2.zero;

        this.controls.Disable();
    }

    private void InitiateGrab()
    {
        if (this.handSpriteRenderer.sprite != this.openHandSprite)
        {
            return;
        }

        this.grabbing = true;

        if (metaDirection.magnitude >= this.analogDeadZoneMagnitude)
        {
            RaycastHit hitInfo;

            Vector3 spherecastOrigin = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, -10f);

            if (Physics.SphereCast(spherecastOrigin, 1.0f, Vector3.forward, out hitInfo, Mathf.Infinity, this.detectableLayer))
            {
                this.grabbedObject = hitInfo.rigidbody.gameObject.GetComponent<GrabbableObject>();
                this.grabbedObject.GetGrabbed(this.handRb);

                if (this.grabbedObject.isStationary == true)
                {
                    this.player.latched = true;
                }
            }
        }

        this.handSpriteRenderer.sprite = this.grabHandSprite;
        this.handCollider.enabled = true;
    }

    private void CancelGrab()
    {
        if (this.grabbedObject != null)
        {
            if (this.grabbedObject.isStationary == false)
            {
                this.ThrowDynamicObject();
            }
            else
            {
                this.player.latched = false;
                this.grabbedObject.GetReleased(Vector3.zero, 0.0f);
                this.grabbedObject = null;

                this.handSpriteRenderer.sprite = this.openHandSprite;
                this.handCollider.enabled = false;
                this.grabbing = false;
            }
        }
        else
        {
            this.handSpriteRenderer.sprite = this.openHandSprite;
            this.handCollider.enabled = false;
            this.grabbing = false;
        }
    }

    private IEnumerator DelayedCancelGrab()
    {
        int delayFrames = 0;

        while (delayFrames < this.frameDelay)
        {
            delayFrames++;
            yield return new WaitForFixedUpdate();
        }

        this.handSpriteRenderer.sprite = this.openHandSprite;
        this.handCollider.enabled = false;
        this.grabbing = false;

        if (this.grabbedObject != null)
        {
            if (this.grabbedObject.isStationary == false)
            {
                Vector3[] finalVelocityHistory = this.velocityHistory.ToArray();

                this.grabbedObject.GetReleased(this.metaDirection, finalVelocityHistory[0].magnitude);
            }
        }

        this.grabbedObject = null;
        this.throwingCoroutine = null;
        this.grabbing = false;
    }

    private void ThrowDynamicObject()
    {
        if (GameManager.throwingMode == ThrowMode.Delay)
        {
            this.handSpriteRenderer.sprite = this.openHandSprite;
            this.handCollider.enabled = false;

            Vector3[] finalDirectionHistory = this.directionHistory.ToArray();
            this.grabbedObject.GetReleased(finalDirectionHistory[0], this.handRb.velocity.magnitude);
            this.grabbedObject = null;
        }
        else
        {
            if (this.throwingCoroutine == null)
            {
                this.throwingCoroutine = StartCoroutine(this.DelayedCancelGrab());
            }
        }
    }

    private void Update()
    {
        if (this.grabbedObject != null)
        {
            if (this.grabbedObject.isStationary == false)
            {
                this.grabbedObject.objectRb.MovePosition(this.gameObject.transform.position);

                this.directionHistory.Enqueue(this.metaDirection);
                this.velocityHistory.Enqueue(this.handRb.velocity);

                if (this.directionHistory.Count > this.maxQueueSize)
                {
                    this.directionHistory.Dequeue();
                }

                if (this.velocityHistory.Count > this.maxQueueSize)
                {
                    this.velocityHistory.Dequeue();
                }
            }
        }
        else
        {
            if (Vector3.Distance(this.handRb.position, this.player.armHand.transform.position) < 1.0f &&
                    this.metaDirection == Vector2.zero)
            {
                Destroy(this.gameObject);
            }
        }
        
    }

    private void OnEnable()
    {
        this.controls.PlayerMap.Enable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "GrabbableObject")
        {
            if (this.grabbing == true && this.grabbedObject == null)
            {
                Rigidbody objectRb = other.gameObject.GetComponent<Rigidbody>();

                Vector3 forceDirection = this.metaDirection.normalized;//(objectRb.position - this.handRb.position).normalized;
                float forceMagnitude = (this.handRb.velocity.magnitude * this.forceCollisionPercentage) * objectRb.mass;

                objectRb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
            }
        }
    }
}
