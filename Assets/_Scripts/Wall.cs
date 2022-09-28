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
        FollowCam.OnSpriteModeChanged += this.ToggleMaterials;
    }

    private void ToggleMaterials()
    {
        if (FollowCam.spritesEnabled == true)
        {
            this.topRenderer.material = Resources.Load<Material>("Materials/TopMaterial");
            this.sideRenderer1.material = Resources.Load<Material>("Materials/SideMaterial");
            this.sideRenderer2.material = Resources.Load<Material>("Materials/SideMaterial");
        }
        else
        {
            this.topRenderer.material = Resources.Load<Material>("Materials/DebugTop");
            this.sideRenderer1.material = Resources.Load<Material>("Materials/DebugSide");
            this.sideRenderer2.material = Resources.Load<Material>("Materials/DebugSide");
        }
    }

    private void OnDestroy()
    {
        FollowCam.OnSpriteModeChanged -= this.ToggleMaterials;
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
