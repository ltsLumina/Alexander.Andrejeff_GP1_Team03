#region
using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using Lumina.Essentials.Attributes;
using MelenitasDev.SoundsGood;
using UnityEngine;
using UnityEngine.AI;
using VInspector;
using Random = UnityEngine.Random;
#endregion

[SelectionBase] [DisallowMultipleComponent] [RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
public class Enemy : MonoBehaviour, IDamageable, IEnemyReset, IInteractable
{
	public enum EnemyType
	{
		Octopus,
		Banshee,
	}
	
	[Tab("Enemy")]
	[SerializeField] EnemyType type;
	[Range(0, 20)]
	[SerializeField] float health = 20f;
	[Range(1, 20)]
	[SerializeField] float maxHealth = 20f;
	[SerializeField, ReadOnly] float hearts = 10f;
	[SerializeField] public float damage = 1f;
	[SerializeField] public float attackCooldown = 2f;
	[SerializeField] public float attackRange = 3.1f;
	[Tooltip("The range at which the enemy can detect the player.")]
	[SerializeField] public float detectionRange = 31f;
	[SerializeField] float staggerDuration = 1.5f;
	[SerializeField] Material octopusDeathMaterial;
	[SerializeField] Material watcherDeathMaterial;
	[SerializeField] Material watcherTier2Material;
	[SerializeField] ParticleSystem hurtVFX;
	[SerializeField] RoomRegistry room;

	[Tab("Patrol")]
	[SerializeField] bool randomPatrol = true;
	[HideIf(nameof(randomPatrol), true)]
	[SerializeField] GameObject patrolStart;
	[SerializeField] GameObject patrolEnd;
	[Tooltip("The radius around the starting position in which the enemy will patrol randomly.")]
	[SerializeField] float patrolRadius = 4f;
	[SerializeField] float patrolSpeed;
	[SerializeField] float patrolYieldTime;
	[EndIf]
	[Tab("NavMesh")]
	[SerializeField] Transform target;

	bool tier2;
	
	NavMeshAgent agent;
	Animator animator;
	float attackTimer = 5f;
	Sound bansheeSFX; // watcher wail
	Collider col;
	Sound daggerHurtSFX; // from dagger exclusively

	float distanceToPlayer;
	Sound hurtSFX;    // generic hurt sound
	Sound octopusSFX; // octopus wail
	Rigidbody rb;

	Coroutine shriekCoroutine;

	bool shriekOnCooldown;

	Sound snarlSFX; // generic creature sound that loops quietly

	Vector3 startPos;
	Quaternion startRot;

	public GameObject Mesh { get; set; }

	public float Health => health;
	public float MaxHealth => maxHealth;

	public EnemyType Type => type;
	public InteractableType InteractableType => InteractableType.Enemy; // For enemy reticle

    void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		rb = GetComponent<Rigidbody>();
		col = GetComponent<BoxCollider>();

		room ??= GetComponentInParent<RoomRegistry>();
		if (room) room.Register(this);

		startPos = transform.position;
		startRot = transform.rotation;

		Debug.Assert(agent, "NavMeshAgent component is missing!", this);
		Debug.Assert(target, "Target field is not assigned! \nThis should almost always be the player.", this);
		Debug.Assert(room, "RoomRegistry component is missing! \nThis is required for enemy respawn mechanics.", this);
	}

	void Start()
	{
		if (!target) target = FindFirstObjectByType<PlayerController>()?.transform;
		
		animator = Mesh.GetComponent<Animator>();

		health = maxHealth;
		hearts = health / 2f;

		name = $"Enemy | {type.ToString()} | ({Random.Range(1000, 9999)})";

		#region Sounds
		switch (type)
		{
			case EnemyType.Octopus:
				octopusSFX = new (SFX.OctopusWail);
				octopusSFX.SetOutput(Output.SFX);
				octopusSFX.SetSpatialSound();
				octopusSFX.SetFollowTarget(transform);
				octopusSFX.SetHearDistance(5, 10);
				octopusSFX.SetVolume(0.2f);
				break;

			case EnemyType.Banshee:
				bansheeSFX = new (SFX.CreatureBanshee);
				bansheeSFX.SetOutput(Output.SFX);
				bansheeSFX.SetSpatialSound();
				bansheeSFX.SetHearDistance(5, 15);
				bansheeSFX.SetFollowTarget(transform);
				bansheeSFX.SetVolume(0.1f);
				break;

			default:
				throw new ArgumentOutOfRangeException();
		}

		snarlSFX = new (SFX.CreatureSnarl);
		snarlSFX.SetOutput(Output.SFX);
		snarlSFX.SetSpatialSound();
		snarlSFX.SetHearDistance(attackRange, 10);
		snarlSFX.SetFollowTarget(transform);
		snarlSFX.SetVolume(0.01f);
		snarlSFX.SetLoop(true);
		snarlSFX.Play();

		hurtSFX = new (SFX.SliceGush);
		hurtSFX.SetOutput(Output.SFX);
		hurtSFX.SetSpatialSound();
		hurtSFX.SetVolume(0.35f);
		hurtSFX.SetFollowTarget(transform);

		daggerHurtSFX = new (SFX.DaggerCut);
		daggerHurtSFX.SetOutput(Output.SFX);
		daggerHurtSFX.SetSpatialSound();
		daggerHurtSFX.SetRandomPitch();
		daggerHurtSFX.SetVolume(0.35f);
		daggerHurtSFX.SetFollowTarget(transform);

		shriekCoroutine = StartCoroutine(Shriek());
		#endregion
	}

	void OnDestroy() => name = $"Enemy | {type.ToString()}";

	public void RankUp()
	{
		var renderers = transform.GetChild(1).GetComponentsInChildren<Renderer>();
		foreach (var rend in renderers)
		{
			var mats = rend.materials;
			for (int i = 0; i < mats.Length; i++)
			{
				mats[i] = watcherTier2Material;
			}
			rend.materials = mats;
		}

		agent.speed *= 2;
		attackCooldown /= 2;
		damage *= 2;
		maxHealth *= 2;
		health = maxHealth;
	}
	
	void Update()
	{
		if (!Application.isPlaying) return;
		if (IsDead) return;

		if (attackTimer >= 0) attackTimer -= Time.deltaTime;
		if (!target) return;
		if (agent.isStopped) return;

		distanceToPlayer = Vector3.Distance(transform.position, target.position);

		hearts = health / 2f;

		// Patrol when player is out of detection range
		if (distanceToPlayer > detectionRange)
		{
			bool reachedDestination = !agent.hasPath || agent.remainingDistance <= agent.stoppingDistance + 0.5f;

			if (randomPatrol)
			{
				// pick a new random patrol point when we don't have a path or we've reached the current one
				if (reachedDestination)
				{
					// pick a random point on the XZ plane around the startPos
					Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
					var randomPoint = new Vector3(startPos.x + randomCircle.x, startPos.y, startPos.z + randomCircle.y);

					if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
					{
						agent.speed = patrolSpeed;
						agent.isStopped = false;
						agent.SetDestination(hit.position);
					}
				}
			}
			else
			{
				agent.speed = patrolSpeed;

				// patrol between two set points
				if (patrolStart != null && patrolEnd != null && reachedDestination)
				{
					// only start the wait+move coroutine if we're not already stopped/waiting
					if (!agent.isStopped)
					{
						Vector3 startPoint = patrolStart.transform.position;
						Vector3 endPoint = patrolEnd.transform.position;

						// determine which point we should go to next by comparing distance to current destination
						Vector3 currentDest = agent.hasPath ? agent.destination : transform.position;
						float distToStart = Vector3.SqrMagnitude(currentDest - startPoint);
						float distToEnd = Vector3.SqrMagnitude(currentDest - endPoint);

						Vector3 nextPoint = distToStart < distToEnd ? endPoint : startPoint;

						StartCoroutine(YieldThenGo(nextPoint));
					}
				}
			}

			animator.SetBool("isMoving", agent.hasPath && agent.velocity.magnitude >= 1f);
			return;
		}

		float offsetDistance = 3f;
		Vector3 desired = target.position + target.forward * offsetDistance;

		// stops the enemy from moving if within attack range
		if (distanceToPlayer > attackRange)
		{
			// returns the nearest point on the navmesh to the desired position
			Vector3 nearestPoint = NavMesh.SamplePosition(desired, out NavMeshHit hit, 1.0f, NavMesh.AllAreas) ? hit.position : desired;
			agent.isStopped = false;
			agent.SetDestination(nearestPoint);
		}

		animator.SetBool("isMoving", agent.hasPath && agent.velocity.magnitude > 0.1f);

		if (distanceToPlayer <= attackRange && attackTimer <= 0 && (agent.remainingDistance <= agent.stoppingDistance + 0.5f || !agent.hasPath))
		{
			attackTimer = attackCooldown;

			if (IsDead) return;

			StartCoroutine(Attack());
		}
	}

	IEnumerator YieldThenGo(Vector3 nextPoint)
	{
		agent.isStopped = true;
		yield return new WaitForSeconds(patrolYieldTime);
		agent.isStopped = false;
		agent.speed = patrolSpeed;
		agent.SetDestination(nextPoint);
	}

	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.TryGetComponent(out Crate crate) && crate.rb.linearVelocity.magnitude > 1f)
		{
			if (crate.Breakable) crate.TakeDamage(1, DamageSource.Enemy);
		}

		var player = FindAnyObjectByType<PlayerController>();

		if (player.Weapon.HurtOnWall && other.gameObject.layer == LayerMask.NameToLayer("Wall"))
		{
			TakeDamage(99, DamageSource.Player);
			Logger.Log("died from wall collision because player has hurt on wall upgrade", other.gameObject);
			
			if (other.gameObject.TryGetComponent(out IDamageable damageable))
			{
				damageable.TakeDamage(99, DamageSource.Enemy);
			}
		}
		
		//Debug.Log($"Enemy collided with {other.gameObject.name}", gameObject);
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, attackRange);

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, detectionRange);
	}

	public void TakeDamage(float damage, DamageSource source)
	{
		health -= damage;

		if (source == DamageSource.Player) detectionRange = distanceToPlayer + 5; // TODO: reset detection range if you run away

		ParticleSystem instance = Instantiate(hurtVFX, transform.position, Quaternion.Inverse(transform.rotation));

		if (!IsDead)
		{
			transform.DOShakePosition(0.2f, 0.5f).SetLink(gameObject);
			
			var renderers = GetComponentsInChildren<Renderer>();
			if (renderers is { Length: > 0 })
			{
			    foreach (var r in renderers)
			    {
			        var mat = r.material;
			        if (mat != null && mat.HasColor("_BaseColor"))
			        {
			            mat.DOColor(Color.red, "_BaseColor", 0.1f)
			               .OnComplete(() => mat.DOColor(Color.white, "_BaseColor", 0.1f))
			               .SetLink(gameObject);
			        }
			    }
			}
		}

		hurtSFX.Play();

		if (target.TryGetComponent(out PlayerController player) && player?.Weapon.EquippedWeapon == Weapon.Weapons.Dagger && source == DamageSource.Player) daggerHurtSFX.Play();

		//Logger.Log("Enemy took: " + damage + " damage.", this, $"{name}");

		if (health <= 0) Death();
	}

	public void ResetToStart()
	{
		if (agent)
		{
			agent.Warp(startPos);
			transform.rotation = startRot;
			agent.ResetPath();
		}
	}

	IEnumerator Shriek()
	{
		while (health > 0 && Vector3.Distance(transform.position, target.position) <= detectionRange)
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

				default:
					throw new ArgumentOutOfRangeException();
			}
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
				bansheeSFX.SetRandomPitch(new (0.95f, 1.05f));
				bansheeSFX.Play();
				break;
		}

		yield return new WaitForSeconds(animDuration / 3f);

		if (IsDead) yield break;

		target.TryGetComponent(out IDamageable damageable);

        float newDistanceToPlayer= Vector3.Distance(transform.position, target.position);
        if (newDistanceToPlayer <= distanceToPlayer)
        {
            damageable.TakeDamage(damage);
        }
	}
	
	public bool IsDead => health <= 0;
	
	void Death()
	{
		//Logger.LogWarning("Enemy died.", this, $"{name}");
		room.Unregister(this);

		snarlSFX.Stop();
		
		col.enabled = false;
		
		transform.DOScaleY(0, 1.5f).SetLink(gameObject);
		
		var index = type == EnemyType.Octopus ? 0 : 1;
		var renderers = transform.GetChild(index).GetComponentsInChildren<Renderer>();
		var deathMaterial = type == EnemyType.Octopus ? octopusDeathMaterial : watcherDeathMaterial;
		
		foreach (var rend in renderers)
		{
		    var mats = rend.materials;
		    for (int i = 0; i < mats.Length; i++)
		    {
		        mats[i] = deathMaterial;
		    }
		    rend.materials = mats;
		}
		
		Destroy(gameObject, 1.5f);
	}

	public void Knockback(float knockbackForce)
	{
		rb.isKinematic = false;

		// knock backwards, opposite to where the enemy is moving
		rb.AddForce(-agent.velocity.normalized * knockbackForce, ForceMode.Impulse);

		Stagger(staggerDuration);
	}

	public void Knockback(Vector3 direction, float knockbackForce)
	{
		rb.isKinematic = false;

		// knock in specified direction
		rb.AddForce(direction.normalized * knockbackForce, ForceMode.Impulse);

		Stagger(staggerDuration);
	}

	public void Stagger(float duration = -1) => StartCoroutine(PerformStagger(duration));

	IEnumerator PerformStagger(float duration)
	{
		if (duration < 0) duration = staggerDuration;

		agent.isStopped = true;
		CancelAttackAnimation();
        yield return new WaitForSeconds(duration);
		agent.isStopped = false;

		rb.isKinematic = true;
	}

	void CancelAttackAnimation()
	{
        StopCoroutine(nameof(Attack));
        animator.ResetTrigger("attack");
        animator.SetBool("isMoving", false);
        animator.CrossFade("Idle", 0.1f, 0, 0f);
    }

    //--------------------------------- respawn mechanics n stuff----------------------------------------

    void OnEnable() => room?.Register(this);

	void OnDisable() => room?.Unregister(this);

    public void Interact()
    {
		
    }
}
