using System.Collections;
using UnityEngine;

public class ButtonAnim : MonoBehaviour
{
    [SerializeField] float animSpeed = 0.67f;
    [SerializeField] GameObject button1;
    [SerializeField] GameObject button2;

    WaitForSeconds wait;

    void OnEnable()
    {
        if (button1 == null || button2 == null)
        {
            Debug.LogWarning("Button references not assigned.", this);
            enabled = false;
            return;
        }

        wait = new WaitForSeconds(animSpeed);
        StartCoroutine(AnimateLoop());
    }

    IEnumerator AnimateLoop()
    {
        button1.SetActive(true);
        button2.SetActive(false);

        while (true)
        {
            yield return wait;

            bool b1 = button1.activeSelf;
            button1.SetActive(!b1);
            button2.SetActive(b1);
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }
}
