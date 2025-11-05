using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class Relic : MonoBehaviour, IInteractable
{
    [SerializeField] private VolumeProfile afterVolume;

    public void Start()
    {
        if(afterVolume == null)
        {
            throw new System.Exception("No volume profile found. Please put PP_After Volume Profile directly from objects PreFab");
        }
    }
    
    public void Interact()
    {
        Debug.Log($"{name} was picked up!");

        var player = FindFirstObjectByType<PlayerController>();

        Volume globalVolume = FindFirstObjectByType<Volume>();

        if (player != null)
        {
            player.HasRelic = true;
            Debug.Log("player was found on relic interact");
        }

        globalVolume.profile = afterVolume;
        
        // find all enemies
        var enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None).ToList();
        foreach (var enemy in enemies.Where(e => e.Type == Enemy.EnemyType.Banshee))
        {
            enemy.RankUp();
        }

        Destroy(gameObject);
    }

}
