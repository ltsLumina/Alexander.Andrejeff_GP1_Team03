using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Options_Menu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject firstObject;
    [SerializeField] private Slider mySliderValueSound;
    [SerializeField] private Slider mySliderValueBrightness;
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

    public void onMasterChanged()
    {
        float sliderValue = mySliderValueSound.value;
        mixer.SetFloat("masterVolume", (sliderValue - 1) * 80);
        PlayerPrefs.SetFloat("Volume", sliderValue);
    }

        public void onBrightnessChanged()
    {
        float sliderValue = mySliderValueBrightness.value;
       ////////////////////////////////////////////////
        PlayerPrefs.SetFloat("Brightness", sliderValue);
    }
}