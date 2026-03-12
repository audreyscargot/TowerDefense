using UnityEngine;

public class Enemy : MonoBehaviour
{
    //home base récupérer le transform
    private Vector2 m_baseLocation;
    private Rigidbody2D m_rb;
    private Transform m_target;
    [SerializeField] private float m_range = 10f;
    Vector2 moveDirection;
    private bool m_canAttack = true;
    
    public float moveSpeed = 2f;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_target = GameObject.Find("Base").transform;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToTarget = Vector2.Distance(m_target.position, transform.position);
        if (m_target && distanceToTarget >= m_range)
        {
            moveDirection = (m_target.position - transform.position).normalized;
        }
        else
        {
            moveDirection = Vector2.zero;
            Attack();
        }
        Debug.Log(moveDirection);
    }

    private void Attack()
    {
        if (m_canAttack)
        {
            
        }
    }
    
    private void FixedUpdate()
    {
        if(m_target)
        {
            m_rb.linearVelocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
        }
    }
}
