using System.Collections;
using Abiogenesis3d;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause_Menu : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject AccessibilityMenu;
    [SerializeField] private GameObject victoryMenu;

    [Header("PauseMenuComponents")]
    [SerializeField] private GameObject restartingText;
    [SerializeField] private GameObject continueButton;

    [Header("OptionsMenuComponents")]
    [SerializeField] private Slider mySliderValueSound;
    [SerializeField] private Slider mySliderValueBrightness;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Image brightnessFilter;

    [Header("AccessibilityComponents")]
    [SerializeField] private Slider colorBlindlessSlider;
    [SerializeField] private Slider pixelFilterSlider;
    [SerializeField] private Toggle bobToggle;
    [SerializeField] private Toggle screenShakeToggle;
    [SerializeField] private TMP_Text ColorBlindlessText;
    [SerializeField] private TMP_Text PixelFilterText;
    [SerializeField] private Material ColorBlindMat;

    [Header("VictoryComponents")]
    [SerializeField] private TMP_Text victoryText;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private GameObject mainMenuButton;

    [SerializeField] string[] victoryTexts = { "Victory is yours!" };



    [Header("Scripts")]
    [SerializeField] private UPixelator uPixelator;
    [SerializeField] private GameManager gm;
    [SerializeField] private CameraScript cameraScript;

    private bool restartingProgress = false;
    private Color colour = new Color(1, 1, 1, 0);


    void Awake()
    {
        mySliderValueSound.value = PlayerPrefs.GetFloat("Volume");
        mySliderValueBrightness.value = PlayerPrefs.GetFloat("Brightness");
        onMasterChanged();
        onBrightnessChanged();
                colorBlindlessSlider.value = PlayerPrefs.GetFloat("ColorBlindlessValue");
        ColorBlindlessValueChanged();
        pixelFilterSlider.value = PlayerPrefs.GetInt("PixelFilterValue");
        onPixelFilterChanged();

        if (PlayerPrefs.GetInt("BobSetting") == 1)
            bobToggle.isOn = true;
        else
            bobToggle.isOn = false;

        onBobToggleChanged();

        if (PlayerPrefs.GetInt("ScreenShakeSetting") == 1)
            screenShakeToggle.isOn = true;
        else
            screenShakeToggle.isOn = false;

        onScreenShakeToggleChanged();

        PlayerPrefs.SetString("ApplicationRunned", "first");
    }

    void Update()
    {

    }

    public void VictoryMenuTriggered()
    {
        int randomInt = Random.Range(0, victoryTexts.Length);
        EventSystem.current.SetSelectedGameObject(mainMenuButton);
        Time.timeScale = 0;
        victoryMenu.SetActive(true);
        victoryText.text = victoryTexts[randomInt];
        coinsText.text = $"Points : {gm.playerCoins}";
    }
    public void PauseMenu()
    {
        if (Time.timeScale != 0)
        {
            pauseMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(continueButton);
            Time.timeScale = 0;
        }

    }
    public void ContinueButton()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void OptionsButton()
    {
        EventSystem.current.SetSelectedGameObject(mySliderValueSound.gameObject);
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void AccessibilityButton()
    {
        EventSystem.current.SetSelectedGameObject(colorBlindlessSlider.gameObject);
        pauseMenu.SetActive(false);
        AccessibilityMenu.SetActive(true);
    }

    public void RestartDeselect()
    {
            restartingText.GetComponent<TMP_Text>().color = new Color(1, 1, 1, 0);
            colour = new Color(1, 1, 1, 0);
            restartingProgress = false;
            StopAllCoroutines();
    }
    public void RestartButton()
    {
        if (Input.anyKey)
        {
  
            if (!restartingProgress)
                StartCoroutine(Restarting());
        }
        else
        {
            StopAllCoroutines();
            restartingText.GetComponent<TMP_Text>().color = new Color(1, 1, 1, 0);
            colour = new Color(1, 1, 1, 0);
            restartingProgress = false;
        }
    }
    IEnumerator Restarting()
    {
        restartingProgress = true;
        yield return new WaitForSecondsRealtime(0.2f);
        for (int i = 0; i < 180; i++)
        {
            Debug.Log(colour);
            colour = new Color(1, 1, 1, colour.a + 1f / 180f);
            restartingText.GetComponent<TMP_Text>().color = colour;
            yield return new WaitForSecondsRealtime(0.01f);
        }
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void onMenuButton()
    {
        SceneManager.LoadScene(0);
    }

    public void onMasterChanged()
    {
        float sliderValue = mySliderValueSound.value;
        mixer.SetFloat("Master", (sliderValue - 1) * 20);
        PlayerPrefs.SetFloat("Volume", sliderValue);
    }

    public void onBrightnessChanged()
    {
        float sliderValue = mySliderValueBrightness.value;
        brightnessFilter.color = new Color(0, 0, 0, 1 - sliderValue / 22);
        PlayerPrefs.SetFloat("Brightness", sliderValue);
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
        uPixelator.pixelMultiplier = PlayerPrefs.GetInt("PixelFilterValue", 2);
    }
    
        public void onBobToggleChanged()
    {
        if (bobToggle.isOn)
            PlayerPrefs.SetInt("BobSetting", 1);
        else
            PlayerPrefs.SetInt("BobSetting", 0);

        cameraScript.BobbingEnabled = bobToggle.isOn;
    }
    
    public void onScreenShakeToggleChanged()
    {
        if (screenShakeToggle.isOn)
            PlayerPrefs.SetInt("ScreenShakeSetting", 1);
        else
            PlayerPrefs.SetInt("ScreenShakeSetting", 0);

        cameraScript.ViewShakeEnabled = screenShakeToggle.isOn;
    }

    public void BackButton(GameObject currentmenu)
    {
        currentmenu.SetActive(false);
        pauseMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(continueButton);
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void RestartForVictory()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    private static readonly Color[,] colors = new Color[,]

{

        { new Color(1, 0, 0), new Color(0, 1, 0), new Color(0, 0, 1) },

        { new Color(0.567f, 0.433f, 0), new Color(0.558f, 0.442f, 0), new Color(0, 0.242f, 0.758f) },

        { new Color(0.625f, 0.375f, 0), new Color(0.700f, 0.300f, 0), new Color(0, 0.300f, 0.700f) },

        { new Color(0.950f, 0.050f, 0), new Color(0, 0.433f, 0.567f), new Color(0, 0.475f, 0.525f) }

};
}
