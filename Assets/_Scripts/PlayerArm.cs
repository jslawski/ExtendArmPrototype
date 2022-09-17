using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArm : MonoBehaviour
{
    public GameObject playerParent;

    private PlayerControls controls;

    private Vector2 armDirection = Vector2.zero;

    public float analogDeadZoneMagnitude = 0.3f;

    [SerializeField, Range(0, 1000)]
    public float maxArmForce = 10f;

    [SerializeField]
    private Rigidbody armRB;
    [SerializeField]
    private Rigidbody armPivot;

    void Awake()
    {
        this.controls = new PlayerControls();

        this.controls.PlayerMap.Arm.performed += context => this.armDirection = context.ReadValue<Vector2>();
        this.controls.PlayerMap.Arm.canceled += context => this.armDirection = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {                
        if (this.armDirection.magnitude < this.analogDeadZoneMagnitude)
        {
            this.armDirection = Vector2.zero;
        }

        //this.targetPosition = this.playerParent.transform.position + directionVector;
    }

    private void FixedUpdate()
    {
        float armForce = this.armDirection.magnitude * this.maxArmForce;

        Debug.LogError(this.armDirection.magnitude);

        this.armRB.AddForce(this.armPivot.transform.right * armForce);
        //this.armRB.MovePosition(this.targetPosition);
    }

    private void OnEnable()
    {
        this.controls.PlayerMap.Enable();
    }
}
