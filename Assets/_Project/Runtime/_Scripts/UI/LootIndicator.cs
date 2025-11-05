using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class LootIndicator : MonoBehaviour
{
    [SerializeField] float animSpeed = 0.67f;
    [SerializeField] GameObject button1;
    [SerializeField] GameObject button2;
    WaitForSeconds wait;

    SpriteRenderer[] spriteRenderer = { };

    Chest chest;

    Camera mainCamera;
    Coroutine animateCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sprite in spriteRenderer)
            sprite.enabled = false;

        chest = GetComponentInParent<Chest>();

        wait = new WaitForSeconds(animSpeed);
        mainCamera = Camera.main;
    }

    public void ShowIndicator(bool show)
    {
        if (spriteRenderer != null)
        {
            foreach (SpriteRenderer sprite in spriteRenderer)
                sprite.enabled = show;
        }

        if (show)
        {
            if (animateCoroutine == null)
                animateCoroutine = StartCoroutine(AnimateLoop());
        }
        else
        {
            if (animateCoroutine != null)
            {
                StopCoroutine(animateCoroutine);
                animateCoroutine = null;
            }

            if (button1 != null) button1.SetActive(false);
            if (button2 != null) button2.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (mainCamera != null)
            transform.forward = mainCamera.transform.forward;

        if (chest != null && chest.IsOpened)
        {
            if (spriteRenderer != null)
            {
                foreach (SpriteRenderer sprite in spriteRenderer)
                    sprite.enabled = false;
            }

            if (animateCoroutine != null)
            {
                StopCoroutine(animateCoroutine);
                animateCoroutine = null;
            }

            if (button1 != null) button1.SetActive(false);
            if (button2 != null) button2.SetActive(false);
        }
    }

    IEnumerator AnimateLoop()
    {
        if (button1 != null) button1.SetActive(true);
        if (button2 != null) button2.SetActive(false);

        while (true)
        {
            yield return wait;

            if (button1 == null || button2 == null) yield break;

            bool b1 = button1.activeSelf;
            button1.SetActive(!b1);
            button2.SetActive(b1);
        }
    }
}
