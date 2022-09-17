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
    private Transform handEndpoint;

    [SerializeField]
    private float lowerPointDistancePercentage = 0.25f;
    [SerializeField]
    private float upperPointDistancePercentage = 0.75f;

    [SerializeField]
    private float lowerPointDeviation = 5f;
    [SerializeField]
    private float upperPointDeviation = 2f;

    [SerializeField]
    private float lowerPointMoveSpeed = 5f;
    [SerializeField]
    private float upperPointMoveSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetupArmRenderer(Transform handTransform)
    {
        this.handEndpoint = handTransform;
        this.armLine.positionCount = 100;
    }

    private Vector3 CalculateCurvePoint(float tValue, Vector3 position0, Vector3 position1, Vector3 position2, Vector3 position3)
    {
        return (Mathf.Pow((1 - tValue), 3.0f) * position0) +
                (3 * Mathf.Pow((1 - tValue), 2.0f) * tValue * position1) +
                (3 * (1 - tValue) * Mathf.Pow(tValue, 2.0f) * position2) +
                (Mathf.Pow(tValue, 3.0f) * position3);
    }

    // Update is called once per frame
    void Update()
    {
        this.bezierPoints[0].position = this.playerEndpoint.position;

        if (this.handEndpoint == null)
        {
            this.bezierPoints[1].position = this.playerEndpoint.position;
            this.bezierPoints[2].position = this.playerEndpoint.position;
            this.bezierPoints[3].position = this.playerEndpoint.position;
            return;
        }

        this.bezierPoints[1].position = this.playerEndpoint.position + (this.lowerPointDistancePercentage * this.handEndpoint.position);
        this.bezierPoints[2].position = this.playerEndpoint.position + (this.upperPointDistancePercentage * this.handEndpoint.position);
        this.bezierPoints[3].position = this.handEndpoint.position;       
    }
}
