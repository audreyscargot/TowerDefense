using Audrey.Player.Script;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public GameObject target;
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

    private Object[] attackables;
    private Transform[] attackablesPos;
    
    void Start()
    {
        player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        
        DetectNewTarget("Base");
    }

    void Update()
    {
        if (agent.remainingDistance <= rangeOfAttack)
        {
            Attack();
        }
    }

    private string GetAttackables()
    {
        FindObjectsInactive inactives = new FindObjectsInactive();
        FindObjectsSortMode sortMode = FindObjectsSortMode.None;
        attackables = FindObjectsByType(typeof(PlayerHealth), inactives, sortMode);
        GameObject closestObject = null;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < attackables.Length; i++)
        {
            GameObject attackable = (GameObject)attackables[i];
            float dist = Vector3.Distance(attackable.transform.position, transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestObject = attackable;
            }
        }

        return closestObject.name;
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
                Debug.Log(pHealth ? pHealth.currentHealth : null);
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
        Debug.Log("Reset");
        canAttack = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter2D");
        if (collision.CompareTag("Attackable"))
        {
            DetectNewTarget(collision.name);
            Debug.Log("AAAAAAAAAAAAH" + collision.name);
        }
    }

    void DetectNewTarget(string newTarget)
    {
        GameObject found = GameObject.Find(newTarget);
        if (found == null)
        {
            Debug.LogWarning($"EnemyAI: Could not find target named '{newTarget}'");
            return;
        }
        target = found;
        if (agent.SetDestination(target.transform.position))
        {
            
        }
    }

    //Check if Target has reachable Path;
    bool CheckReachable()
    {
        NavMeshPath path = new NavMeshPath();
        if(agent.CalculatePath(target.transform.position, path) &&  path.status == NavMeshPathStatus.PathComplete)
        {
            agent.SetPath(path);
            return true;
        }
        return false;
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
