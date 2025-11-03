using System;
using DG.Tweening;
using UnityEngine;
using VInspector;

public class EyeBlink : MonoBehaviour
{
    [Tab("Blink")]
    [SerializeField] float duration;
    [SerializeField] Ease ease;

    [Tab("References")]
    [SerializeField] RectTransform topLid;
    [SerializeField] RectTransform botLid;

    [Button]
    void Blink()
    {
	    topLid.anchoredPosition = Vector2.zero;
	    botLid.anchoredPosition = Vector2.zero;
	    
	    Start();
    }

    void Start()
    {
	    Debug.Assert(topLid != null, "Top lid reference is missing");
	    Debug.Assert(botLid != null, "Bot lid reference is missing");
	    Debug.Assert(duration > 0f, "Duration must be greater than zero");
	    
	    Sequence sequence = DOTween.Sequence();
	    sequence.Append(topLid.DOAnchorPosY(topLid.anchoredPosition.y + topLid.rect.height, 0.35f));
	    sequence.Join(topLid.DOScaleY(3f, 0.35f).SetEase(Ease.Linear));
	    sequence.Join(botLid.DOAnchorPosY(botLid.anchoredPosition.y - botLid.rect.height, 0.35f));
	    sequence.Join(botLid.DOScaleY(3f, 0.35f).SetEase(Ease.Linear));
	    sequence.Append(topLid.DOScaleX(6.5f, duration));
	    sequence.Join(botLid.DOScaleX(6.5f, duration));
	    sequence.Append(topLid.DOScaleY(0f, duration).SetEase(Ease.Linear));
	    sequence.Join(botLid.DOScaleY(0f, duration).SetEase(Ease.Linear));
	    sequence.SetEase(ease);
    }
}
