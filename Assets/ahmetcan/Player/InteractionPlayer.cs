using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionPlayer : MonoBehaviour
{
    public InputActionReference InteractAction;
    public Interactable InteractionObjScript;
    private void OnEnable()
    {
        InteractAction.action.performed += OnInteract;
        InteractAction.action.Enable();
    }

    private void OnDisable()
    {
        InteractAction.action.performed -= OnInteract;
        InteractAction.action.Disable();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        Interact();
    }

    public void Interact()
    {
        if (InteractionObjScript) InteractionObjScript.Action();
    }
}