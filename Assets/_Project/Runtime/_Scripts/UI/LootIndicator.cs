using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class LootIndicator : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Chest chest;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

        chest = GetComponentInParent<Chest>();
    }

    public void ShowIndicator(bool show)
    {
        spriteRenderer.enabled = show;
    }

    private void LateUpdate()
    {
        if (Camera.main != null)
            transform.forward = Camera.main.transform.forward;

        if (chest.IsOpened)
            spriteRenderer.enabled = false;
    }
}
