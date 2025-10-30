using System;
using UnityEngine;

public class WeaponLootable : MonoBehaviour, IInteractable
{
    [SerializeField] WeaponData data;
    [SerializeField] MeshFilter meshFilter;

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!meshFilter || !data) return;
        meshFilter.mesh = data.Mesh;
    }
#endif

    void Start() => meshFilter.mesh = data.Mesh;

    public void Interact()
    {
        Debug.Log($"{name} was picked up!");
        
        var player = FindFirstObjectByType<PlayerController>();

        player.Weapon.Equip(data);
    }
}
