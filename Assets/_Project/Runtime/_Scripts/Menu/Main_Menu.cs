using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject firstObject;
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(firstObject);
    }
    public void PlayButton()
    {

        //Only for Editor
#if UNITY_EDITOR
        //Debug.Log("Changing To Game Scene!");
        SceneManager.LoadScene(1);
#endif
        // GameScene missing for now ****!

    }

    public void ExitButton()
    {
        //Only for Editor
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif

        //Exits Game
        Application.Quit();
    }

    public void OptionsButton()
    {
        //Get Options
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }
}