using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    private Rigidbody2D m_rigidBody;
    private Vector2 m_moveInput;
    [SerializeField] private float rotationSpeed;
    
    //Attack related variables
    public bool canAttack = true;
    [SerializeField] private float attackRange = 2.0f;
    public LayerMask layerMask;

    public float attackCooldown = 0.5f;
    public float damage = 2.0f;

    private Animator anim;
    private Vector2 lastMoveDirection;

    public GameObject rotateForAim;
    
    public Camera mainCam;
    public InputActionReference pointerPositionAction;
    private Vector2 currentMouseScreenPos;

    public AudioSource weapon;
    
    
    void Start()
    {
        anim = GetComponent<Animator>();
        m_rigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        ProcessInput();
        Animate();
        Test2();
        if (pointerPositionAction != null)
            currentMouseScreenPos = pointerPositionAction.action.ReadValue<Vector2>();
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

    //Make Rotation for linetrace aim
    private void RotateInDirection()
    {
        Vector2 mouseWorldPos = mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 mouseFixedPos = new Vector2(mouseWorldPos.x, -mouseWorldPos.y);
        
        if (mouseFixedPos != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation( mouseFixedPos, transform.up);
            Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            
            rotateForAim.transform.SetPositionAndRotation(transform.position, rotation);
        }  
    }

    //Functions for sprite animation
    void ProcessInput()
    {
        m_moveInput.Normalize();
        if ((m_moveInput.x != 0 || m_moveInput.y != 0))
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
            anim.SetTrigger("Attack");
            if(weapon) weapon.Play();
            Vector2 mouseWorldPos = mainCam.ScreenToWorldPoint(currentMouseScreenPos);
            Vector2 newTemp = new Vector2(transform.position.x, transform.position.y);
            CapsuleDirection2D dirTemp = new CapsuleDirection2D();
            RaycastHit2D hit = Physics2D.Linecast(transform.position, mouseWorldPos, layerMask);
            
            // calcul distance between player to mouseworldpos
            if (hit.collider != null)
            {
                if (Vector2.Distance(transform.position, hit.collider.transform.position) <= attackRange)
                {
                    //Enemy
                    EnemyAI hitEnemy = hit ? hit.collider.gameObject.GetComponent<EnemyAI>() : null;
                    if (hitEnemy)
                    {
                        hitEnemy.TakeDamage(damage);
                
                    }
           
                    //Ressource
                    ResourceNode resourceNode = hit ? hit.collider.gameObject.GetComponent<ResourceNode>() : null;
                    if (resourceNode)
                    {
                        resourceNode.TakeDamage(damage);
                    }
                    canAttack =  false;
                    Invoke("ResetAttack", attackCooldown);
                } 
                }
            }
    }
    
    //DEBUG for attack
    void OnDrawGizmos()
    {
        if (mainCam != null)
        {
            Vector2 mouseWorldPos = mainCam.ScreenToWorldPoint(currentMouseScreenPos);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position,mouseWorldPos); 
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(mouseWorldPos, 0.1f);
            Gizmos.DrawSphere(transform.position, 0.1f);
        }
    }
    
    void ResetAttack()
    {
        canAttack = true;
    }

    public PlayerFood food;
    
    public void Test2()
    {
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            food.ConsumeFood();
            Debug.Log("Consume Food");
        }
    }
}
