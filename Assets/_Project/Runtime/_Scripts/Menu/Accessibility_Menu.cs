using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Accessibility_Menu : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject accessibilityMenu;
    [SerializeField] private GameObject mainMenu;

    [Header("Settings")]
    [SerializeField] private Slider colorBlindlessSlider;
    [SerializeField] private Slider pixelFilterSlider;
    [SerializeField] private Toggle bobToggle;
    [SerializeField] private Toggle ScreenShakeToggle;
    [SerializeField] private Toggle OneHandedToggle;

    [Header("Texts")]
    [SerializeField] private TMP_Text ColorBlindlessText;
    [SerializeField] private TMP_Text PixelFilterText;

    [Header("Other")]
    [SerializeField] private Material ColorBlindMat;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(colorBlindlessSlider.gameObject);
        colorBlindlessSlider.value = PlayerPrefs.GetFloat("ColorBlindlessValue");
        ColorBlindlessValueChanged();
        pixelFilterSlider.value = PlayerPrefs.GetInt("PixelFilterValue");
        onPixelFilterChanged();

        bobToggle.isOn = PlayerPrefs.GetInt("BobSetting") == 1;

        onBobToggleChanged();

        ScreenShakeToggle.isOn = PlayerPrefs.GetInt("ScreenShakeSetting") == 1;

        onScreenShakeToggleChanged();
    }

    private static readonly Color[,] colors = {

        { new Color(1, 0, 0), new Color(0, 1, 0), new Color(0, 0, 1) },

        { new Color(0.567f, 0.433f, 0), new Color(0.558f, 0.442f, 0), new Color(0, 0.242f, 0.758f) },

        { new Color(0.625f, 0.375f, 0), new Color(0.700f, 0.300f, 0), new Color(0, 0.300f, 0.700f) },

        { new Color(0.950f, 0.050f, 0), new Color(0, 0.433f, 0.567f), new Color(0, 0.475f, 0.525f) }

    };
    
    public void BackButton()
    {
        accessibilityMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void ColorBlindlessValueChanged()
    {
        switch (colorBlindlessSlider.value)
        {
            case 3:
                ColorBlindMat.SetFloat("_Opacity", 0.7f);
                ColorBlindMat.SetColor("_R", colors[3, 0]);
                ColorBlindMat.SetColor("_G", colors[3, 1]);
                ColorBlindMat.SetColor("_B", colors[3, 2]);
                ColorBlindlessText.text = "Tritanopia";
                break;
            case 2:
                ColorBlindMat.SetFloat("_Opacity", 0.7f);
                ColorBlindMat.SetColor("_R", colors[2, 0]);
                ColorBlindMat.SetColor("_G", colors[2, 1]);
                ColorBlindMat.SetColor("_B", colors[2, 2]);
                ColorBlindlessText.text = "Deuteranopia";
                break;
            case 1:
                ColorBlindMat.SetFloat("_Opacity", 0.7f);
                ColorBlindMat.SetColor("_R", colors[1, 0]);
                ColorBlindMat.SetColor("_G", colors[1, 1]);
                ColorBlindMat.SetColor("_B", colors[1, 2]);
                ColorBlindlessText.text = "Protanopia";
                break;
            case 0:
                ColorBlindMat.SetInt("_Opacity", 0);
                ColorBlindlessText.text = "Default";
                break;
            default:
                ColorBlindMat.SetInt("_Opacity", 0);
                ColorBlindlessText.text = "Default";
                break;
        }
        PlayerPrefs.SetFloat("ColorBlindlessValue", colorBlindlessSlider.value);
    }

    public void onPixelFilterChanged()
    {
        switch (pixelFilterSlider.value)
        {
            case 2:
                PlayerPrefs.SetInt("PixelFilterValue", (int)pixelFilterSlider.value);
                PixelFilterText.text = "Lowest";
                break;
            case 3:
                PlayerPrefs.SetInt("PixelFilterValue", (int)pixelFilterSlider.value);
                PixelFilterText.text = "Low";
                break;
            case 4:
                PlayerPrefs.SetInt("PixelFilterValue", (int)pixelFilterSlider.value);
                PixelFilterText.text = "Medium";
                break;
            case 5:
                PlayerPrefs.SetInt("PixelFilterValue", (int)pixelFilterSlider.value);
                PixelFilterText.text = "High";
                break;
            default:
                PlayerPrefs.SetInt("PixelFilterValue", (int)pixelFilterSlider.value);
                break;
        }
    }

    public void onBobToggleChanged() => PlayerPrefs.SetInt("BobSetting", bobToggle.isOn ? 1 : 0);

    public void onScreenShakeToggleChanged() => PlayerPrefs.SetInt("ScreenShakeSetting", ScreenShakeToggle.isOn ? 1 : 0);

    public void onOneHandedToggleChanged(bool isOn) => PlayerPrefs.SetInt("OneHandedSetting", isOn ? 1 : 0);
}
