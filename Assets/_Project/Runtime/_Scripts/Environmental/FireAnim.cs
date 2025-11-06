using UnityEngine;
using DG.Tweening;

public class FireAnim : MonoBehaviour
{
    [SerializeField] float radius = 1f;
    [SerializeField] float minDuration = 0.8f;
    [SerializeField] float maxDuration = 1.5f;
    [SerializeField] Ease ease = Ease.Linear;
    [SerializeField] bool loop = true;
    [SerializeField] bool useXZPlane = true; // true -> XZ plane, false -> XY plane
    [SerializeField] bool localSpace = false;

    Vector3 origin;
    Tween current;

    void Start()
    {
        origin = localSpace ? transform.localPosition : transform.position;
        MoveToRandom();
    }

    void OnDisable()
    {
        current?.Kill();
    }

    void MoveToRandom()
    {
        Vector2 p = Random.insideUnitCircle * radius;
        Vector3 target = useXZPlane
            ? new Vector3(origin.x + p.x, origin.y, origin.z + p.y)
            : new Vector3(origin.x + p.x, origin.y + p.y, origin.z);

        float duration = Random.Range(minDuration, maxDuration);

        current = (localSpace ? transform.DOLocalMove(target, duration) : transform.DOMove(target, duration))
            .SetEase(ease)
            .OnComplete(() =>
            {
                if (loop) MoveToRandom();
                else current = null;
            });
    }

    public void Restart()
    {
        current?.Kill();
        origin = localSpace ? transform.localPosition : transform.position;
        MoveToRandom();
    }
}