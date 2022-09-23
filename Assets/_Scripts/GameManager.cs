using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public enum ThrowMode { Delay, Anticipate }

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> grabbableObjectPrefabs;

    private GameObject currentSmallGrabbableObject;
    private GameObject currentMedGrabbableObject;
    private GameObject currentBigGrabbableObject;

    public static ThrowMode throwingMode = ThrowMode.Anticipate;

    [SerializeField]
    private TextMeshProUGUI throwModeLabel;

    private void Update()
    {
        /*
        if (this.currentSmallGrabbableObject == null)
        {
            this.currentSmallGrabbableObject = Instantiate(this.grabbableObjectPrefabs[0], 
                new Vector3(-5.0f, 0.0f, 0.0f), new Quaternion());
        }

        if (this.currentMedGrabbableObject == null)
        {
            this.currentMedGrabbableObject = Instantiate(this.grabbableObjectPrefabs[1],
                new Vector3(0.0f, 3.0f, 0.0f), new Quaternion());
        }

        if (this.currentBigGrabbableObject == null)
        {
            this.currentBigGrabbableObject = Instantiate(this.grabbableObjectPrefabs[2],
                new Vector3(5.0f, 0.0f, 0.0f), new Quaternion());
        }*/
    }

    public void ChangeThrowMode()
    {
        switch (throwingMode)
        {
            case ThrowMode.Delay:
                throwingMode = ThrowMode.Anticipate;
                this.throwModeLabel.text = "Mode B Active";
                break;
            case ThrowMode.Anticipate:
                throwingMode = ThrowMode.Delay;
                this.throwModeLabel.text = "Mode A Active";
                break;

        }
    }
}
