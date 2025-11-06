using System.Linq;
using MelenitasDev.SoundsGood;
using UnityEngine;
using UnityEngine.Rendering;

public class Relic : MonoBehaviour, IInteractable
{
    [SerializeField] VolumeProfile afterVolume;

    Sound pickupSound;
    
    public void Start()
    {
        Debug.Assert(afterVolume, "No volume profile found. Please put PP_After Volume Profile directly from objects Prefab");

        pickupSound = new Sound(SFX.AmbientShriek);
        pickupSound.SetOutput(Output.SFX);
        pickupSound.SetSpatialSound();
        pickupSound.SetVolume(0.5f);
        pickupSound.SetPosition(transform.position);
    }
    
    public void Interact()
    {
        Debug.Log($"{name} was picked up!");

        var player = FindFirstObjectByType<PlayerController>();
        var playerHealth = player.GetComponent<PlayerHealth>();
        playerHealth.IncreaseMaxHealth(10, true);

        player.BaseMoveSpeed *= 2;
        player.Weapon.AttackCooldown /= 2;
        player.Weapon.KickCooldown /= 2;

        Volume globalVolume = FindFirstObjectByType<Volume>();

        if (player != null) player.HasRelic = true;

        globalVolume.profile = afterVolume;
        
        pickupSound.Play();
        
        // find all enemies
        var enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None).ToList();
        foreach (var enemy in enemies.Where(e => e.Type == Enemy.EnemyType.Banshee))
        {
            enemy.RankUp();
        }

        Destroy(gameObject);
    }

}
