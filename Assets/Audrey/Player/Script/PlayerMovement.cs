using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    private Rigidbody2D m_rigidBody;
    private Vector2 m_moveInput;
    
    
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
}
