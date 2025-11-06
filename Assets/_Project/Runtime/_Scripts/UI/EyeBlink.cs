using System;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using VInspector;

public class EyeBlink : MonoBehaviour
{
	[Tab("Blink")]
	[SerializeField] bool playOnAwake;
	[SerializeField] bool invert;
	[HideIf(nameof(invert), true)]
    [SerializeField] float forwardsDuration = 2f;
    [ShowIf(nameof(invert), true)]
    [SerializeField] float backwardsDuration = 0.5f;
    [EndIf]
    [SerializeField] Ease ease;

    [Tab("References")]
    [SerializeField] RectTransform topLid;
    [SerializeField] RectTransform botLid;

    [Button, UsedImplicitly]
    public void Blink()
    {
	    // topLid.anchoredPosition = Vector2.zero;
	    // botLid.anchoredPosition = Vector2.zero;
	    
	    Play();
    }

    void Awake()
    {
	    topLid.gameObject.SetActive(false);
	    botLid.gameObject.SetActive(false);
	    
	    if (playOnAwake) Blink();
    }

    void Play()
    {
	    Debug.Assert(topLid != null, "Top lid reference is missing");
	    Debug.Assert(botLid != null, "Bot lid reference is missing");
	    Debug.Assert(forwardsDuration > 0f, "Duration must be greater than zero");

	    if (!invert) Forwards();
	    else Backwards();
    }

    void Forwards()
    {
	    topLid.gameObject.SetActive(true);
	    botLid.gameObject.SetActive(true);
	    
	    Sequence sequence = DOTween.Sequence();
	    sequence.Append(topLid.DOAnchorPosY(topLid.anchoredPosition.y + topLid.rect.height, 0.35f));
	    sequence.Join(topLid.DOScaleY(3f, 0.35f).SetEase(Ease.Linear));
	    sequence.Join(botLid.DOAnchorPosY(botLid.anchoredPosition.y - botLid.rect.height, 0.35f));
	    sequence.Join(botLid.DOScaleY(3f, 0.35f).SetEase(Ease.Linear));
	    sequence.Append(topLid.DOScaleX(6.5f, forwardsDuration));
	    sequence.Join(botLid.DOScaleX(6.5f, forwardsDuration));
	    sequence.Append(topLid.DOScaleY(0f, forwardsDuration).SetEase(Ease.Linear));
	    sequence.Join(botLid.DOScaleY(0f, forwardsDuration).SetEase(Ease.Linear));
	    sequence.SetEase(ease);
	    sequence.SetLink(gameObject);
    }

    void Backwards()
    {
	    topLid.gameObject.SetActive(true);
	    botLid.gameObject.SetActive(true);
	    
	    Sequence sequence = DOTween.Sequence();
	    sequence.Append(topLid.DOAnchorPosY(topLid.anchoredPosition.y + topLid.rect.height, 0.35f));
	    sequence.Join(topLid.DOScaleY(3f, 0.35f).SetEase(Ease.Linear));
	    sequence.Join(botLid.DOAnchorPosY(botLid.anchoredPosition.y - botLid.rect.height, 0.35f));
	    sequence.Join(botLid.DOScaleY(3f, 0.35f).SetEase(Ease.Linear));
	    sequence.Append(topLid.DOScaleX(6.5f, backwardsDuration));
	    sequence.Join(botLid.DOScaleX(6.5f, backwardsDuration));
	    sequence.Append(topLid.DOScaleY(0f, backwardsDuration).SetEase(Ease.Linear));
	    sequence.Join(botLid.DOScaleY(0f, backwardsDuration).SetEase(Ease.Linear));
	    sequence.SetEase(ease);
	    sequence.SetLink(gameObject);
	    sequence.SetAutoKill(false);
	    sequence.Goto(sequence.Duration(), false);
	    sequence.PlayBackwards();
    }
}
