using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArm : MonoBehaviour
{
    [SerializeField, Range(0, 100)]
    private float maxArmDistance = 10f;

    public GameObject playerParent;

    private PlayerControls controls;

    private Vector2 armDirection = Vector2.zero;

    public float analogDeadZoneMagnitude = 0.3f;

    private Rigidbody armRB;


    [SerializeField, Range(0, 1000)]
    public float armForce = 10f;
    
    [SerializeField]
    private Rigidbody armPivot;

    private float targetAngle;

    void Awake()
    {
        this.controls = new PlayerControls();

        this.controls.PlayerMap.Arm.performed += context => this.armDirection = context.ReadValue<Vector2>();
        this.controls.PlayerMap.Arm.canceled += context => this.armDirection = Vector2.zero;

        this.armRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {                
        if (this.armDirection.magnitude < this.analogDeadZoneMagnitude)
        {
            this.armDirection = Vector2.zero;
        }

        Vector2 armDirection2D = (this.armDirection * this.maxArmDistance);
        Vector3 directionVector = new Vector3(armDirection2D.x, armDirection2D.y, 0.0f);

        this.targetAngle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;

        //this.targetPosition = this.playerParent.transform.position + directionVector;
    }

    private void FixedUpdate()
    {
        this.armPivot.MoveRotation(Quaternion.Euler(0.0f, 0.0f, this.targetAngle));
        
        this.armRB.AddForce(this.armPivot.transform.right * this.armForce);
        //this.armRB.MovePosition(this.targetPosition);
    }

    private void OnEnable()
    {
        this.controls.PlayerMap.Enable();
    }
}
