using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandFollow : MonoBehaviour
{
    [SerializeField]
    private Rigidbody handRB;
    [SerializeField]
    private Transform originTransform;
    [SerializeField]
    private Transform targetTransform;

    [SerializeField, Range(0, 100)]
    float followSpeed = 0.0f;

    Vector3 targetPosition = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        this.targetPosition = Vector3.Lerp(this.handRB.position, 
                                           this.targetTransform.position, 
                                           this.followSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        this.handRB.MovePosition(this.targetPosition);
    }
}
