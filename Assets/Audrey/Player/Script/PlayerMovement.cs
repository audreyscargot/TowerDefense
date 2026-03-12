using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    private Rigidbody2D m_rigidBody;
    private Vector2 m_moveInput;
    // public GameManager GameManager;
    // public float updateDistance = 1f;
    // private Vector2 lastPosition;
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
       // lastPosition = m_rigidBody.position;
    }
    
    
    void FixedUpdate()
    {
        m_rigidBody.linearVelocity = m_moveInput * moveSpeed;
        // float distance = Vector2.Distance(lastPosition, m_rigidBody.position);
        // if (distance >= updateDistance)
        // {
        //     lastPosition = m_rigidBody.position;
        //    GameManager.PlayerMoved();
        // }
    }

    public void Move(InputAction.CallbackContext context)
    {
        m_moveInput = context.ReadValue<Vector2>();
    }
}
