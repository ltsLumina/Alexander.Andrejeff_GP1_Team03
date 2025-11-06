using UnityEngine;
using UnityEngine.Rendering;

public class SecretDoor : MonoBehaviour
{
    [SerializeField] bool isVisible;
    [SerializeField] Chest relicChest;

    BoxCollider bc;
    MeshRenderer mr;

    void OnEnable() => relicChest.OnRelicChestOpened += SwitchDoorVisibility;
    void OnDisable() => relicChest.OnRelicChestOpened -= SwitchDoorVisibility;

    void Start()
    {
        //gameObject.SetActive(isVisible);
        bc = GetComponent<BoxCollider>();
        mr = GetComponentInChildren<MeshRenderer>();

        bc.enabled = isVisible;
        mr.enabled = isVisible;
    }

    void SwitchDoorVisibility()
    {
        isVisible = !isVisible;

        bc.enabled = isVisible;
        mr.enabled = isVisible;
    }

}
