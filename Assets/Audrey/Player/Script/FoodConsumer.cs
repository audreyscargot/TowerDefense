using UnityEngine;
using UnityEngine.InputSystem;

public class FoodConsumer : MonoBehaviour
{
    [Header("Settings")]
    public InputActionReference consumeFoodAction;

    private PlayerFood food;

    private void Awake()
    {
        food = GetComponent<PlayerFood>();
    }

    private void OnEnable()
    {
        consumeFoodAction.action.Enable();
        consumeFoodAction.action.performed += OnConsumeFood;
    }

    private void OnDisable()
    {
        consumeFoodAction.action.performed -= OnConsumeFood;
        consumeFoodAction.action.Disable();
    }

    private void OnConsumeFood(InputAction.CallbackContext context)
    {
        food.ConsumeFood();
        Debug.Log("Consume Food");
    }
}