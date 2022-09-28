using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Range(0, 100)]
    private float maxSpeed = 10f;

    private Rigidbody playerRB;

    PlayerControls controls;

    Vector2 moveVector = Vector2.zero;

    public float analogDeadZoneMagnitude = 0.3f;

    private Vector2 armDirection = Vector2.zero;

    [SerializeField, Range(0, 100)]
    private float maxArmDistance = 10f;

    public GameObject armHand;

    private Vector3 armTargetPosition;

    [SerializeField]
    private GameObject gameHandPrefab;
    private Rigidbody gameHandRB;
    private Vector3 gameHandTargetPosition;

    [SerializeField, Range(0, 500)]
    float gameHandFollowSpeed = 0.0f;

    [SerializeField, Range(0, 500)]
    float latchedPlayerFollowSpeed = 5.0f;

    [SerializeField]
    private BezierArmRenderer armRender;

    private bool latched = false;

    [SerializeField]
    private GameObject instructionsObject;

    void Awake()
    {
        this.controls = new PlayerControls();

        this.controls.PlayerMap.Move.performed += context => this.moveVector = context.ReadValue<Vector2>();
        this.controls.PlayerMap.Move.canceled += context => this.moveVector = Vector2.zero;

        this.controls.PlayerMap.Arm.performed += context => this.armDirection = context.ReadValue<Vector2>();
        this.controls.PlayerMap.Arm.canceled += context => this.armDirection = Vector2.zero;

        this.playerRB = GetComponent<Rigidbody>();

        this.instructionsObject = GameObject.Find("Instructions");
    }

    // Update is called once per frame
    void Update()
    {       
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (this.instructionsObject.activeSelf == true)
        {
            return;
        }

        if (this.moveVector.magnitude < this.analogDeadZoneMagnitude)
        {
            this.moveVector = Vector2.zero;
        }

        if (this.armDirection.magnitude < this.analogDeadZoneMagnitude)
        {
            this.armDirection = Vector2.zero;
        }

        Vector2 armDirection2D = (this.armDirection * this.maxArmDistance);
        Vector3 directionVector = new Vector3(armDirection2D.x, armDirection2D.y, 0.0f);
        this.armTargetPosition = this.transform.position + directionVector;

        if (directionVector != Vector3.zero)
        {
            if (this.gameHandRB == null)
            {
                GameObject gameHandInstance = Instantiate(this.gameHandPrefab, this.transform.position, new Quaternion());
                this.gameHandRB = gameHandInstance.GetComponent<Rigidbody>();
                Transform bottomHandTransform = gameHandInstance.GetComponent<HandControls>().bottomTransform;
                this.armRender.SetupArmRenderer(bottomHandTransform, this.armHand.transform);
            }

            this.gameHandTargetPosition = Vector3.Lerp(this.gameHandRB.position,
                                           this.armTargetPosition,
                                           this.gameHandFollowSpeed * Time.fixedDeltaTime);
        }        
        
        if (this.armDirection.magnitude < this.analogDeadZoneMagnitude)
        {
            if (this.gameHandRB != null)
            {
                this.gameHandTargetPosition = Vector3.Lerp(this.gameHandRB.position,
                                           this.armTargetPosition,
                                           this.gameHandFollowSpeed * Time.fixedDeltaTime);
            }
        }
    }

    public void LatchPlayer()
    {
        this.latched = true;
        Gamepad.current.SetMotorSpeeds(0.1f, 0.1f);
    }

    public void UnlatchPlayer()
    {
        this.latched = false;
        Gamepad.current.SetMotorSpeeds(0.0f, 0.0f);
    }

    private void FixedUpdate()
    {        
        if (this.latched == false)
        {
            this.MoveFreely();            
        }
        else
        {
            this.MoveLatched();
        }
    }

    private void MoveFreely()
    {
        Vector2 velocity2D = (this.moveVector * this.maxSpeed * Time.fixedDeltaTime);
        Vector3 velocityVector = new Vector3(velocity2D.x, velocity2D.y, 0.0f);
        Vector3 newPosition = this.transform.position + velocityVector;

        this.playerRB.MovePosition(newPosition);

        this.armHand.transform.position = this.armTargetPosition;

        if (this.gameHandRB != null)
        {
            this.gameHandRB.MovePosition(this.gameHandTargetPosition);
        }
    }

    private void MoveLatched()
    {
        Vector3 armOffset = new Vector3(-this.armDirection.x, -this.armDirection.y, 0.0f).normalized * (this.armDirection.magnitude * this.maxArmDistance);

        Vector3 targetPosition = (this.gameHandRB.position + armOffset);

        Vector3 newPosition = Vector3.Lerp(this.playerRB.position,
                                           targetPosition,
                                           this.latchedPlayerFollowSpeed * Time.fixedDeltaTime);

        this.armHand.transform.position = this.armTargetPosition;

        this.playerRB.MovePosition(newPosition);
    }

    private void OnEnable()
    {
        this.controls.PlayerMap.Enable();
    }

    private void PreventPlayerMovement(ContactPoint impedingContact)
    {        
        if (impedingContact.separation < 0)
        {
            this.playerRB.position += (impedingContact.normal.normalized * Mathf.Abs(impedingContact.separation));
            this.playerRB.velocity = Vector3.zero;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (this.latched == false && collision.gameObject.tag == "DestroyCollider")
        {
            
            this.PreventPlayerMovement(collision.GetContact(0));
        }

        if (collision.gameObject.tag == "Wall")
        {
            this.PreventPlayerMovement(collision.GetContact(0));
        }
    }

    private void OnDestroy()
    {
        this.controls.Disable();
    }
}
