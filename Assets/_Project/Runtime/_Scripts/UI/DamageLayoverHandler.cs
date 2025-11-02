using System;
using UnityEngine;
using UnityEngine.UI;

public class DamageLayoverHandler : MonoBehaviour // Mostly vibecoded by vibeGPT
{
    [SerializeField] private Image targetImage;
    [SerializeField] Sprite damageSprite;
    [SerializeField] Sprite healSprite;
    [SerializeField] private float lowHealthThreshold = 5f;
    [Range(0f, 1f)]
    [SerializeField] private float maxAlphaAtZeroHP = 0.6f;
    [Range(1f, 20f)]
    [SerializeField] private float baseAlphaLerpSpeed = 10f;

    [Range(0f, 1f)]
    [SerializeField] private float flashPeakAlpha = 0.35f;
    [Range(0.05f, 2f)]
    [SerializeField] private float flashDuration = 0.45f;

    private float currentAlpha;
    private float flashElapsed = 999f;
    private float lastCurrentHP = 999f;

    private void Reset()
    {
        targetImage = GetComponent<Image>();
    }

    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if (!targetImage) return;
            if (targetImage.enabled) Logger.LogWarning("Target Image should be disabled in edit mode to avoid interfering with other UI elements.", this, "DamageLayoverHandler");
            targetImage.enabled = false;
        }
    }

    private void Awake()
    {
        if (!targetImage) targetImage = GetComponent<Image>();
        if (targetImage)
        {
            var c = targetImage.color;
            c.a = 0f;
            targetImage.color = c;
        }

        targetImage.enabled = true;
    }

    private void OnEnable()
    {
        PlayerHealth.OnHealthChanged += DamageLayoverChange;
    }

    private void OnDisable()
    {
        PlayerHealth.OnHealthChanged -= DamageLayoverChange;
    }

    private void Update()
    {
        if (!targetImage) return;

        float baseTargetAlpha = 0f;
        float hp = lastCurrentHP;

        if (hp <= lowHealthThreshold)
        {
            float t = Mathf.InverseLerp(lowHealthThreshold, 0f, Mathf.Max(0f, hp));
            baseTargetAlpha = t * maxAlphaAtZeroHP;
        }

        flashElapsed += Time.deltaTime;
        float flashT = Mathf.Clamp01(flashElapsed / flashDuration);
        float flashAlpha = flashPeakAlpha * (1f - Mathf.SmoothStep(0f, 1f, flashT));

        float targetAlpha = Mathf.Min(baseTargetAlpha + flashAlpha, maxAlphaAtZeroHP);

        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * baseAlphaLerpSpeed);

        var col = targetImage.color;
        col.a = currentAlpha;
        targetImage.color = col;
    }

    private void DamageLayoverChange(float currentHealth, float previousHealth)
    {
        if (currentHealth > previousHealth)
        {
            targetImage.sprite = healSprite;
            lastCurrentHP = currentHealth;
            flashElapsed = 0f;
        }
        else
        {
            targetImage.sprite = damageSprite;
            lastCurrentHP = currentHealth;
            flashElapsed = 0f;
        }
    }
}
