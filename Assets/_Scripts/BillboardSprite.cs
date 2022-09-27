using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
    public bool isHand = false;
    public bool isPlayer = false;

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 originalRotation = this.gameObject.transform.rotation.eulerAngles;

        this.gameObject.transform.forward = -Camera.main.transform.forward;
        
        if (FollowCam.currentMode == CameraMode.Isometric)
        {
            if (this.isPlayer == true)
            {
                this.gameObject.transform.rotation = Quaternion.Euler(this.gameObject.transform.rotation.eulerAngles.x,
                                                                  this.gameObject.transform.rotation.eulerAngles.y,
                                                                  this.gameObject.transform.rotation.eulerAngles.z + 15);
            }
            else 
            {
                this.gameObject.transform.rotation = Quaternion.Euler(this.gameObject.transform.rotation.eulerAngles.x,
                                                                      this.gameObject.transform.rotation.eulerAngles.y,
                                                                      this.gameObject.transform.rotation.eulerAngles.z + 20);
            }
        }        

        if (this.isHand == true)
        {
            this.gameObject.transform.rotation = Quaternion.Euler(this.gameObject.transform.rotation.eulerAngles.x,
                                                                  this.gameObject.transform.rotation.eulerAngles.y,
                                                                  originalRotation.z);    
        }
    }
}
