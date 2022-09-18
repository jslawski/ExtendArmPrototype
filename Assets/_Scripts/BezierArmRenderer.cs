using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierArmRenderer : MonoBehaviour
{
    [SerializeField]
    private LineRenderer armLine;

    [SerializeField]
    private Transform[] bezierPoints;

    [SerializeField]
    private Transform playerEndpoint;
    [SerializeField]
    private Transform gameHandEndpoint;

    private Transform metaHandTransform;

    [SerializeField, Range(0, 1)]
    private float lowerPointDistancePercentage = 0.25f;
    [SerializeField, Range(0, 1)]
    private float upperPointDistancePercentage = 0.75f;

    [SerializeField, Range(0, 1)]
    private float lowerPointDeviation = 1.0f;
    [SerializeField, Range(0, 1)]
    private float upperPointDeviation = 0.5f;

    public void SetupArmRenderer(Transform gameHand, Transform metaHand)
    {
        this.gameHandEndpoint = gameHand;
        this.metaHandTransform = metaHand;
        this.armLine.positionCount = 100;
    }

    private Vector3 CalculateCurvePoint(float tValue, Vector3 position0, Vector3 position1, Vector3 position2, Vector3 position3)
    {
        return (Mathf.Pow((1 - tValue), 3.0f) * position0) +
                (3 * Mathf.Pow((1 - tValue), 2.0f) * tValue * position1) +
                (3 * (1 - tValue) * Mathf.Pow(tValue, 2.0f) * position2) +
                (Mathf.Pow(tValue, 3.0f) * position3);
    }

    private void RenderArm()
    {
        float tValue = 0.0f;
        float tIncrement = (1.0f / (float)this.armLine.positionCount);

        for (int i = 0; i < this.armLine.positionCount; i++)
        {
            this.armLine.SetPosition(i, this.CalculateCurvePoint(tValue, 
                                        this.bezierPoints[0].position, 
                                        this.bezierPoints[1].position,
                                        this.bezierPoints[2].position,
                                        this.bezierPoints[3].position));

            tValue += tIncrement;
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.bezierPoints[0].position = this.playerEndpoint.position;

        if (this.gameHandEndpoint == null)
        {
            this.bezierPoints[1].position = this.playerEndpoint.position;
            this.bezierPoints[2].position = this.playerEndpoint.position;
            this.bezierPoints[3].position = this.playerEndpoint.position;

            this.armLine.positionCount = 0;
            return;
        }

        Vector3 originalLowerPointPosition = Vector3.Lerp(this.playerEndpoint.position, this.gameHandEndpoint.position, this.lowerPointDistancePercentage);
        Vector3 originalUpperPointPosition = Vector3.Lerp(this.playerEndpoint.position, this.gameHandEndpoint.position, this.upperPointDistancePercentage);

        this.bezierPoints[1].position = originalLowerPointPosition;
        this.bezierPoints[2].position = originalUpperPointPosition;

        this.bezierPoints[3].position = this.gameHandEndpoint.position;
        
        Vector3 lowerPointTargetDirection = Vector3.Slerp(this.gameHandEndpoint.position, 
                                            this.metaHandTransform.position, 
                                            this.lowerPointDeviation);

        Vector3 upperPointTargetDirection = Vector3.Slerp(this.gameHandEndpoint.position,
                                            this.metaHandTransform.position,
                                            this.upperPointDeviation);

        this.bezierPoints[1].position = Vector3.Lerp(this.playerEndpoint.position, lowerPointTargetDirection, this.lowerPointDistancePercentage);
        this.bezierPoints[2].position = Vector3.Lerp(this.playerEndpoint.position, upperPointTargetDirection, this.upperPointDistancePercentage);

        //Vector3 targetLowerPointVector = lowerPointTargetDirection * originalLowerPointPosition.magnitude;
        //Vector3 targetUpperPointVector = upperPointTargetDirection * originalUpperPointPosition.magnitude;

        //this.bezierPoints[1].position = targetLowerPointVector - this.playerEndpoint.position;
        //this.bezierPoints[2].position = targetUpperPointVector - this.playerEndpoint.position;
        
        this.RenderArm();
    }
}
