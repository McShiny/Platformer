using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    
    public static GameInput Instance { get; private set; }

    public event EventHandler OnJumpPreformed;
    public event EventHandler OnDashPreformed;

    private PlayerInputActions playerInputActions;

    private void Awake() {
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Jump.performed += Jump_performed;
        playerInputActions.Player.Dash.performed += Dash_performed;
    }

    private void Dash_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnDashPreformed?.Invoke(this, EventArgs.Empty);
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnJumpPreformed?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy() {
        playerInputActions.Dispose();
    }

    public float GetMovementVectorNormalized() {
        float inputVector = playerInputActions.Player.Move.ReadValue<float>();

        return inputVector;
    }

    public bool GetJumpDown() {
        return playerInputActions.Player.Jump.ReadValue<float>() > 0.5f;
    }

}
