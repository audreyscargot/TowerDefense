using Audrey.Player.Script;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public GameObject target;
    public Transform targetTransform;
    public float health;
    
    public GameObject player;
    public float damage;

    public LayerMask enemyLayerMask;
    
    private NavMeshAgent agent;
    private bool canAttack = true;
    [SerializeField] private float rangeOfAttack = 1.0f;
    [SerializeField] private float rangeOfSight = 3.0f;

    private bool hasLineOfSight = false;

    private float attackCooldown = 1.0f;
    private float tick;
    
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        
        // player = GameObject.FindGameObjectWithTag("Player");
        DetectNewTarget("Base");
    }

    void Update()
    {
        if (agent.remainingDistance <= rangeOfAttack)
        {
            Attack();
        }
    }

    //Check if player is visible (if visible target change to player, else target is base
    private void FixedUpdate()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, player.transform.position - transform.position);
        hasLineOfSight = ray.collider != null;
        if (hasLineOfSight && (player.transform.position - transform.position).magnitude < rangeOfSight)
        {
            DetectNewTarget("Player");
            return;
        }
        DetectNewTarget("Base");
    }

    void Attack()
    {
        // Le code tout caca d'enzo parce qu'il veut pas mettre le player health 
        if (canAttack)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, target.transform.position - transform.position, rangeOfAttack, enemyLayerMask);

            if (hit.collider != null)
            {
                GameObject hitObject = hit.collider.gameObject;

                PlayerHealth pHealth = hitObject.GetComponent<PlayerHealth>();
                if (pHealth != null) pHealth.ChangeHealth(-damage);
                
                // Barrier barrier = hitObject.GetComponent<Barrier>();
                // if (barrier != null) barrier.TakeDamage(damage);
            }

            canAttack = false;
            Invoke("ResetAttack", attackCooldown);
        }
        
        //Le bon code
        
        // if (canAttack)
        // {
        //     RaycastHit2D ray = Physics2D.Raycast(transform.position, target.transform.position - transform.position, rangeOfAttack, enemyLayerMask);
        //     GameObject hit = ray.collider.gameObject;
        //     PlayerHealth hitHealth = hit ? hit.GetComponent<PlayerHealth>() : null;
        //     if (hitHealth != null)
        //     {
        //         hitHealth.ChangeHealth(-damage);
        //     }
        //     canAttack = false;
        //     Invoke("ResetAttack",attackCooldown);
        // }
    }

    void ResetAttack()
    {
        canAttack = true;
    }

    void DetectNewTarget(string newTarget)
    {
        target = GameObject.Find(newTarget);
        targetTransform = target.transform;
        agent.SetDestination(targetTransform.position);
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        canAttack = false;
        Invoke("ResetAttack", 1.0f);
        
        if (health <= 0)
        {
            EnemySpawner.Instance.EnemyDied(); 
            Destroy(gameObject);
        }
    }

}
