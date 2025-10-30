using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Options_DontDestroyOnLoad : MonoBehaviour
{
    [SerializeField] private Toggle boobingToggle;
    private CameraScript cameraScript;
    private bool boobingbool = true;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        if(boobingToggle == null)
        {
            cameraScript = FindAnyObjectByType<CameraScript>().GetComponent<CameraScript>();

                cameraScript.bobbingEnabled = boobingbool;
                Destroy(this);

        }
    }

    public void onToggleChange()
    {
        boobingbool = boobingToggle.GetComponent<Toggle>().isOn;
    }
}
