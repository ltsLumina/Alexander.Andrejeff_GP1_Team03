using UnityEngine;

public class DisableDebugTools : MonoBehaviour
{
	// This runs only in non-editor builds and disables all direct child GameObjects.
#if !UNITY_EDITOR
    void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        Destroy(gameObject);
    }
#endif
}
