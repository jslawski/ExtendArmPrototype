using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCollider : MonoBehaviour
{
    private MeshRenderer mesh;

    public bool showDebugMesh = false;

    // Start is called before the first frame update
    void Start()
    {
        this.mesh = GetComponent<MeshRenderer>();     
    }

    private void Update()
    {
        if (this.showDebugMesh == true)
        {
            this.mesh.enabled = true;
        }
        else
        {
            this.mesh.enabled = false;
        }
    }
}
