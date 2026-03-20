using Audrey.Player.Script;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Stats")]
    public float health = 100f;
    public float damage = 10f;

    [Header("Detection")]
    public LayerMask enemyLayerMask;
    public LayerMask buildingLayerMask;
    [SerializeField] private float rangeOfAttack = 1.0f;
    [SerializeField] private float rangeOfSight = 5.0f;
    [SerializeField] private float moveSpeed = 3f;

    [Header("Timing")]
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private float pathRefreshInterval = 0.4f;

    [Header("Targets")]
    public LayerMask baseLayerMask;

    private Transform playerTransform;
    private GameObject base_;
    private GameObject currentTarget;

    private List<Vector2> currentPath = new List<Vector2>();
    private int pathIndex = 0;
    private bool canAttack = true;
    private float pathRefreshTimer = 0f;
    private Rigidbody2D rb;

    private enum State { GoToBase, ChasePlayer, AttackBuilding, AttackPlayer }
    private State state = State.GoToBase;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null) playerTransform = playerObj.transform;

        Collider2D baseCol = Physics2D.OverlapCircle(Vector2.zero, 9999f, baseLayerMask);
        if (baseCol != null) base_ = baseCol.gameObject;

        TransitionTo(State.GoToBase);
    }


    private void Update()
    {
        pathRefreshTimer -= Time.deltaTime;

        switch (state)
        {
            case State.GoToBase:       UpdateGoToBase();       break;
            case State.ChasePlayer:    UpdateChasePlayer();    break;
            case State.AttackBuilding: UpdateAttackBuilding(); break;
            case State.AttackPlayer:   UpdateAttackPlayer();   break;
        }
    }

    private void FixedUpdate()
    {
        if (state == State.AttackBuilding || state == State.AttackPlayer)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        FollowPath();
    }

    // ── States ───────────────────────────────────────────────

    private void UpdateGoToBase()
    {
        GameObject blocker = GetBlockingBuilding(base_);

        if (blocker != null)
        {
            if (IsInAttackRange(blocker)) { TransitionTo(State.AttackBuilding, blocker); return; }

            if (pathRefreshTimer <= 0f)
            {
                pathRefreshTimer = pathRefreshInterval;
                RequestPath(blocker);
            }
            return;
        }

        if (IsInAttackRange(base_)) { TransitionTo(State.AttackBuilding, base_); return; }

        if (CanSeePlayer()) { TransitionTo(State.ChasePlayer); return; }

        if (pathRefreshTimer <= 0f)
        {
            pathRefreshTimer = pathRefreshInterval;
            RequestPath(base_);
        }
    }

    private void UpdateChasePlayer()
    {
        if (playerTransform == null || !CanSeePlayer()) { TransitionTo(State.GoToBase); return; }

        GameObject blocker = GetBlockingBuilding(playerTransform.gameObject);

        if (blocker != null)
        {
            if (IsInAttackRange(blocker)) { TransitionTo(State.AttackBuilding, blocker); return; }

            if (pathRefreshTimer <= 0f)
            {
                pathRefreshTimer = pathRefreshInterval;
                RequestPath(blocker);
            }
            return;
        }

        if (IsInAttackRange(playerTransform.gameObject)) { TransitionTo(State.AttackPlayer); return; }

        if (pathRefreshTimer <= 0f)
        {
            pathRefreshTimer = pathRefreshInterval;
            RequestPath(playerTransform.gameObject);
        }
    }

    private void UpdateAttackBuilding()
    {
        if (currentTarget == null) { TransitionTo(State.GoToBase); return; }

        if (!IsInAttackRange(currentTarget))
        {
            TransitionTo(State.GoToBase);
            return;
        }

        Attack();
    }

    private void UpdateAttackPlayer()
    {
        if (playerTransform == null || !CanSeePlayer()) { TransitionTo(State.GoToBase); return; }
        if (!IsInAttackRange(playerTransform.gameObject)) { TransitionTo(State.ChasePlayer); return; }
        Attack();
    }


    private void RequestPath(GameObject target)
    {
        if (target == null || AStarGrid.Instance == null) return;
        currentTarget = target;

        List<Vector2> path = AStarGrid.Instance.FindPath(transform.position, target.transform.position);
        if (path != null && path.Count > 0)
        {
            currentPath = path;
            pathIndex = 0;
        }
        else
        {
            currentPath = new List<Vector2> { target.transform.position };
            pathIndex = 0;
        }
    }

    private void FollowPath()
    {
        if (currentPath == null || pathIndex >= currentPath.Count)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 target = currentPath[pathIndex];
        if (Vector2.Distance(transform.position, target) < 0.1f) { pathIndex++; return; }

        rb.linearVelocity = (target - (Vector2)transform.position).normalized * moveSpeed;
    }

    // ── Helpers ──────────────────────────────────────────────

    private void TransitionTo(State newState, GameObject target = null)
    {
        state = newState;
        pathRefreshTimer = 0f;
        currentPath.Clear();
        pathIndex = 0;
        if (target != null) currentTarget = target;
    }

    private bool IsInAttackRange(GameObject target)
    {
        if (target == null) return false;
        return Vector2.Distance(transform.position, target.transform.position) <= rangeOfAttack;
    }

    private bool CanSeePlayer()
    {
        if (playerTransform == null) return false;
        float dist = Vector2.Distance(transform.position, playerTransform.position);
        if (dist > rangeOfSight) return false;

        RaycastHit2D ray = Physics2D.Raycast(
            transform.position,
            (playerTransform.position - transform.position).normalized,
            dist, buildingLayerMask
        );
        return ray.collider == null;
    }

    private GameObject GetBlockingBuilding(GameObject target)
    {
        if (target == null) return null;
        Vector2 dir = (target.transform.position - transform.position).normalized;
        float dist = Vector2.Distance(transform.position, target.transform.position);

        RaycastHit2D hit = Physics2D.BoxCast(
            transform.position, new Vector2(0.8f, 0.8f),
            0f, dir, dist, buildingLayerMask
        );

        if (hit.collider == null) return null;

        // Don't treat the target itself as a blocker
        if (hit.collider.gameObject == target ||
            hit.collider.transform.IsChildOf(target.transform)) return null;

        BuildingHealth bh = hit.collider.GetComponent<BuildingHealth>()
                         ?? hit.collider.GetComponentInParent<BuildingHealth>()
                         ?? hit.collider.GetComponentInChildren<BuildingHealth>();

        return bh != null ? bh.gameObject : null;
    }

    private void Attack()
    {
        if (!canAttack || currentTarget == null) return;

        if (state == State.AttackPlayer)
        {
            Collider2D playerCol = Physics2D.OverlapCircle(transform.position, rangeOfAttack, enemyLayerMask);
            playerCol?.GetComponent<PlayerHealth>()?.ChangeHealth(-damage);
        }
        else
        {
            BuildingHealth bh = currentTarget.GetComponent<BuildingHealth>()
                                ?? currentTarget.GetComponentInParent<BuildingHealth>()
                                ?? currentTarget.GetComponentInChildren<BuildingHealth>();

            if (bh != null)
            {
                bh.TakeDamage(damage);
            }
            else
            {
                PlayerHealth ph = currentTarget.GetComponent<PlayerHealth>()
                                  ?? currentTarget.GetComponentInParent<PlayerHealth>()
                                  ?? currentTarget.GetComponentInChildren<PlayerHealth>();
                ph?.ChangeHealth(-damage);
            }
        }

        canAttack = false;
        Invoke(nameof(ResetAttack), attackCooldown);
    }


    private void ResetAttack() => canAttack = true;

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            EnemySpawner.Instance?.EnemyDied();
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangeOfSight);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangeOfAttack);
    }
}
