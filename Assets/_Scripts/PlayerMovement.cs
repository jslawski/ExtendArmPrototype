using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Range(0, 100)]
    private float maxSpeed = 10f;

    private Rigidbody playerRB;

    PlayerControls controls;

    Vector2 moveVector = Vector3.zero;
    Vector2 input = Vector2.zero;

    public float analogDeadZoneMagnitude = 0.3f;

    private Vector2 armDirection = Vector2.zero;

    [SerializeField, Range(0, 100)]
    private float maxArmDistance = 10f;

    [SerializeField]
    private GameObject armHand;

    private Vector3 armTargetPosition;

    [SerializeField]
    private GameObject gameHandPrefab;
    private Rigidbody gameHandRB;
    private Vector3 gameHandTargetPosition;

    [SerializeField, Range(0, 500)]
    float gameHandFollowSpeed = 0.0f;

    [SerializeField]
    private BezierArmRenderer armRender;

    void Awake()
    {
        this.controls = new PlayerControls();

 
        this.controls.PlayerMap.Move.performed += context => this.moveVector = context.ReadValue<Vector2>();
        this.controls.PlayerMap.Move.canceled += context => this.moveVector = Vector2.zero;

        this.controls.PlayerMap.Arm.performed += context => this.armDirection = context.ReadValue<Vector2>();
        this.controls.PlayerMap.Arm.canceled += context => this.armDirection = Vector2.zero;

        this.playerRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        // 3d Move test
        Vector3 m = new Vector3(moveVector.x, 0, moveVector.y) * Time.deltaTime*this.maxSpeed;
        this.transform.Translate(m, Space.World);

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
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
        Vector3 directionVector = new Vector3(armDirection2D.x, 0.0f, armDirection2D.y);
        this.armTargetPosition = this.transform.position + directionVector ;
        //print(this.armTargetPosition);

        if (directionVector != Vector3.zero)
        {
            if (this.gameHandRB == null)
            {
                GameObject gameHandInstance = Instantiate(this.gameHandPrefab, this.transform.position, new Quaternion());
                this.gameHandRB = gameHandInstance.GetComponent<Rigidbody>();
                this.armRender.SetupArmRenderer(this.gameHandRB.transform, this.armHand.transform);
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

    private void FixedUpdate()
    {
        Vector2 velocity2D = (this.moveVector * this.maxSpeed * Time.fixedDeltaTime);
        Vector3 velocityVector = new Vector3(velocity2D.x, 0.0f, velocity2D.y);
        Vector3 newPosition = this.transform.position + velocityVector;        

        this.playerRB.MovePosition(newPosition);

        this.armHand.transform.position = this.armTargetPosition;
        

        if (this.gameHandRB != null)
        {
            print(true);
            this.gameHandRB.MovePosition(this.gameHandTargetPosition);

            if (Vector3.Distance(this.gameHandRB.position, this.armHand.transform.position) < 1.0f &&
                this.armDirection == Vector2.zero)
            {
                Destroy(this.gameHandRB.gameObject);
                this.gameHandRB = null;
            }
        }
    }

    private void OnEnable()
    {
        this.controls.PlayerMap.Enable();
    }
}
