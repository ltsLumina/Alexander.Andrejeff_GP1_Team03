#pragma warning disable 0414

#region
using System;
using System.Collections;
using DG.Tweening;
using Lumina.Essentials.Attributes;
using Lumina.Essentials.Modules;
using MelenitasDev.SoundsGood;
using UnityEngine;
using VInspector;
#endregion

public class SpikeTrap : MonoBehaviour
{
	enum TrapType
	{
		[Tooltip("The trap is always active.")]
		Static,
		[Tooltip("The trap activates periodically.")]
		Periodic,
		[Tooltip("The trap activates when the player/enemy steps on it.")]
		Triggered,
	}
	
	[Tab("Trap")]
	[SerializeField] TrapType type;

	[Header("Static")] [ShowIf(nameof(type), TrapType.Static)]
	[SerializeField] [ReadOnly] string staticInfo = "The trap is always active.";

	[Header("Periodic")] [ShowIf(nameof(type), TrapType.Periodic)]
	[Range(0.5f, 5f)]
	[SerializeField] float period = 2f;

	[Header("Trigger")]
	[ShowIf(nameof(type), TrapType.Triggered)]
	[Tooltip("Triggers once upon activation, then deactivates the trap, meaning it wont trigger again.)")]
	[SerializeField] bool triggerOnce;
	[EndIf]
	[ShowIf(nameof(triggerOnce), true)]
	[SerializeField, ReadOnly] bool hasTriggered; 
	[EndIf]
	[ShowIf(nameof(type), TrapType.Triggered)]
	[Range(0, 1f)] [Tooltip("A safety delay, allowing you to bait the trap without taking damage.")]
	[SerializeField] float playerTriggerDelay = 0.2f;
	[Range(0, 1f)] [Tooltip("The delay before activating the trap for enemies.")]
	[SerializeField] float enemyTriggerDelay = 0.2f;
	[Range(0.5f, 5f)]
	[Tooltip("The cooldown time before the trap can be triggered again.")]
	[SerializeField] float cooldown = 2.5f;
	[EndIf]
	
	[Header("Stats")]
	[SerializeField] float size = 1f;
	[Range(1, 20)]
	[SerializeField] float damage = 1;
	[SerializeField] float enemyDamageMultiplier = 2f;
	[SerializeField] float knockbackForce = 5f;

	[Tab("Settings")]
	// placeholder
	[SerializeField] GameObject spikeMesh;
	
	bool canActivate;
	bool isPlayer;
	IDamageable target;
	float timer;
	
	Sound trapSFX;

	bool @static => type == TrapType.Static;
	bool periodic => type == TrapType.Periodic;
	bool trigger => type == TrapType.Triggered;

	string additionalInfo => type switch
	{ TrapType.Static    => "Active.",
	  TrapType.Periodic  => $"Activates every {period} seconds.",
	  TrapType.Triggered => $"Cooldown: {cooldown} seconds.",
	  _                  => "" };

	void Start()
	{
		transform.localScale = new (size, 1, size);
		timer = 0;
		canActivate = true;

		name = $"Spike Trap ({type} - {additionalInfo})";

		#region Sound
		trapSFX = new (SFX.SpikeTrap);
		trapSFX.SetVolume(0.4f);
		trapSFX.SetSpatialSound();
		trapSFX.SetHearDistance(5f, 20f);
		trapSFX.SetFollowTarget(transform);
		#endregion

		if (@static) Static();
		if (periodic) StartCoroutine(Periodic());
	}

	void Update()
	{
		timer = Math.Max(0, timer - Time.deltaTime);
		canActivate = timer == 0;
	}

	void OnTriggerEnter(Collider other)
	{
		if (!other.TryGetComponent(out target))
        {
            target = null; 
            return;
        }
		
		isPlayer = other.CompareTag("Player");
		if (trigger) StartCoroutine(Trigger());
	}

	void OnTriggerStay(Collider other)
	{
		if (!other.TryGetComponent(out target))
		{
			target = null; 
			return;
		}
		
		isPlayer = other.CompareTag("Player");
	}

	void OnTriggerExit(Collider other)
	{
		isPlayer = false;
		target = null;
	}

	void OnValidate()
	{
		transform.localScale = new (size, 1, size);
		name = $"Spike Trap ({type} - {additionalInfo})";
	}

	void Static()
	{
		if (@static)
		{
			spikeMesh.transform.DOMoveY(2, 0.2f).SetEase(Ease.InExpo);
			trapSFX.Play();

			if (target != null)
			{
				TakeDamageAndKnockback();
				Logger.Log($"Trap triggered! | Hit: {target} | Damage dealt: {damage}", this, "SpikeTrap");
				target = null;
			}
		}
	}

	IEnumerator Periodic()
	{
		while (periodic)
		{
			yield return new WaitForSeconds(period);

			Sequence sequence = DOTween.Sequence();
			sequence.Append(spikeMesh.transform.DOMoveY(2, 0.2f).SetEase(Ease.InExpo));
			sequence.AppendCallback(() => trapSFX.Play());
			sequence.AppendInterval(0.2f);
			sequence.Append(spikeMesh.transform.DOMoveY(0, 0.2f).SetEase(Ease.OutExpo));

			if (target != null)
			{
				TakeDamageAndKnockback();
				Logger.Log($"Trap triggered! | Hit: {target} | Damage dealt: {damage}", this, "SpikeTrap");
				target = null;
			}
		}
	}

	IEnumerator Trigger()
	{
		if (!canActivate) yield break;

		switch (triggerOnce)
		{
			case false: {
				Sequence sequence = DOTween.Sequence();
				sequence.Append(spikeMesh.transform.DOMoveY(2, 0.2f).SetEase(Ease.InExpo));
				sequence.AppendCallback(() => trapSFX.Play());
				sequence.AppendInterval(0.2f);
				sequence.Append(spikeMesh.transform.DOMoveY(0, 0.2f).SetEase(Ease.OutExpo));

				yield return new WaitForSeconds(isPlayer ? playerTriggerDelay : enemyTriggerDelay);
				break;
			}

			case true when !hasTriggered:
				spikeMesh.transform.DOMoveY(2, 0.2f).SetEase(Ease.InExpo);
				break;
		}

		if (target != null)
		{
			TakeDamageAndKnockback();
			
			Logger.Log($"Trap triggered! | Hit: {target} | Damage dealt: {damage}", this, "SpikeTrap");
			target = null;
		}

		hasTriggered = true;
		timer = cooldown;
	}

	void TakeDamageAndKnockback()
	{
		var enemy = target as Enemy;
		enemy?.Knockback(knockbackForce);
		
		target.TakeDamage(damage * (enemy != null ? enemyDamageMultiplier : 1f));
	}
}
