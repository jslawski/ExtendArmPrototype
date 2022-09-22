using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    public bool grabbed = false;

    [HideInInspector]
    public Rigidbody objectRb;

    public float maxSpeed;

    private int grabbedLayer;
    private int grabbableLayer;

    public bool isStationary = false;

    // Start is called before the first frame update
    void Start()
    {
        this.objectRb = GetComponent<Rigidbody>();
        this.grabbedLayer = LayerMask.NameToLayer("Grabbed");
        this.grabbableLayer = LayerMask.NameToLayer("Grabbable");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(this.gameObject.transform.position);

        if (this.objectRb.velocity.magnitude <= 1 && this.grabbed == false)
        {
            if (viewportPos.x <= 0.0f || viewportPos.x > 1.0f)
            {
                Destroy(this.gameObject);
            }
            if (viewportPos.y <= 0.0f || viewportPos.y > 1.0f)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void GetGrabbed()
    {
        this.grabbed = true;
        this.gameObject.layer = this.grabbedLayer;
    }

    public void GetReleased(Vector2 releaseDirection, float releaseSpeed)
    {
        this.grabbed = false;

        if (this.isStationary == false)
        {
            this.objectRb.velocity = new Vector3(releaseDirection.x, releaseDirection.y, 0.0f).normalized * releaseSpeed;

            if (this.objectRb.velocity.magnitude > this.maxSpeed)
            {
                this.objectRb.velocity = releaseDirection.normalized * this.maxSpeed;
            }

            Invoke("LayerReset", 0.2f);
        }
        else
        {
            this.LayerReset();
        }
    }

    private void LayerReset()
    {
        this.gameObject.layer = this.grabbableLayer;
    }
}
