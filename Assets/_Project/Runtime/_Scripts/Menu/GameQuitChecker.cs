using UnityEngine;

public class GameQuitChecker : MonoBehaviour
{
    [SerializeField] private Material colorBlindlessMat;
    [ContextMenu("Run Prefs Reset")]public void SettingsReset()
    {
        PlayerPrefs.SetFloat("Volume", 1);
        PlayerPrefs.SetFloat("Brightness", 0);
        PlayerPrefs.SetFloat("ColorBlindlessValue", 0);
        PlayerPrefs.SetInt("PixelFilterValue", 5);
        PlayerPrefs.SetInt("BobSetting", 1);
        PlayerPrefs.SetInt("ScreenShakeSetting", 1);
        PlayerPrefs.SetString("ApplicationRunned", "first");
        colorBlindlessMat.SetInt("_Opacity", 0);
    }
    void OnApplicationQuit()
    {
        SettingsReset();
    }

}
