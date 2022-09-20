using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> grabbableObjectPrefabs;

    private GameObject currentSmallGrabbableObject;
    private GameObject currentMedGrabbableObject;
    private GameObject currentBigGrabbableObject;

    private void Update()
    {
        if (this.currentSmallGrabbableObject == null)
        {
            this.currentSmallGrabbableObject = Instantiate(this.grabbableObjectPrefabs[0], 
                new Vector3(-5.0f, 3.0f, 0.0f), new Quaternion());
        }

        if (this.currentMedGrabbableObject == null)
        {
            this.currentMedGrabbableObject = Instantiate(this.grabbableObjectPrefabs[1],
                new Vector3(0.0f, 3.0f, 0.0f), new Quaternion());
        }

        if (this.currentBigGrabbableObject == null)
        {
            this.currentBigGrabbableObject = Instantiate(this.grabbableObjectPrefabs[2],
                new Vector3(5.0f, 3.0f, 0.0f), new Quaternion());
        }
    }    
}
