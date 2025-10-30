using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Characters")]
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _enemyPrefab;

    [Header("Marker Materials")]
    [SerializeField] private Material _hallwayMaterial;
    [SerializeField] private Material _impRoomMaterial;
    [SerializeField] private Material _daggerRoomMaterial;
    [SerializeField] private Material _fuseRoomMaterial;
    [SerializeField] private Material _keyRoomMaterial;
    [SerializeField] private Material _enemyRoomMaterial;
    [SerializeField] private Material _playerSpawnRoomMaterial;
    [SerializeField] private Material _playerWinRoomMaterial;
    [SerializeField] private Material _normalRoomMaterial;

    [Header("Props")]
    [SerializeField] private int _TableCountMultiplier = 2;
    [SerializeField] private GameObject _tablePrefab;
    [SerializeField] private int _LightsCountMultiplier = 3;
    [SerializeField] private GameObject _lightPrefab;

    private List<MazeCell> _keyRoomsCollections; //Assigned
    private MazeCell _playerSpawnRoom; //Assigned
    private MazeCell _playerWinRoom; //Assigned
    private MazeCell _enemySpawnRoom;
    private MazeCell _daggerRoom;
    private MazeCell _fuseRoom;
    private MazeCell _keyRoom;
    private MazeCell _startCell;
    private int _mazeSize = 0;


    private bool _isMapGenerated = false;
    private bool _isReady = false;
    private bool _isDaggerPicked = false;
    private bool _isItemSpawned = false;

    MazeCell[,] _mazeGrid;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _keyRoomsCollections = new List<MazeCell>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isMapGenerated && !_isReady) {
            return;
        }
        if (_isDaggerPicked && !_isItemSpawned) {
            //Trigger Enemy Spawn
            // Trigger Fuse and Key Item Spawn
        }
    }
    public void SetMapGenerated(bool value , MazeCell[,] maze, int size) {
        _isMapGenerated = value;
        _mazeGrid = maze;
        _mazeSize = size;
        if (_mazeGrid !=null) {
            Debug.Log($"GameManager Received The maze of size - {_mazeSize} * {_mazeSize} ");
            _startCell = _mazeGrid[0, 0];
            _keyRoomsCollections = _keyRoomsCollections.GroupBy(c => new { c.x, c.y }).Select(g => g.First()).ToList();
            foreach (MazeCell cell in _mazeGrid) {
                if (cell.GetCellType() == "PlayerSpawnRoom") {
                    cell.SetFloorMaterial(_playerSpawnRoomMaterial);
                    _playerSpawnRoom = cell;
                    // Spawn Player

                } else if (cell.GetCellType() == "PlayerWinRoom") {
                    cell.SetFloorMaterial(_playerWinRoomMaterial);
                    _playerWinRoom = cell;
                    //Spawn Win Trigger Here
                } else if (cell.GetCellType() == "PlayerWinRoom") {
                    cell.SetFloorMaterial(_enemyRoomMaterial);
                    _enemySpawnRoom = cell;
                } else if (cell.GetCellType() == "ImpRoom") {
                    // Collect key rooms
                    cell.SetFloorMaterial(_impRoomMaterial);


                } else if (cell.GetCellType() == "Hallway") {
                    cell.SetFloorMaterial(_hallwayMaterial);
                } else if (cell.GetCellType() == "NormalRoom") {
                    cell.SetFloorMaterial(_normalRoomMaterial);
                }
                //Debug.Log($"Cell Type : {cell.GetCellType()} Key Room Collection {_keyRoomsCollections.Count}");
            }
            //Debug.Log($"Key Rooms Assigned ({_keyRoomsCollections.Count})");
        }
        AssignKeyItems();
    }

    private void AssignKeyItems() {
        // Assign Dagger Room

        int _daggerRoomIndex = 1;
        int enemyRoomIndex = 0;
        _daggerRoom = _keyRoomsCollections[_daggerRoomIndex];
        _enemySpawnRoom = _keyRoomsCollections[enemyRoomIndex];

        Debug.Log($"Collection before Assigning DaggerRoom and enemy : {_keyRoomsCollections.Count}");
        Debug.Log($"Dagger Room Selected at Index : {_daggerRoomIndex} ({_daggerRoom.x}, {_daggerRoom.y})");
        Debug.Log($"Enemy Spawn Room Selected at Index : {enemyRoomIndex} ({_enemySpawnRoom.x}, {_enemySpawnRoom.y})");

        _keyRoomsCollections.Remove(_daggerRoom);
        _keyRoomsCollections.Remove(_enemySpawnRoom);

        Debug.Log($"Collection after Assigning DaggerRoom and Enemy Room : {_keyRoomsCollections.Count}");
        _daggerRoom.SetCellType("DaggerRoom");
        _daggerRoom.SetFloorMaterial(_daggerRoomMaterial);
        Debug.Log($"Dagger Room Assigned at ({_daggerRoom.x}, {_daggerRoom.y}) Key Room Collection Count : {_keyRoomsCollections.Count}");

        _enemySpawnRoom.SetCellType("EnemyRoom");
        _enemySpawnRoom.SetFloorMaterial(_enemyRoomMaterial);
        Debug.Log($"Enemy Spawn Room Assigned at ({_enemySpawnRoom.x}, {_enemySpawnRoom.y}) Key Room Collection Count : {_keyRoomsCollections.Count}");
        // Spawn Dagger and Enemy Here

        int _fuseRoomIndex = RandomNumberGenerator(_keyRoomsCollections.Count);
        _fuseRoom = _keyRoomsCollections[_fuseRoomIndex];
        _keyRoomsCollections.Remove(_fuseRoom);
        _fuseRoom.SetCellType("FuseRoom");
        _fuseRoom.SetFloorMaterial(_fuseRoomMaterial);
        Debug.Log($"Fuse Room Assigned at ({_fuseRoom.x}, {_fuseRoom.y}) Key Room Collection Count : {_keyRoomsCollections.Count}");

        int _keyRoomIndex = RandomNumberGenerator(_keyRoomsCollections.Count);
        _keyRoom = _keyRoomsCollections[_keyRoomIndex];
        _keyRoomsCollections.Remove(_keyRoom);
        _keyRoom.SetCellType("KeyRoom");
        _keyRoom.SetFloorMaterial(_keyRoomMaterial);
        Debug.Log($"Key Room Assigned at ({_keyRoom.x}, {_keyRoom.y}) Key Room Collection Count : {_keyRoomsCollections.Count}");

        AssignLightsAndTables();
    }

    private void AssignLightsAndTables() {

    }

    private int RandomNumberGenerator(int maxValue) {
        return Random.Range(0,maxValue);
    }

    public void AddImpRoomToCollection(MazeCell cell) {
        _keyRoomsCollections.Add(cell);
    }

    public void TriggerDaggerPicked() {
        _isDaggerPicked = true;
    }
}
