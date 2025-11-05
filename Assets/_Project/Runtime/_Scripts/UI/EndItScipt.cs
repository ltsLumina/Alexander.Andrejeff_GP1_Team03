using UnityEngine;

public class EndItScipt : MonoBehaviour
{

    [SerializeField] private Pause_Menu endGameUI;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            endGameUI.VictoryMenuTriggered();
        }
    }
}
