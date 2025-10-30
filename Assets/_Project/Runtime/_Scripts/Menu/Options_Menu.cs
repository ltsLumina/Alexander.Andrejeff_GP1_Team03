using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Options_Menu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject firstObject;

    [SerializeField] private GameObject boobingToggle;

    [SerializeField] private AudioMixer mixer;
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(firstObject);
    }
    public void BackButton()
    {
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void onMasterChanged(float i)
    {
        mixer.SetFloat("masterVolume", (i -1) * 80);
    }


    public void onMusicChanged(float i)
    {
        mixer.SetFloat("musicVolume", (i -1) * 80);
    }


    public void onEffectsChanged(float i)
    {
        mixer.SetFloat("effectsVolume", (i - 1) * 80);
    }
}