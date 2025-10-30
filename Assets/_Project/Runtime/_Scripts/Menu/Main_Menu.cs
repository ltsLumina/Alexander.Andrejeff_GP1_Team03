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

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(firstObject);
    }
    public void PlayButton()
    {

    }

    public void AccessibilityButton()
    {
        mainMenu.SetActive(false);
        accessibilityMenu.SetActive(true);
    }

    public void OptionsButton()
    {
        //Get Options
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }
}