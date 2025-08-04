using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public enum EnemyBehaviorType
{
    Melee,
    Ranged,
    Boss
}

public class EnemyController : BaseController
{
    [Header("Stats")]
    [SerializeField] private EnemyStats stats;
    public EnemyStats Stats { get { return stats; } }

    [Header("Enemy Type")]
    [SerializeField] private EnemyBehaviorType behaviorType; // Defines enemy behavior

    [Header("Ranged Attack")]
    [SerializeField] private GameObject arrowPrefab;

    private Transform target;
    private EnemyAnimationHandler _animation;
    private IEnemyAttack EnemyAttack;

    private int originalLayer;

    bool isDead = false;
    public bool IsDead => isDead;
    public GameObject ArrowPrefab => arrowPrefab;

    // Initialization
    protected override void Awake()
    {
        base.Awake();

        originalLayer = gameObject.layer;
        moveSpeed = stats.moveSpeed;

        switch (behaviorType) // Initialize behavior strategy
        {
            case EnemyBehaviorType.Melee:
                EnemyAttack = new MeleeEnemyAttack();
                break;
            case EnemyBehaviorType.Ranged:
                EnemyAttack = new RangedEnemyAttack();
                break;
            case EnemyBehaviorType.Boss:
                EnemyAttack = new BossEnemyAttack();
                break;
        }

        if (_animation == null)
            _animation = GetComponent<EnemyAnimationHandler>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected void OnEnable()
    {
        if (stats != null)
        {
            stats.StatInitialize(); // Reset stats
        }

        StartCoroutine(WaitForInit());

        _animation?.ResetState();
    }

    protected override void Update()
    {
        base.Update();

        _animation?.Move(moveDirection);

        if (behaviorType == EnemyBehaviorType.Boss && EnemyAttack is BossEnemyAttack boss)
        {
            boss.Update(this); // Special logic for bosses
        }
    }

    public void Init(Transform target) // Set target
    {
        this.target = target;
    }

    // Calculate distance to player
    protected float DistanceToTarget()
    {
        if (target == null) return float.MaxValue;
        return Vector2.Distance(transform.position, target.position);
    }

    // Get normalized direction vector to player
    public Vector2 DirectionToTarget()
    {
        if (target == null) return Vector2.zero;
        return (target.position - transform.position).normalized;
    }

    // Check if player is within attack range
    public bool IsInAttackRange()
    {
        if (target == null) return false;
        return DistanceToTarget() <= stats.attackRange;
    }

    // AI movement and attack logic
    protected override void HandleInput()
    {
        if (target == null || stats.healthPoint <= 0)
        {
            moveDirection = Vector2.zero;
            lookDirection = Vector2.zero;
            return;
        }

        float distance = DistanceToTarget();
        Vector2 direction = DirectionToTarget();

        lookDirection = direction;

        if (distance <= stats.detectRange)
        {
            float optimalDist = stats.OptimalDistance;

            if (distance > optimalDist + stats.distanceTolerance)
            {
                moveDirection = direction; // Move closer
            }
            else
            {
                moveDirection = Vector2.zero; // Stop moving
            }
        }
        else
        {
            moveDirection = Vector2.zero; // Idle if out of detection range
        }

        if (IsInAttackRange() && EnemyAttack.CanAttack(this))
        {
            EnemyAttack.Attack(this);
        }

        bool inRange = IsInAttackRange();
        bool canAttack = EnemyAttack.CanAttack(this);

        Debug.Log($"Distance: {distance:F2}, Range: {stats.attackRange}, InRange: {inRange}, CanAttack: {canAttack}");

        if (inRange && canAttack)
        {
            Debug.Log("Attack conditions met! Calling Attack()");
            EnemyAttack.Attack(this);
        }
    }

    // Take damage and handle death
    public void GetDamage(float dmg)
    {
        isDead = stats.TakeDamage(dmg);

        if (isDead)
        {
            HandleDeath();
        }
        else
        {
            _animation.Damage();
        }
    }

    private void HandleDeath()
    {
        _animation.Death(); // Play death animation
        StartCoroutine(DeathCoroutine());
    }

    private IEnumerator WaitForInit()
    {
        yield return new WaitUntil(() => StageManager.Instance != null &&
            StageManager.Instance._Player != null);
        Init(StageManager.Instance._Player.transform);
    }

    private IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(0.4f);

        if (StageManager.Instance != null)
        {
            StageManager.Instance.RemoveMonsterFromList(gameObject);
        }

        EnemyPoolObject poolObject = GetComponent<EnemyPoolObject>();
        if (poolObject != null)
        {
            poolObject.ReturnToPool();
        }
        else
        {
            Debug.Log($"{gameObject.name}: No pool reference, destroying object.");
            Destroy(gameObject);
        }
    }

    // On contact with player
    public void ApplyContactDamage(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            float damage = stats.contactDamage;

            if (behaviorType == EnemyBehaviorType.Boss && EnemyAttack is BossEnemyAttack boss && boss.IsCharging())
            {
                damage = stats.contactDamage * 3f;
                Debug.Log("Charging attack! Damage: " + damage);
            }
            else
            {
                Debug.Log("Body slam! Damage: " + damage);
            }

            /*
            ResourceController rc = collider.GetComponent<ResourceController>();
            if (rc != null)
            {
                rc.ChangeHealth(-damage);
            }
            */

            Player player = collider.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage, this.gameObject);

                BaseController controller = player.GetComponent<BaseController>();
                if (controller != null)
                {
                    controller.ApplyKnockback(transform, 2f, 0.3f);
                }
            }
        }
    }

    // Reset values when returned to pool
    public void OnReturnToPool()
    {
        Animator animator = GetComponentInChildren<Animator>();

        if (animator == null)
            Debug.LogWarning("Enemy's animator is NULL!");

        EnemyAnimationHandler animationHandler = GetComponent<EnemyAnimationHandler>();
        if (animationHandler != null)
        {
            animationHandler.ResetState();
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} is missing EnemyAnimationHandler.");
        }

        stats.healthPoint = stats.maxHealth;
        isDead = false;

        gameObject.layer = originalLayer;

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = true;
    }
}
