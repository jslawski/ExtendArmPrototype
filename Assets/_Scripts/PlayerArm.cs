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

    private Vector3 targetPosition = Vector3.zero;

    [SerializeField, Range(0, 1000)]
    public float maxArmForce = 10f;

    [SerializeField]
    private Rigidbody armRB;
    [SerializeField]
    private GameObject armPivot;

    [SerializeField, Range(0, 1000)]
    private float springForce = 0.0f;

    [SerializeField, Range(0, 100)]
    private float minExtendSpeed = 5.0f;
    [SerializeField, Range(0, 1000)]
    private float maxExtendSpeed = 50.0f;

    [SerializeField]
    private bool slerpFlag = false;

    void Awake()
    {
        this.controls = new PlayerControls();

        this.controls.PlayerMap.Arm.performed += context => this.armDirection = context.ReadValue<Vector2>();
        this.controls.PlayerMap.Arm.canceled += context => this.armDirection = Vector2.zero;
    }

    private float GetDistanceRatio(Vector3 origin, Vector3 current, Vector3 target)
    {
        return (Vector3.Distance(origin, current) / Vector3.Distance(origin, target));
    }

    // Update is called once per frame
    void Update()
    {                
        if (this.armDirection.magnitude < this.analogDeadZoneMagnitude)
        {
            this.armDirection = Vector2.zero;
        }

        

        Vector3 directionVector = new Vector3(this.armDirection.x, this.armDirection.y, 0.0f);

        Vector3 originPoint = this.playerParent.transform.position + directionVector.normalized * this.analogDeadZoneMagnitude;

        Vector3 finalPosition =  originPoint + (directionVector.normalized * 
            (this.armDirection.magnitude * this.maxArmDistance));

        Vector3 maxPosition = originPoint + (directionVector.normalized * this.maxArmDistance);

        float currentSpeed = Mathf.Lerp(this.minExtendSpeed, this.maxExtendSpeed,
            this.GetDistanceRatio(originPoint, this.armRB.position, maxPosition));

        Debug.LogError("Current Speed: " + currentSpeed);

        if (this.armRB.position != finalPosition)
        {
            this.targetPosition = Vector3.Lerp(this.armRB.position, finalPosition, currentSpeed * Time.deltaTime);
        }

        //Slerp for first half, Lerp for second half
        /*if (this.GetDistanceRatio(originPoint, this.armRB.position, finalPosition) < 0.5f)
        {

        if (this.slerpFlag == true)
        {
            this.targetPosition = Vector3.Slerp(this.armRB.position, finalPosition, this.armExtendSpeed * Time.deltaTime);
        }
        else
        {
            this.targetPosition = Vector3.Lerp(this.armRB.position, finalPosition, this.armExtendSpeed * Time.deltaTime);
        }
        //}

        //this.targetPosition = ()
        */
    }

    private void FixedUpdate()
    {
        float armForce = this.armDirection.magnitude * this.maxArmForce;

        //Debug.LogError(this.armDirection.magnitude);
        /*
        this.armRB.AddForce(this.armPivot.transform.right * armForce);

        if (Vector3.Distance(this.armPivot.transform.position, this.armRB.position) > this.maxArmDistance)
        {
            Debug.LogError("Applying Spring Force");
            this.armRB.AddForce(-this.armPivot.transform.right * this.springForce);
        }
        */
        this.armRB.MovePosition(this.targetPosition);
    }

    private void OnEnable()
    {
        this.controls.PlayerMap.Enable();
    }
}
