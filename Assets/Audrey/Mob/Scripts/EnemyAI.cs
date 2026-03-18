using Audrey.Player.Script;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float health;
    public float damage;
    public LayerMask enemyLayerMask;
    public LayerMask buildingLayerMask;

    [SerializeField] private float rangeOfAttack = 1.0f;
    [SerializeField] private float rangeOfSight = 3.0f;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private float pathCheckInterval = 0.4f;

    private NavMeshAgent agent;
    private GameObject player;
    private GameObject base_;
    private GameObject currentTarget;
    private GameObject blockingBuilding;

    private bool canAttack = true;
    private float pathCheckTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        player = GameObject.Find("Player");
        base_ = GameObject.Find("Base");

        SetTarget(base_);
    }

    void Update()
    {
        if (currentTarget == null) { SetTarget(base_); return; }

        pathCheckTimer -= Time.deltaTime;
        if (pathCheckTimer <= 0f)
        {
            pathCheckTimer = pathCheckInterval;
            ResolveTarget();
        }

        if (!agent.pathPending && agent.remainingDistance <= rangeOfAttack)
            Attack();
    }

    // Called every pathCheckInterval — decides who to target
    private void ResolveTarget()
    {
        // 1. Can we see the player?
        GameObject idealTarget = CanSeePlayer() ? player : base_;

        // 2. Is the path to the ideal target blocked by a building?
        GameObject blocker = GetBlockingBuilding(idealTarget);

        if (blocker != null)
        {
            // Attack the building blocking the way
            blockingBuilding = blocker;
            SetTarget(blockingBuilding);
        }
        else
        {
            // Path is clear — go for ideal target
            blockingBuilding = null;
            SetTarget(idealTarget);
        }
    }

    private bool CanSeePlayer()
    {
        if (player == null) return false;
        float dist = Vector2.Distance(transform.position, player.transform.position);
        if (dist > rangeOfSight) return false;

        // Raycast — check nothing blocks line of sight to player
        RaycastHit2D ray = Physics2D.Raycast(
            transform.position,
            (player.transform.position - transform.position).normalized,
            dist,
            buildingLayerMask
        );

        return ray.collider == null; // clear line of sight
    }

    private GameObject GetBlockingBuilding(GameObject target)
    {
        if (target == null) return null;

        Vector2 dir = (target.transform.position - transform.position).normalized;
        float dist = Vector2.Distance(transform.position, target.transform.position);

        RaycastHit2D hit = Physics2D.BoxCast(
            transform.position,
            new Vector2(0.8f, 0.8f),
            0f,
            dir,
            dist,
            buildingLayerMask
        );

        if (hit.collider != null)
        {
            BuildingHealth bHealth = hit.collider.GetComponent<BuildingHealth>()
                                     ?? hit.collider.GetComponentInParent<BuildingHealth>()
                                     ?? hit.collider.GetComponentInChildren<BuildingHealth>();
            if (bHealth != null) return bHealth.gameObject;
        }

        return null;
    }


    private void SetTarget(GameObject newTarget)
    {
        if (newTarget == null || newTarget == currentTarget) return;
        currentTarget = newTarget;
        agent.SetDestination(currentTarget.transform.position);
    }

    private void Attack()
    {
        if (!canAttack) return;

        Vector2 dir = (currentTarget.transform.position - transform.position).normalized;

        // Attack player
        RaycastHit2D playerHit = Physics2D.Raycast(transform.position, dir, rangeOfAttack, enemyLayerMask);
        if (playerHit.collider != null)
        {
            PlayerHealth pHealth = playerHit.collider.GetComponent<PlayerHealth>();
            if (pHealth != null) pHealth.ChangeHealth(-damage);
        }

        // Attack building
        RaycastHit2D buildingHit = Physics2D.Raycast(transform.position, dir, rangeOfAttack, buildingLayerMask);
        if (buildingHit.collider != null)
        {
            BuildingHealth bHealth = buildingHit.collider.GetComponent<BuildingHealth>()
                                  ?? buildingHit.collider.GetComponentInParent<BuildingHealth>()
                                  ?? buildingHit.collider.GetComponentInChildren<BuildingHealth>();
            if (bHealth != null) bHealth.TakeDamage(damage);
        }

        canAttack = false;
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    void ResetAttack() => canAttack = true;

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            EnemySpawner.Instance.EnemyDied();
            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangeOfSight);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangeOfAttack);
    }
}
