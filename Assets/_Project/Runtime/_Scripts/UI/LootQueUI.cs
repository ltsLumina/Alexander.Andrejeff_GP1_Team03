using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class LootQueUI : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false; 
    }

    public void ShowIndicator(bool show)
    {
        spriteRenderer.enabled = show;
    }

    private void LateUpdate()
    {
        if (Camera.main != null)
            transform.forward = Camera.main.transform.forward;
    }
}
