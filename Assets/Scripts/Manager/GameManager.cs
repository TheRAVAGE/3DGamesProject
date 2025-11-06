using System.Collections;
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
    [SerializeField] private int _TableCount = 200;
    [SerializeField] private GameObject _tablePrefab;
    [SerializeField] private int _LightsCount = 100;
    [SerializeField] private GameObject _lightPrefab;
    [SerializeField] private int _CrouchWallCount = 50;
    [SerializeField] private GameObject _crouchWallPrefab;

    private List<MazeCell> _keyRoomsCollections; //Assigned
    private List<MazeCell> _normalRoomsCollections;
    private List<MazeCell> _SelectedRoomsCollections;
    private MazeCell _playerSpawnRoom; //Assigned
    private MazeCell _playerWinRoom; //Assigned
    private MazeCell _enemySpawnRoom;
    private MazeCell _daggerRoom;
    private MazeCell _fuseRoom;
    private MazeCell _keyRoom;
    private MazeCell _startCell;
    private int _mazeSize = 0;


    WinPoint _winpoint;


    private bool _isMapGenerated = false;
    private bool _isReady = false;
    private bool _isDaggerPicked = false;
    private bool _isItemSpawned = false;
    private bool _isWinPointReady = false;

    MazeCell[,] _mazeGrid;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _keyRoomsCollections = new List<MazeCell>();
        _normalRoomsCollections = new List<MazeCell>();
        _SelectedRoomsCollections = new List<MazeCell>();
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

    public void GetMapGenerated(bool value, MazeCell[,] maze, int size)
    {
        StartCoroutine(SetMapGenerated(value, maze, size));
    }
    private IEnumerator SetMapGenerated(bool value , MazeCell[,] maze, int size) {
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
                    
                } else if (cell.GetCellType() == "ImpRoom") {
                    // Collect key rooms
                    cell.SetFloorMaterial(_impRoomMaterial);


                } else if (cell.GetCellType() == "Hallway") {
                    cell.SetFloorMaterial(_hallwayMaterial);
                } else if(cell.GetCellType() == "NormalRoom") {
                    _normalRoomsCollections.Add(cell);
                    cell.SetFloorMaterial(_normalRoomMaterial);
                }
                //Debug.Log($"Cell Type : {cell.GetCellType()} Key Room Collection {_keyRoomsCollections.Count}");
                yield return new WaitForEndOfFrame();
            }
            
        }
        
        Debug.Log($"Key Rooms Assigned ({_keyRoomsCollections.Count})");
        for (int i = _keyRoomsCollections.Count - 1; i >= 0; i--)
        {
            if (_keyRoomsCollections[i].x == _mazeSize-1 && _keyRoomsCollections[i].y == _mazeSize-1)
            {
                _keyRoomsCollections.Remove(_keyRoomsCollections[i]);
                Debug.Log($"KeyRoom Removed :{_keyRoomsCollections[i].x}_{_keyRoomsCollections[i].y}");
            }
            
            if (_keyRoomsCollections[i].GetCellType() != "ImpRoom")
            {
                Debug.Log($"KeyRoom Removed :{_keyRoomsCollections[i].x}_{_keyRoomsCollections[i].y}");
                _keyRoomsCollections.Remove(_keyRoomsCollections[i]);
            }
        }
        Debug.Log($"Normal Rooms Assigned ({_normalRoomsCollections.Count})");
        Debug.Log($"Key Rooms Assigned ({_keyRoomsCollections.Count})");
        StartCoroutine(AssignKeyItems());
    }

    private IEnumerator AssignKeyItems()
    {
        float StepWaitSeconds = 0.5f;
        
        _daggerRoom = GetRoomGenerate( 1, "DaggerRoom", _daggerRoomMaterial);
        yield return new WaitForSeconds(StepWaitSeconds);
        
        _enemySpawnRoom = GetRoomGenerate( 0, "EnemyRoom", _enemyRoomMaterial);
        yield return new WaitForSeconds(StepWaitSeconds);
        
        //-1 To Randomly Generate from the Imp rooms assigned
        _fuseRoom = GetRoomGenerate( -1, "FuseRoom", _fuseRoomMaterial);
        yield return new WaitForSeconds(StepWaitSeconds);
        
        _keyRoom = GetRoomGenerate( -1, "KeyRoom", _keyRoomMaterial);
        yield return new WaitForSeconds(StepWaitSeconds);

        PickCrouchWallRooms();
        StartCoroutine(DeployCrouchWall());
    }

    private void PickCrouchWallRooms()
    {
        Debug.Log("Picking Crouch Walls");
        Debug.Log($"PickCrouchWallRooms called. crouchCount={_CrouchWallCount}, normalRoomsCount={_normalRoomsCollections?.Count}");
        for (int i = 0; i < _CrouchWallCount; i++)
        {
            int _roomIndex = Random.Range(0, _normalRoomsCollections.Count-1);
            MazeCell _rngRoom = _normalRoomsCollections[_roomIndex];
            if (!_SelectedRoomsCollections.Contains(_rngRoom))
            {
                if (_rngRoom.x > 1 && _rngRoom.y > 1)
                {
                    _SelectedRoomsCollections.Add(_rngRoom);
                    Debug.Log($"{i}Room Added from Normal Room : {_roomIndex}");
                }
                else
                {
                    Debug.Log($"{i}Room didnt Match the criteria");
                }
                
            }
            else
            {
                Debug.Log("Picked a Room already available");
                i--;
            }
            
        }
        
        Debug.Log($"CrouchWalls Picked Rooms: {_SelectedRoomsCollections.Count}");
    }

    private IEnumerator DeployCrouchWall()
    {
        //Crouch Walls Deciding Logic Here

        foreach (MazeCell cell in _SelectedRoomsCollections)
        {
            if (cell.x <= 1 || cell.y <= 1 || cell.x >= 18 || cell.y >= 18)
            {
                continue;
                
            }
            Debug.Log($"MazeCell in SelectedRooms For Crouching {cell.x}:{cell.y}");
            List<Wall> _walls = new List<Wall>(cell.GetWalls());
            _walls = _walls.Where(w => !w.GetIsCleared()).ToList();
            foreach (Wall wall in _walls)
            {
                Debug.Log($"Remaining Wall in cell{cell.x}_{cell.y} is {wall.name} ");
            }
            int _rng = RandomNumberGenerator(_walls.Count);
            Wall crouchWall = _walls[_rng];
            MazeCell crouchCell = crouchWall.GetComponentInParent<MazeCell>();
            if (crouchCell == null)
            {
                Debug.Log("Crouch Cell is Null");
            }
            else
            {
                Debug.Log($"Chosen wall is {crouchWall.name}");
                Vector3 _currentCellRotation;
                Vector3 _currentCellPosition;
                Vector3 _previousCellPosition;
                Vector3 _midPoint;
                if (crouchWall.name == "LeftWall")
                {
                    MazeCell previousCell = _mazeGrid[crouchCell.x-1,crouchCell.y];
                    
                    _currentCellRotation = crouchCell.GetLocationLeftWall().rotation.eulerAngles;
                    _currentCellPosition = crouchCell.GetLocationLeftWall().position;
                    _previousCellPosition = previousCell.GetLocationRightWall().position;
            
                    _midPoint = (_currentCellPosition - _previousCellPosition)/2;
                    
                    Debug.Log($"Clearing Crouch Wall {crouchCell.x}:{crouchCell.y} & Previous Cell Wall {previousCell.x}:{previousCell.y}");
                    crouchCell.ClearLeftWall();
                    previousCell.ClearRightWall();
                    Instantiate(_crouchWallPrefab, _currentCellPosition-_midPoint, Quaternion.Euler(_currentCellRotation));
                }
                else if (crouchWall.name == "RightWall")
                {
                    MazeCell previousCell = _mazeGrid[crouchCell.x+1,crouchCell.y];
                    
                    _currentCellRotation = crouchCell.GetLocationRightWall().rotation.eulerAngles;
                    _currentCellPosition = crouchCell.GetLocationRightWall().position;
                    _previousCellPosition = previousCell.GetLocationLeftWall().position;
            
                    _midPoint = (_currentCellPosition - _previousCellPosition)/2;
                    
                    Debug.Log($"Clearing Crouch Wall {crouchCell.x}:{crouchCell.y} & Previous Cell Wall {previousCell.x}:{previousCell.y}");
                    crouchCell.ClearRightWall();
                    previousCell.ClearLeftWall();
                    Instantiate(_crouchWallPrefab, _currentCellPosition-_midPoint, Quaternion.Euler(_currentCellRotation));
                } else if (crouchWall.name == "FrontWall")
                {
                    MazeCell previousCell = _mazeGrid[crouchCell.x,crouchCell.y+1];
                    
                    _currentCellRotation = crouchCell.GetLocationFrontWall().rotation.eulerAngles;
                    _currentCellPosition = crouchCell.GetLocationFrontWall().position;
                    _previousCellPosition = previousCell.GetLocationBackWall().position;
            
                    _midPoint = (_currentCellPosition - _previousCellPosition)/2;
                    
                    Debug.Log($"Clearing Crouch Wall {crouchCell.x}:{crouchCell.y} & Previous Cell Wall {previousCell.x}:{previousCell.y}");
                    crouchCell.ClearFrontWall();
                    previousCell.ClearBackWall();
                    Instantiate(_crouchWallPrefab, _currentCellPosition-_midPoint, Quaternion.Euler(_currentCellRotation));
                } else if (crouchWall.name == "BackWall")
                {
                    MazeCell previousCell = _mazeGrid[crouchCell.x,crouchCell.y-1];
                    
                    _currentCellRotation = crouchCell.GetLocationBackWall().rotation.eulerAngles;
                    _currentCellPosition = crouchCell.GetLocationBackWall().position;
                    _previousCellPosition = previousCell.GetLocationFrontWall().position;
            
                    _midPoint = (_currentCellPosition - _previousCellPosition)/2;
                    
                    Debug.Log($"Clearing Crouch Wall {crouchCell.x}:{crouchCell.y} & Previous Cell Wall {previousCell.x}:{previousCell.y}");
                    crouchCell.ClearBackWall();
                    previousCell.ClearFrontWall();
                    Instantiate(_crouchWallPrefab, _currentCellPosition-_midPoint, Quaternion.Euler(_currentCellRotation));
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
        
        Debug.Log("CrouchWalls Deployed");
        AssignLightsAndTables();
    }

    private void AssignLightsAndTables() {
        //Assign Tables and Lights Here
        Debug.Log("Assigning Tables and Lights Deployed");
    }

    private int RandomNumberGenerator(int maxValue) {
        return Random.Range(0,maxValue-1);
    }

    private MazeCell GetRoomGenerate(int index,string roomName, Material roomMaterial)
    {
        int _roomIndex;
        if (index == -1)
        {
            _roomIndex = RandomNumberGenerator(_keyRoomsCollections.Count);
        }
        else
        {
            _roomIndex = index;
        }
        MazeCell _room = _keyRoomsCollections[_roomIndex];
        _keyRoomsCollections.Remove(_room);
        Debug.Log($"Room Index : {_roomIndex} & Remaining Rooms {_keyRoomsCollections.Count}");
        _room.SetCellType(roomName);
        _room.SetFloorMaterial(roomMaterial);
        Debug.Log($"{roomName} Assigned at ({_room.x}, {_room.y}) Key Room Collection Count : {_keyRoomsCollections.Count}");
        return _room;
    }

    public void AddImpRoomToCollection(MazeCell cell) {
        _keyRoomsCollections.Add(cell);
    }

    public void TriggerDaggerPicked() {
        _isDaggerPicked = true;
    }


}
