using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines.Interpolators;

public class NodeController : MonoBehaviour
{
    public static NodeController Instance { get; private set; }

    [Header("Node Settings")]
    [SerializeField] private float _tickRate = 1f;
    [SerializeField] private float _tickAmount = 1.0f;
    [SerializeField] private float _reward = 100.0f;

    private float _SpawnedTime;
    private float _nextTickTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _SpawnedTime = Time.time;
        _nextTickTime = _SpawnedTime + _tickRate;
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= _nextTickTime) {
            _nextTickTime += _tickRate;
            _reward -= _tickAmount;
        }

        if (_reward <= 0) {
            Destroy(this.gameObject);
        }

    }

    //Getters and Setters
    public float GetNodeReward() {
        return _reward;
    }
}
