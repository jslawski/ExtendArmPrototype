using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneToggle : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        FollowCam.OnSpriteModeChanged += this.ToggleTexture;
        this.meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnDestroy()
    {
        FollowCam.OnSpriteModeChanged -= this.ToggleTexture;
    }

    private void ToggleTexture()
    {
        if (FollowCam.spritesEnabled == true)
        {
            this.meshRenderer.material = Resources.Load<Material>("Materials/Plane");
        }
        else
        {
            this.meshRenderer.material = Resources.Load<Material>("Materials/DebugPlane");
        }
    }
}
