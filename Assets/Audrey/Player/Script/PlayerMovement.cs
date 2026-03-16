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

    private Animator anim;
    private Vector2 lastMoveDirection;

    public GameObject rotateForAim;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        m_rigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        ProcessInput();
        Animate();
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
            
            rotateForAim.transform.SetPositionAndRotation(transform.position, rotation);
        }
    }

    void ProcessInput()
    {
        float moveX = m_moveInput.x;
        float moveY = m_moveInput.y;

        if ((moveX == 0 && moveY == 0) || (moveX != 0 && moveY != 0))
        {
            lastMoveDirection = m_moveInput;
        }
    }

    void Animate()
    {
        anim.SetFloat("MoveX", m_moveInput.x);
        anim.SetFloat("MoveY", m_moveInput.y);
        anim.SetFloat("MoveMagnitude", m_moveInput.magnitude);
        anim.SetFloat("lastMoveX", lastMoveDirection.x);
        anim.SetFloat("lastMoveY", lastMoveDirection.y);
    }
    
    //Attack function
    public void Attack(InputAction.CallbackContext context)
    {
        if (canAttack)
        {
            RaycastHit2D hit = Physics2D.CircleCast(rotateForAim.transform.position, 0.5f, rotateForAim.transform.up, attackRange, layerMask);
            EnemyAI hitEnemy = hit ? hit.collider.gameObject.GetComponent<EnemyAI>() : null;
            if (hitEnemy)
            {
                hitEnemy.TakeDamage(damage);
                
            }
            
            
            //à modifier avec une health en commun parce que c'est n'imp là
           ResourceNode resourceNode = hit ? hit.collider.gameObject.GetComponent<ResourceNode>() : null;
            if (resourceNode)
            {
                resourceNode.TakeDamage(damage);
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
