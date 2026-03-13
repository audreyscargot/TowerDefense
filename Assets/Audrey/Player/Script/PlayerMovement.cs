using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    private Rigidbody2D m_rigidBody;
    private Vector2 m_moveInput;
    [SerializeField] private float rotationSpeed;
    
    //Attack related variables
    private bool canAttack = true;
    [SerializeField] private float attackRange = 2.0f;
    public LayerMask layerMask;

    public float attackCooldown = 0.5f;
    public float damage = 2.0f;
    
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
    }
    
    
    void FixedUpdate()
    {
        m_rigidBody.linearVelocity = m_moveInput * moveSpeed;
        RotateInDirection();
    }

    public void Move(InputAction.CallbackContext context)
    {
        m_moveInput = context.ReadValue<Vector2>();
    }

    private void RotateInDirection()
    {
        if (m_moveInput != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, m_moveInput);
            Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            
            m_rigidBody.MoveRotation(rotation);
        }
    }
    
    
    //Attack function
    public void Attack(InputAction.CallbackContext context)
    {
        if (canAttack)
        {
            // RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, attackRange, layerMask);
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.5f, transform.up, attackRange, layerMask);
            Debug.Log(hit ? hit.collider.gameObject : null);
            EnemyAI hitEnemy = hit ? hit.collider.gameObject.GetComponent<EnemyAI>() : null;
            if (hitEnemy)
            {
                hitEnemy.TakeDamage(damage);
                
            }
            canAttack =  false;
            Invoke("ResetAttack", attackCooldown);
        }
    }

    void ResetAttack()
    {
        canAttack = true;
    }
}
