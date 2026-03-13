using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    private Rigidbody2D m_rigidBody;
    private Vector2 m_moveInput;
    
    //Attack related variables
    private bool canAttack = true;
    [SerializeField] private float attackRange = 1.0f;
    private LayerMask layerMask;

    public float attackCooldown = 0.5f;
    public float damage = 1.0f;
    
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
    }
    
    
    void FixedUpdate()
    {
        m_rigidBody.linearVelocity = m_moveInput * moveSpeed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        m_moveInput = context.ReadValue<Vector2>();
    }

    //Attack function
    public void Attack(InputAction.CallbackContext context)
    {
        if (canAttack)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.forward, attackRange, layerMask);
            EnemyAI hitEnemy = hit.collider.gameObject.GetComponent<EnemyAI>();
            if (hitEnemy)
            {
                hitEnemy.TakeDamage(damage);
                canAttack =  false;
                Invoke("ResetAttack", attackCooldown);
            }
        }
    }

    void ResetAttack()
    {
        canAttack = true;
    }
}
