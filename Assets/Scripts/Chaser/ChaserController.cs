using UnityEngine;

public class ChaserController : MonoBehaviour {

    [Header("Inputs")]
    [SerializeField] private bool _isInputEnabled;

    [Header("Movement")]
    [SerializeField] private float _walkMaxSpeed = 10f;
    [SerializeField] private float _runMaxSpeed = 22f;
    [SerializeField] private float _acceleration = 15f;

    [Header("Mouse")]
    [SerializeField] private float _mouseSensitivity = 50f;

    private string _currentMovementState;
    private float _currentSpeed;
    private float _currentMaxSpeed;
    private float xRotation = 0f;

    //Control Variables
    Vector2 inputVector;
    float runningInput;
    Vector2 lookInput;

    private bool _isIdle;

    Rigidbody _rigidbody;
    GameInput _gameInput;
    Camera _mainCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        _currentSpeed = 0;
        //INITIALIZATIONS
        _gameInput = GameInput.Instance;
        _rigidbody = GetComponent<Rigidbody>();
        _mainCamera = GetComponentInChildren<Camera>();

        //NULL CHECKS
        if (_gameInput == null) { Debug.LogError("GameInput instance is null in PlayerController."); }

        if (_rigidbody == null) { Debug.LogError("Rigidbody component is missing in PlayerController."); }

        if (_mainCamera == null) { Debug.LogError("Main Camera is missing in PlayerController."); }

        //LOCK AND HIDE CURSOR
        Cursor.lockState = CursorLockMode.Locked;

        //Camera Initial State
        if (_isInputEnabled) {
            _mainCamera.gameObject.SetActive(true);
            Debug.Log("ChaserController Activated With Input");
        } else {
            _mainCamera.gameObject.SetActive(false);
            Debug.Log("ChaserController Activated");
        }
    }

    // Update is called once per frame
    void Update() {

        SpeedHandler(_currentMaxSpeed);
        ManageInputs();
    }

    //MANAGE INPUTS
    private void ManageInputs() {
        if (_isInputEnabled) {
            inputVector = _gameInput.MovementNormalized();
            runningInput = _gameInput.RunningInput();
            lookInput = _gameInput.LookInput();
        } else {
            return;
        }
        HandleInputs();
    }

    //HANDLE INPUTS
    private void HandleInputs() {
        ApplyLook(lookInput);
        if (inputVector != Vector2.zero) {
            if (_isIdle) {
                _isIdle = false;
            }
            //TODO: Make Game Manager control the max speed based on chase state
            _currentMaxSpeed = _walkMaxSpeed;
            ApplyMovement(inputVector);
        } else if (!_isIdle) {
            _currentMovementState = "Idle";
            _isIdle = true;
            _currentMaxSpeed = 0;

        }
        //Debug.Log($"Is Idle: {isIdle}");
    }

    //Speed
    void SpeedHandler(float _currentMaxSpeed) {
        float accelerationStep = _acceleration * Time.deltaTime; ;
        if (Mathf.Abs(_currentSpeed - _currentMaxSpeed) <= 0.1f) {
            _currentSpeed = _currentMaxSpeed;
        } else if (_currentSpeed >= _currentMaxSpeed) {
            _currentSpeed = Mathf.Max((_currentSpeed - (accelerationStep)), _currentMaxSpeed);

        } else if (_currentSpeed <= _currentMaxSpeed) {
            _currentSpeed = Mathf.Min((_currentSpeed + (accelerationStep)), _currentMaxSpeed);

        } else if ((_currentSpeed + accelerationStep) != _currentMaxSpeed) {
            _currentSpeed = _currentMaxSpeed;
        }
        //Debug.Log($"Current Speed: {_currentSpeed}, Current Max Speed: {_currentMaxSpeed}");
    }

    //Look Around
    private void ApplyLook(Vector2 lookInput) {
        float mouseX = lookInput.x * _mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * _mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -60f, 60f);

        _mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);


    }

    //Movement
    private void ApplyMovement(Vector2 inputVector) {
        Vector3 moveDirection = (transform.forward * inputVector.y + transform.right * inputVector.x).normalized;
        _rigidbody.MovePosition(transform.position + moveDirection * _currentSpeed * Time.deltaTime);
    }

    //SETTERS AND GETTERS
}
