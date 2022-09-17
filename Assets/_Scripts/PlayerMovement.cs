using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Range(0, 100)]
    private float maxSpeed = 10f;

    private Rigidbody playerRB;

    PlayerControls controls;

    Vector2 moveVector = Vector2.zero;

    public float analogDeadZoneMagnitude = 0.3f;

    [SerializeField]
    private Rigidbody armPivot;

    private Vector2 armDirection = Vector2.zero;
    private Vector2 prevArmDirection = Vector2.zero;
    private float targetAngle;

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
        if (this.moveVector.magnitude < this.analogDeadZoneMagnitude)
        {
            this.moveVector = Vector2.zero;
        }

        if (this.armDirection.magnitude < this.analogDeadZoneMagnitude)
        {
            this.armDirection = this.prevArmDirection;
        }

        this.targetAngle = Mathf.Atan2(this.armDirection.y, this.armDirection.x) * Mathf.Rad2Deg;
        this.prevArmDirection = this.armDirection;
    }

    private void FixedUpdate()
    {
        Vector2 velocity2D = (this.moveVector * this.maxSpeed * Time.fixedDeltaTime);
        Vector3 velocityVector = new Vector3(velocity2D.x, velocity2D.y, 0.0f);
        Vector3 newPosition = this.transform.position + velocityVector;        

        this.playerRB.MovePosition(newPosition);

        this.armPivot.MovePosition(newPosition);
        this.armPivot.MoveRotation(Quaternion.Euler(0.0f, 0.0f, this.targetAngle));

    }

    private void OnEnable()
    {
        this.controls.PlayerMap.Enable();
    }
}
