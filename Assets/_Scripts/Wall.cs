using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer topRenderer;
    [SerializeField]
    private MeshRenderer sideRenderer1;
    [SerializeField]
    private MeshRenderer sideRenderer2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (FollowCam.currentMode == CameraMode.TopDown)
        {
            this.topRenderer.enabled = true;
            this.sideRenderer1.enabled = false;
            this.sideRenderer2.enabled = false;
        }
        else
        {
            this.topRenderer.enabled = false;
            this.sideRenderer1.enabled = true;
            this.sideRenderer2.enabled = true;
        }
    }
}
