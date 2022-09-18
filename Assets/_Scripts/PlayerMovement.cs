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

    void Awake()
    {
        /*
         BLAH BLAH BLAH BIG CHANGES 
        BLAH BLAH BLAH BIG CHANGES 
        BLAH BLAH BLAH BIG CHANGES 
        BLAH BLAH BLAH BIG CHANGES 
        BLAH BLAH BLAH BIG CHANGES 
        BLAH BLAH BLAH BIG CHANGES 
        BLAH BLAH BLAH BIG CHANGES 
        BLAH BLAH BLAH BIG CHANGES 
        BLAH BLAH BLAH BIG CHANGES 
        */
        this.controls = new PlayerControls();

        this.controls.PlayerMap.Move.performed += context => this.moveVector = context.ReadValue<Vector2>();
        this.controls.PlayerMap.Move.canceled += context => this.moveVector = Vector2.zero;

        this.playerRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.moveVector.magnitude < this.analogDeadZoneMagnitude)
        {
            this.moveVector = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        Vector2 velocity2D = (this.moveVector * this.maxSpeed * Time.fixedDeltaTime);
        Vector3 velocityVector = new Vector3(velocity2D.x, velocity2D.y, 0.0f);
        Vector3 newPosition = this.transform.position + velocityVector;        

        this.playerRB.MovePosition(newPosition);
    }

    private void OnEnable()
    {
        this.controls.PlayerMap.Enable();
    }
}
