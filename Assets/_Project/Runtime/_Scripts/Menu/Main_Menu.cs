using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject accessibilityMenu;
    
    [Header("Other")]
    [SerializeField] private GameObject firstObject;
    [SerializeField] EyeBlink eye;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        FindAnyObjectByType<GameQuitChecker>().SettingsReset();
    }

    void OnEnable() => EventSystem.current.SetSelectedGameObject(firstObject);

    public void PlayButton()
    {
        StartCoroutine(Load());

        return;
        IEnumerator Load()
        {
            eye.Blink();
            
            yield return new WaitForSeconds(1f);
            
            SceneManager.LoadScene(1);
        }
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