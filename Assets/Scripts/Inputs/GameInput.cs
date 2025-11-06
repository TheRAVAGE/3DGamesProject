using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private PlayerInputActions _playerInputActions;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();
        Debug.Log($"Move Input: {_playerInputActions.Player.Move.bindings}");

    }

    public Vector2 MovementNormalized() {
        return _playerInputActions.Player.Move.ReadValue<Vector2>().normalized;
    }

    public float RunningInput() {
        return _playerInputActions.Player.Run.ReadValue<float>();
    }

    public Vector2 LookInput() {
        return _playerInputActions.Player.Look.ReadValue<Vector2>();
    }

    public float CrouchInput() {
        return _playerInputActions.Player.Crouch.ReadValue<float>();
    }
    public float InteractInput() {
        return _playerInputActions.Player.Interact.ReadValue<float>();
    }

}
