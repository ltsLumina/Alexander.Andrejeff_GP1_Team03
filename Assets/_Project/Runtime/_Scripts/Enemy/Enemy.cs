#region
using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using Lumina.Essentials.Attributes;
using MelenitasDev.SoundsGood;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using VInspector;
using Random = UnityEngine.Random;
#endregion

[SelectionBase, DisallowMultipleComponent, RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody), typeof(CapsuleCollider))]
public class Enemy : MonoBehaviour, IDamageable, IEnemyReset
{
	enum EnemyType
	{
		Octopus,
		Banshee,
		Debug,
	}

	[Tab("Enemy")]
	[SerializeField] EnemyType type;
	[Range(0, 20)]
	[SerializeField] float health = 20f;
	[Range(1, 20)]
	[SerializeField] float maxHealth = 20f;
	[SerializeField] [ReadOnly] float hearts = 10f;
	[SerializeField] public float damage = 1f;
	[SerializeField] public float attackCooldown = 2f;
	[SerializeField] public float attackRange = 3.1f;
	[Tooltip("The range at which the enemy can detect the player.")]
	[SerializeField] public float detectionRange = 31f;
	[SerializeField] float staggerDuration = 1.5f;
	[SerializeField] ParticleSystem hurtVFX;
	[SerializeField] RoomRegistry room;

	[Tab("NavMesh")]
	[SerializeField] Transform target;

	public float Health => health;
	public float MaxHealth => maxHealth;

	float distanceToPlayer;
	float attackTimer = 5f;

	Vector3 startPos;
	Quaternion startRot;

	NavMeshAgent agent;
	Rigidbody rb;
	Collider col;
	Animator animator;

	Sound snarlSFX; // generic creature sound that loops quietly
	Sound octopusSFX; // octopus wail
	Sound bansheeSFX; // watcher wail
	Sound hurtSFX; // generic hurt sound
	Sound daggerHurtSFX; // from dagger exclusively

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, attackRange);

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, detectionRange);
	}
	
	void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		rb = GetComponent<Rigidbody>();
		col = GetComponent<Collider>();
		animator = GetComponentInChildren<Animator>();

		room = room ? room : GetComponentInParent<RoomRegistry>();
		if (room) room.Register(this);

		startPos = transform.position;
		startRot = transform.rotation;

		Debug.Assert(agent, "NavMeshAgent component is missing!", this);
		Debug.Assert(target, "Target field is not assigned! \nThis should almost always be the player.", this);
	}

	void Start()
	{
		health = maxHealth;
		hearts = health / 2f;

		name = $"Enemy | {type.ToString()} | ({Random.Range(1000, 9999)})";

		#region Sounds
		switch (type)
		{
			case EnemyType.Octopus:
				octopusSFX = new Sound(SFX.OctopusWail);
				octopusSFX.SetSpatialSound();
				octopusSFX.SetFollowTarget(transform);
				octopusSFX.SetHearDistance(10, 35);
				octopusSFX.SetCustomVolumeRolloffCurve(AnimationCurve.EaseInOut(0, 1, 1, 0));
				octopusSFX.SetVolume(0.5f);
				octopusSFX.Play();
				break;

			case EnemyType.Banshee:
				bansheeSFX = new Sound(SFX.CreatureBanshee);
				bansheeSFX.SetSpatialSound();
				bansheeSFX.SetHearDistance(10, 50);
				bansheeSFX.SetCustomVolumeRolloffCurve(AnimationCurve.EaseInOut(0, 1, 1, 0));
				bansheeSFX.SetFollowTarget(transform);
				bansheeSFX.SetVolume(0.3f);
				bansheeSFX.Play();
				break;

			case EnemyType.Debug:
				break;

			default:
				throw new ArgumentOutOfRangeException();
		}

		snarlSFX = new Sound(SFX.CreatureSnarl);
		snarlSFX.SetSpatialSound();
		snarlSFX.SetHearDistance(attackRange, detectionRange);
		snarlSFX.SetFollowTarget(transform);
		snarlSFX.SetVolume(0.01f);
		snarlSFX.SetLoop(true);
		snarlSFX.Play();

		hurtSFX = new Sound(SFX.SliceGush);
		hurtSFX.SetSpatialSound();
		hurtSFX.SetVolume(0.35f);
		hurtSFX.SetFollowTarget(transform);
		
		daggerHurtSFX = new Sound(SFX.DaggerCut);
		daggerHurtSFX.SetSpatialSound();
		daggerHurtSFX.SetRandomPitch();
		daggerHurtSFX.SetVolume(0.35f);
		daggerHurtSFX.SetFollowTarget(transform);

		shriekCoroutine = StartCoroutine(Shriek());
		#endregion
	}

	Coroutine shriekCoroutine;

	bool shriekOnCooldown;

	IEnumerator Shriek()
	{
		while (health > 0)
		{
			float waitTime = Random.Range(5f, 15f);
			yield return new WaitForSeconds(waitTime);

			switch (type)
			{
				case EnemyType.Octopus:
					octopusSFX.SetRandomPitch();
					octopusSFX.Play();
					break;

				case EnemyType.Banshee:
					bansheeSFX.Play();
					break;

				case EnemyType.Debug:
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	void Update()
	{
		if (attackTimer >= 0) attackTimer -= Time.deltaTime;
		if (!target) return;
		if (agent.isStopped) return;

		distanceToPlayer = Vector3.Distance(transform.position, target.position);

		hearts = health / 2f;

		if (distanceToPlayer > detectionRange) return;

		float offsetDistance = 3f;
		Vector3 desired = target.position + target.forward * offsetDistance;

		// returns the nearest point on the navmesh to the desired position
		Vector3 nearestPoint = NavMesh.SamplePosition(desired, out NavMeshHit hit, 1.0f, NavMesh.AllAreas) ? hit.position : desired;

		// stops the enemy from moving if within attack range
		if (distanceToPlayer > attackRange) agent.SetDestination(nearestPoint);

		animator.SetBool("isMoving", agent.velocity.magnitude > 0.1f);

		if (distanceToPlayer <= attackRange && attackTimer <= 0 && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
		{
			attackTimer = attackCooldown;
			
			StartCoroutine(Attack());
		}
	}

	IEnumerator Attack()
	{ 
		// pause shriek coroutine during attack
		if (shriekCoroutine != null)
		{
			StopCoroutine(shriekCoroutine);
			shriekCoroutine = StartCoroutine(Shriek());
		}
		
		float animDuration = animator.GetCurrentAnimatorStateInfo(0).length;
		
		animator.SetTrigger("attack");
		switch (type)
		{
			case EnemyType.Octopus:
				octopusSFX.Play();
				break;

			case EnemyType.Banshee:
				bansheeSFX.Play();
				break;
		}

		yield return new WaitForSeconds(animDuration / 3f);
		
		target.TryGetComponent(out IDamageable damageable);
		damageable.TakeDamage(damage);
	}

	public void TakeDamage(float damage, DamageSource source)
	{
		health -= damage;
		
		var instance = Instantiate(hurtVFX, transform.position, Quaternion.Inverse(transform.rotation));
		transform.DOShakePosition(0.2f, 0.5f).SetLink(gameObject);
		
		// ignore this mess - it flashes the enemy red when hurt
		GetComponentsInChildren<Renderer>().ToList().ForEach(r => r.material.DOColor(Color.red, "_BaseColor", 0.1f).OnComplete(() =>
		{
			r.material.DOColor(Color.white, "_BaseColor", 0.1f);
		}).SetLink(gameObject));
		
		hurtSFX.Play();
		
		if (target.TryGetComponent(out PlayerController player) && player?.Weapon.EquippedWeapon == Weapon.Weapons.Dagger && source == DamageSource.Player)
		{
			daggerHurtSFX.Play();
		}

		//Logger.Log("Enemy took: " + damage + " damage.", this, $"{name}");

		if (health <= 0) Death();
	}

	void Death()
	{
		Logger.LogWarning("Enemy died.", this, $"{name}");
		room.Unregister(this);
		
		snarlSFX.Stop();
		Destroy(gameObject);
	}

	public void Knockback(float knockbackForce)
	{
		rb.isKinematic = false;

		// knock backwards, opposite to where the enemy is moving
		rb.AddForce(-agent.velocity.normalized * knockbackForce, ForceMode.Impulse);
		
		Stagger(staggerDuration);
	}

	public void Stagger(float duration = -1) => StartCoroutine(PerformStagger(duration));

	IEnumerator PerformStagger(float duration)
	{
		if (duration < 0) duration = staggerDuration;
		
		agent.isStopped = true;
		yield return new WaitForSeconds(duration);
		agent.isStopped = false;

		rb.isKinematic = true;
	}

	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.TryGetComponent(out Crate crate))
		{
			if (crate.Breakable)
				crate.TakeDamage(1, DamageSource.Enemy);
			
			TakeDamage(1, DamageSource.Crate);
		}
	}

	//--------------------------------- respawn mechanics n stuff----------------------------------------

	void OnEnable() => room.Register(this);

	void OnDisable() => room.Unregister(this);

	public void ResetToStart()
	{
		if (agent)
		{
			agent.Warp(startPos);
			transform.rotation = startRot;
			agent.ResetPath();
		}
	}
}
