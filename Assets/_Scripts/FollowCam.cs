using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraMode { TopDown, Isometric }

public class FollowCam : MonoBehaviour
{
    [SerializeField]
    private Transform cameraHolderTransform;
    [SerializeField]
    private Transform cameraTransform;
    
    [SerializeField]
    private Transform target = null;

    private Vector3 offset;

    private PlayerControls controls;

    public float topDownCameraZDistance = 20f;
    public float isometricCameraZDistance = 10f;

    public Vector3 isometricPositionOffset;

    public Vector3 isometricCameraHolderRotation;
    public Vector3 isometricCameraRotation;

    
    public static CameraMode currentMode = CameraMode.TopDown;

    public static bool spritesEnabled = true;

    [SerializeField, Range(0f, 10f)]
    private float cameraMoveSpeed = 3.0f;

    public delegate void SpriteModeChanged();
    public static SpriteModeChanged OnSpriteModeChanged;

    // Start is called before the first frame update
    void Awake()
    {
        offset = transform.position - target.position;

        this.cameraHolderTransform.position = new Vector3(this.target.position.x, this.target.position.y, -this.topDownCameraZDistance);
        this.cameraHolderTransform.rotation = Quaternion.Euler(Vector3.zero);
        this.cameraTransform.rotation = Quaternion.Euler(Vector3.zero);

        FollowCam.currentMode = CameraMode.TopDown;

        this.controls = new PlayerControls();
        
        this.controls.PlayerMap.ToggleCamera.performed += context => this.ToggleCamera();
    }

    private void OnDestroy()
    {
        this.controls.Disable();
    }

    private void ToggleCamera()
    {
        if (this.cameraHolderTransform == null)
        {
            return;
        }
        
        if (FollowCam.currentMode == CameraMode.TopDown)
        {
            //Switch to isometric
            this.cameraHolderTransform.position = new Vector3(this.target.position.x + this.isometricPositionOffset.x,
                                                                this.target.position.y + this.isometricPositionOffset.y,
                                                                 -this.isometricCameraZDistance);
            this.cameraHolderTransform.rotation = Quaternion.Euler(this.isometricCameraHolderRotation);
            this.cameraTransform.localRotation = Quaternion.Euler(this.isometricCameraRotation);

            FollowCam.currentMode = CameraMode.Isometric;
        }
        else
        {
            //Switch to top-down
            this.cameraHolderTransform.position = new Vector3(this.target.position.x, this.target.position.y, -this.topDownCameraZDistance);
            this.cameraHolderTransform.rotation = Quaternion.Euler(Vector3.zero);
            this.cameraTransform.rotation = Quaternion.Euler(Vector3.zero);

            FollowCam.currentMode = CameraMode.TopDown;
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, 
            new Vector3(target.position.x, target.position.y, target.position.z) + offset, 
            (this.cameraMoveSpeed * Time.deltaTime));

        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            FollowCam.spritesEnabled = !FollowCam.spritesEnabled;
            
            if (FollowCam.OnSpriteModeChanged != null)
            {
                FollowCam.OnSpriteModeChanged();
            }
        }

    }

    private void OnEnable()
    {
        this.controls.Enable();
    }
}
