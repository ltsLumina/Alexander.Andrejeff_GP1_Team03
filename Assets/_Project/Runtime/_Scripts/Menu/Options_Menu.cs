using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Options_Menu : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    
    [Header("Settings")]
    [SerializeField] private Slider mySliderValueSound;
    [SerializeField] private Slider mySliderValueBrightness;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Image brightnessFilter;
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(mySliderValueSound.gameObject);
        mySliderValueSound.value = PlayerPrefs.GetFloat("Volume");
        mySliderValueBrightness.value = PlayerPrefs.GetFloat("Brightness");
        onMasterChanged();
        onBrightnessChanged();
    }
    public void BackButton()
    {
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void onMasterChanged()
    {
        float sliderValue = mySliderValueSound.value;
        if (sliderValue == 0) mixer.SetFloat("MasterVolume", -80);
        mixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("Volume", sliderValue);
    }

    public void onBrightnessChanged()
    {
        float sliderValue = mySliderValueBrightness.value;
        brightnessFilter.color = new Color(0, 0, 0, 1 - sliderValue / 22);
        PlayerPrefs.SetFloat("Brightness", sliderValue);
    }
}