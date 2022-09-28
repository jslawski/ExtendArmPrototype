using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField]
    private GrabbableObject grabbedObject;
    [SerializeField]
    private LayerMask detectableLayer;
    [SerializeField]
    private AudioSource closeHandSound;
    [SerializeField]
    private AudioSource grabSound;
    [SerializeField]
    private AudioSource punchSound;
    [SerializeField]
    private AudioSource armStretchSound;

    private Queue<Vector3> directionHistory;
    private Queue<Vector3> velocityHistory;
    [SerializeField, Range(0,30)]
    private int maxQueueSize = 0;

    private int frameDelay = 3;

    private Vector2 metaDirection = Vector2.zero;

    private Coroutine throwingCoroutine = null;

    private PlayerMovement player;

    private bool grabbing = false;

    private float forceCollisionPercentage = 0.5f;

    public float analogDeadZoneMagnitude = 0.3f;

    public Transform bottomTransform;

    private Coroutine rumbleCoroutine = null;

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

        Gamepad.current.SetMotorSpeeds(0.0f, 0.0f);
    }

    private void InitiateGrab()
    {
        if (this.handSpriteRenderer.sprite != this.openHandSprite)
        {
            return;
        }

        this.closeHandSound.pitch = Random.Range(0.5f, 1.5f);
        this.closeHandSound.Play();

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
                    this.player.LatchPlayer();
                }

                this.grabSound.Play();
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
                this.player.UnlatchPlayer();
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
        Vector3 metaDirection3D = new Vector3(this.metaDirection.x, this.metaDirection.y, 0.0f);
        Vector3 reflectionNormal = (this.handRb.transform.position - this.player.transform.position).normalized;
        Vector3 reflectionVector = (metaDirection3D - (2 * Vector3.Dot(metaDirection3D.normalized, reflectionNormal) * reflectionNormal));

        float handRotationAngle = (Mathf.Atan2(reflectionVector.y, reflectionVector.x) * Mathf.Rad2Deg) + 90f;
        this.handSpriteRenderer.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -handRotationAngle);

        if (this.grabbedObject != null)
        {
            if (this.armStretchSound.isPlaying == false)
            {
                this.armStretchSound.Play();
            }
            else
            {
                float lerpedValue = Mathf.Lerp(1.0f, 3.0f, this.handRb.velocity.magnitude / 50.0f);

                this.armStretchSound.pitch = lerpedValue;
            }
            
            if (this.grabbedObject.isStationary == false)
            {
                this.GrabRumble();

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
            this.armStretchSound.Stop();
            if (this.rumbleCoroutine == null)
            {
                Gamepad.current.SetMotorSpeeds(0.0f, 0.0f);
            }

            if (Vector3.Distance(this.handRb.position, this.player.armHand.transform.position) < 1.0f &&
                    this.metaDirection == Vector2.zero)
            {
                Destroy(this.gameObject);
            }
        }
        
    }
    private void GrabRumble()
    {
        float motorLerpValue = Mathf.Lerp(0.0f, 1.0f, this.handRb.velocity.magnitude / 50.0f);
        motorLerpValue *= (this.grabbedObject.objectRb.mass / 3.0f);

        float leftMult = this.metaDirection.x < 0 ? Mathf.Abs(this.metaDirection.x) : 0.0f;
        float rightMult = this.metaDirection.x > 0 ? Mathf.Abs(this.metaDirection.x) : 0.0f;

        float leftAdd = 0.0f;
        float rightAdd = 0.0f;
        /*
        if (this.metaDirection.x <= 0)
        {
            leftAdd = Mathf.Abs(this.metaDirection.y);
        }
        if (this.metaDirection.x >= 0)
        {
            leftAdd = Mathf.Abs(this.metaDirection.y);
        }
        */
        float leftResult = (motorLerpValue * leftMult) + leftAdd;
        float rightResult = (motorLerpValue * rightMult) + rightAdd;


        Gamepad.current.SetMotorSpeeds(leftResult, rightResult);        
    }

    private void OnEnable()
    {
        this.controls.PlayerMap.Enable();
    }

    private void PreventHandMovement(ContactPoint impedingContact)
    {
        if (impedingContact.separation < 0)
        {
            this.handRb.position += (impedingContact.normal.normalized * Mathf.Abs(impedingContact.separation));
            this.handRb.velocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "GrabbableObject")
        {
            if (this.grabbing == true && this.grabbedObject == null)
            {
                Rigidbody objectRb = other.gameObject.GetComponent<Rigidbody>();

                Vector3 forceDirection = this.metaDirection.normalized;//(objectRb.position - this.handRb.position).normalized;
                float forceMagnitude = (this.handRb.velocity.magnitude * this.forceCollisionPercentage) * objectRb.mass;

                objectRb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);                

                if (this.punchSound.isPlaying == false)
                {
                    float lerpedValue = Mathf.Lerp(0.1f, 1.0f, this.handRb.velocity.magnitude / 50f);

                    this.punchSound.pitch = Random.Range(0.8f, 1.5f);

                    this.punchSound.volume = lerpedValue;

                    this.punchSound.Play();
                    if (this.rumbleCoroutine == null)
                    {
                        lerpedValue *= (objectRb.mass / 3.0f);
                        this.rumbleCoroutine = StartCoroutine(this.Rumble(lerpedValue));
                    }
                }
            }
        }
    }

    private IEnumerator Rumble(float value)
    {        
        Gamepad.current.SetMotorSpeeds(value, value);

        int waitFrames = 10;

        for (int i = 0; i < waitFrames; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        Gamepad.current.SetMotorSpeeds(0.0f, 0.0f);        
        this.rumbleCoroutine = null;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Wall" && this.grabbedObject == null)
        {
            this.PreventHandMovement(collision.GetContact(0));
        }
    }
}
