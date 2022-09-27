using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MetaControls : MonoBehaviour
{
    [SerializeField]
    private GameObject instructionsObject;

    private PlayerControls controls;


    // Start is called before the first frame update
    void Awake()
    {
        this.controls = new PlayerControls();

        this.controls.PlayerMap.Start.performed += context => this.DisableInstructions();
        this.controls.PlayerMap.Restart.performed += context => this.Restart();
    }

    private void OnDestroy()
    {
        this.controls.Disable();
    }

    private void DisableInstructions()
    {
        if (this.instructionsObject != null)
        {
            this.instructionsObject.SetActive(false);
        }
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }

    private void OnEnable()
    {
        this.controls.PlayerMap.Enable();
    }
}
