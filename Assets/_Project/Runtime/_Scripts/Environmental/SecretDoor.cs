using UnityEngine;
using UnityEngine.Rendering;

public class SecretDoor : MonoBehaviour
{
    [SerializeField] bool isVisible = false;
    [SerializeField] Chest relicChest;

    BoxCollider bc;
    MeshRenderer mr;

    private void OnEnable()
    {
        relicChest.OnRelicChestOpened += SwitchDoorVisibility;
    }
    private void OnDisable()
    {
        relicChest.OnRelicChestOpened -= SwitchDoorVisibility;
    }

    private void Start()
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
