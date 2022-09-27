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
