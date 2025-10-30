using System;
using UnityEditor;
using UnityEngine;
using VInspector;

public class Crate : MonoBehaviour, IDamageable
{
	[Header("Crate"), Tooltip("Can be broken on impact or by attacking it.")]
	[SerializeField] bool breakable;
	[ShowIf(nameof(breakable), true)]
	[SerializeField] float health = 1f;
	[EndIf]
	
	[Tooltip("If true, the crate can be broken on impact after kicking it. (Mostly for doors)")]
	[SerializeField] bool kickable;
	[ShowIf(nameof(kickable), true)]
	[Tooltip("The velocity threshold at which the crate will break upon collision.")]
	[SerializeField] float breakVelocity = 5f;
	[EndIf]

	[Header("VFX")]
	[SerializeField] ParticleSystem dustVFX;
	[SerializeField] ParticleSystem breakVFX;

	Rigidbody rb;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	public void TakeDamage(float damage)
	{
		health -= damage;
		if (health <= 0f) Break();
	}

	void Break()
	{
		Instantiate(breakVFX, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		if (!Application.isPlaying) return;
		
		string healthInfo = breakable ? $"Health: {health}" : "Unbreakable";
		string velocityInfo = $"Velocity: {rb.linearVelocity.magnitude:F2}";
		
		Handles.Label(transform.position + Vector3.up, $"[{name}]\n{healthInfo}\n{velocityInfo}");
	}
#endif

	void OnCollisionEnter(Collision other)
	{
		if (rb.linearVelocity.magnitude > 2f)
		{
			Vector3 offset = Vector3.up * 0.85f;
			Instantiate(dustVFX, transform.position - offset, Quaternion.identity);
		}

		if (rb.linearVelocity.magnitude >= breakVelocity)
		{
			if (breakable) TakeDamage(1f);
			if (kickable) Break();
		}
	}
}
