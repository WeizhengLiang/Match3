using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputReader : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction selectAction;
    private InputAction fireAction;

    // Ensure the Player Input component is set to fire C# events
    public event Action OnFire;

    // Property to get the selected position from input
    public Vector2 SelectedPosition => selectAction.ReadValue<Vector2>();

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        selectAction = playerInput.actions["Select"];
        fireAction = playerInput.actions["Fire"];

        fireAction.performed += HandleFireAction;
    }

    void OnDestroy()
    {
        fireAction.performed -= HandleFireAction;
    }

    // Handler for the fire action
    private void HandleFireAction(InputAction.CallbackContext context)
    {
        Debug.Log("Fire action performed");
        OnFire?.Invoke();
    }
}