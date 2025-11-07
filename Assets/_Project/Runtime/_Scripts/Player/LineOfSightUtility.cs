using UnityEngine;

public static class LineOfSightUtility
{
	/// <summary>
	/// Checks if there is a clear line of sight from the main camera to the target point, considering specified line-of-sight blockers and on-screen margin.
	/// </summary>
	/// <param name="targetTransform"></param>
	/// <param name="targetPoint"></param>
	/// <param name="losBlockers"></param>
	/// <param name="onScreenMargin"></param>
	/// <returns> true if the camera can "see" the target point and there is no blocker in losBlockers between camera and targetTransform. </returns>
	public static bool HasLineOfSightToTarget(Transform targetTransform, Vector3 targetPoint, LayerMask losBlockers, float onScreenMargin = 0.03f)
	{
		var cam = Camera.main;
		if (cam == null) return false;

		// viewport / on-screen test
		Vector3 eVP = cam.WorldToViewportPoint(targetPoint);
		bool onScreen = eVP.z > 0f && eVP.x > onScreenMargin && eVP.x < 1f - onScreenMargin && eVP.y > onScreenMargin && eVP.y < 1f - onScreenMargin;
		if (!onScreen) return false;

		// raycast from camera to target point
		Vector3 origin = cam.transform.position;
		Vector3 dir = targetPoint - origin;
		float dist = dir.magnitude;
		if (dist <= 0.001f) return false;

		if (Physics.Raycast(origin, dir / dist, out var hit, dist, losBlockers, QueryTriggerInteraction.Ignore))
		{
			// blocked unless we hit the target's root
			return hit.collider.transform.root == targetTransform.root;
		}

		// no blocker hit
		return true;
	}
}
