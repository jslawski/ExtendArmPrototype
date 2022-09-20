using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandControls : MonoBehaviour
{
    private PlayerControls controls;

    [SerializeField]
    private Rigidbody handRb;
    [SerializeField]
    private SpriteRenderer handSpriteRenderer;
    [SerializeField]
    private Sprite openHandSprite;
    [SerializeField]
    private Sprite grabHandSprite;
    [SerializeField]
    private BoxCollider handCollider;

    private GrabbableObject grabbedObject;

    [SerializeField]
    private LayerMask detectableLayer;

    private void Awake()
    {
        this.controls = new PlayerControls();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.controls.PlayerMap.Grab.performed += context => this.InitiateGrab();
        this.controls.PlayerMap.Grab.canceled += context => this.CancelGrab();

    }

    private void OnDestroy()
    {
        this.controls.PlayerMap.Grab.performed -= context => this.InitiateGrab();
        this.controls.PlayerMap.Grab.canceled -= context => this.CancelGrab();
        this.controls.Disable();
    }

    private void InitiateGrab()
    {
        RaycastHit hitInfo;

        Vector3 spherecastOrigin = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, -10f);

        if (Physics.SphereCast(spherecastOrigin, 1.0f, Vector3.forward, out hitInfo, Mathf.Infinity, this.detectableLayer))
        {
            this.grabbedObject = hitInfo.rigidbody.gameObject.GetComponent<GrabbableObject>();
            this.grabbedObject.GetGrabbed();
        }
        
        this.handSpriteRenderer.sprite = this.grabHandSprite;

        this.handCollider.enabled = true;
    }

    private void CancelGrab()
    {
        this.handSpriteRenderer.sprite = this.openHandSprite;
        this.handCollider.enabled = false;

        if (this.grabbedObject != null)
        {
            this.grabbedObject.GetReleased(this.handRb.velocity);
        }

        this.grabbedObject = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (this.grabbedObject != null)
        {
            this.grabbedObject.objectRb.MovePosition(this.gameObject.transform.position);
        }
    }

    private void OnEnable()
    {
        this.controls.PlayerMap.Enable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "GrabbableObject")
        {
            //Do Something
        }
    }
}
