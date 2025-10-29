using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private bool _isInputEnabled;

    [Header("Movement")]
    [SerializeField] private float _walkMaxSpeed = 11f;
    [SerializeField] private float _runMaxSpeed = 20f;
    [SerializeField] private float _acceleration = 15f;

    [Header("Mouse")]
    [SerializeField] private float _mouseSensitivity = 50f;

    [Header("Node")]
    [SerializeField] private NodeController _nodePrefab;
    [SerializeField] private GameObject _nodeSpawner;
    [SerializeField] private float _nodeGenerationDistance = 5f;
    [SerializeField] private float _nodeGenerationInterval = 2f;


    private string _currentMovementState;
    private float _currentSpeed;
    private float _currentMaxSpeed;
    private float xRotation = 0f;
    private float _nextNodeGenTime = 0f;
    private float _distanceToLastNode;

    //Control Variables
    Vector2 inputVector;
    float runningInput;
    Vector2 lookInput;
    float crouchInput;

    Vector3 _lastNodeGenPosition;

    private bool _isIdle;
    private bool _isHiding;

    Rigidbody _rigidbody;
    GameInput _gameInput;
    Camera _mainCamera;
    Collider _playerCollider;

    Flashlight _playerFlashlight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentSpeed = 0;
        _lastNodeGenPosition = _nodeSpawner.transform.position;

        //INITIALIZATIONS
        _gameInput = GameInput.Instance;
        _rigidbody = GetComponent<Rigidbody>();
        _mainCamera = GetComponentInChildren<Camera>();
        _playerCollider = GetComponent<BoxCollider>();

        _playerFlashlight = GetComponentInChildren<Flashlight>();

        //NULL CHECKS
        if (_gameInput == null) {Debug.LogError("GameInput instance is null in PlayerController.");}

        if (_rigidbody == null) { Debug.LogError("Rigidbody component is missing in PlayerController.");}

        if (_mainCamera == null) { Debug.LogError("Main Camera is missing in PlayerController."); }

        if (_playerCollider == null) { Debug.LogError("Player Collider is missing in PlayerController."); }

        if (_playerFlashlight == null) { Debug.LogError("Player Flashlight is missing in PlayerController."); }

        //LOCK AND HIDE CURSOR
        Cursor.lockState = CursorLockMode.Locked;

        //Camera Initial State
        if (_isInputEnabled) {
            _mainCamera.gameObject.SetActive(true);
            Debug.Log("PlayerController Activated With Input");
        } else {
            _mainCamera.gameObject.SetActive(false);
            Debug.Log("PlayerController Activated");
        }
    }

    // Update is called once per frame
    void Update()
    {
        NodeGenerator();
        SpeedHandler(_currentMaxSpeed);
        ManageInputs();
    }

    //NODE GENERATOR
    private void NodeGenerator() {
        if (_nodePrefab == null) {
            Debug.LogError("Node Prefab is not assigned in PlayerController.");
            return;
        }else if (_nodeSpawner == null) {
            Debug.LogError("Node Spawner is not assigned in PlayerController.");
            return;
        }
        else {
            _distanceToLastNode = Vector3.Distance(_nodeSpawner.transform.position, _lastNodeGenPosition);
            if (Time.time >= _nextNodeGenTime && _distanceToLastNode >= _nodeGenerationDistance) {
                Vector3 _nodeGenPositon = _nodeSpawner.transform.position;
                Instantiate(_nodePrefab, _nodeGenPositon, Quaternion.identity);
                _nextNodeGenTime = Time.time + _nodeGenerationInterval;
                Vector3 _lastNodeGenPosition = _nodeGenPositon;
            }
        }
    }

    //MANAGE INPUTS
    private void ManageInputs()
    {
        if (_isInputEnabled) {
            inputVector = _gameInput.MovementNormalized();
            runningInput = _gameInput.RunningInput();
            lookInput = _gameInput.LookInput();
            crouchInput = _gameInput.CrouchInput();
        } else {
            return;
        }
        HandleInputs();
    }

    //HANDLE INPUTS
    private void HandleInputs() {
        ApplyLook(lookInput);
        if (crouchInput > 0) {
            _currentMovementState = "Crouching";
            transform.localScale = new Vector3 (0.0f,0.4f,0.0f);
            _currentMaxSpeed = _walkMaxSpeed / 2;
            ApplyMovement(inputVector);
            return;
        } else {
            transform.localScale = new Vector3(0.0f, 1f, 0.0f);
        }

        if (inputVector != Vector2.zero) {
            if (_isIdle) {
                _isIdle = false;
            }
            if (runningInput > 0) {
                _currentMovementState = "Running";
                _currentMaxSpeed = _runMaxSpeed;
            } else {
                _currentMovementState = "Walking";
                _currentMaxSpeed = _walkMaxSpeed;
            }
            ApplyMovement(inputVector);
        } else if (!_isIdle) {
            _currentMovementState = "Idle";
            _isIdle = true;
            _currentMaxSpeed = 0;

        }
        //Debug.Log($"Is Idle: {_isIdle}");
    }

    //Speed
    void SpeedHandler(float _currentMaxSpeed) {
        float accelerationStep = _acceleration * Time.deltaTime;
        if ( Mathf.Abs(_currentSpeed - _currentMaxSpeed) <= 0.1f) {
            _currentSpeed = _currentMaxSpeed;
        }
        else if (_currentSpeed >= _currentMaxSpeed) {
            _currentSpeed = Mathf.Max((_currentSpeed - (accelerationStep)), _currentMaxSpeed);

        } else if (_currentSpeed <= _currentMaxSpeed) {
            _currentSpeed = Mathf.Min((_currentSpeed + (accelerationStep)), _currentMaxSpeed);

        } else if ((_currentSpeed + accelerationStep) != _currentMaxSpeed) {
            _currentSpeed = _currentMaxSpeed;
        }
        //Debug.Log($"Current Speed: {_currentSpeed}, Current Max Speed: {_currentMaxSpeed}");
    }

    //Look Around
    private void ApplyLook( Vector2 lookInput) {
        float mouseX = lookInput.x * _mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * _mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -30f, 30f);

        _mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        if (_playerFlashlight != null) { _playerFlashlight.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); }

        transform.Rotate(Vector3.up * mouseX);


    }

    //Movement
    private void ApplyMovement(Vector2 inputVector) {
        Vector3 moveDirection = (transform.forward * inputVector.y + transform.right * inputVector.x).normalized;
        _rigidbody.MovePosition(transform.position + moveDirection * _currentSpeed * Time.deltaTime);
    }

    //SETTERS AND GETTERS
}
