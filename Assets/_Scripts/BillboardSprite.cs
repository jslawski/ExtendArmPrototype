using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
    private Transform targetObject;

    public bool isHand = false;

    private void Start()
    {
        this.targetObject = GameObject.Find("CameraFollow").transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 originalRotation = this.gameObject.transform.rotation.eulerAngles;
        if (FollowCam.currentMode == CameraMode.TopDown)
        {
            this.gameObject.transform.LookAt(Camera.main.transform, Vector3.up);            
        }
        else
        {
            this.gameObject.transform.LookAt(Camera.main.transform, Vector3.back);
        }

        if (this.isHand == true)
        {
            this.gameObject.transform.rotation = Quaternion.Euler(this.gameObject.transform.rotation.eulerAngles.x,
                                                                  this.gameObject.transform.rotation.eulerAngles.y,
                                                                  originalRotation.z);    
        }
    }
}
