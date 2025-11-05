using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject firstObject;
    [SerializeField] private GameObject accessibilityMenu;


    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

            FindAnyObjectByType<GameQuitChecker>().SettingsReset();
    }
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(firstObject);
    }
    public void PlayButton()
    {
        SceneManager.LoadScene(1);
    }

    public void AccessibilityButton()
    {
        mainMenu.SetActive(false);
        accessibilityMenu.SetActive(true);
    }

    public void OptionsButton()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }
}