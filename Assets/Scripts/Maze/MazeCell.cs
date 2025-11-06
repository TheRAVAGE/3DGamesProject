using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [Header("Maze Objects")]
    [SerializeField] private Wall _leftWall;
    [SerializeField] private Wall _rightWall;
    [SerializeField] private Wall _frontWall;
    [SerializeField] private Wall _backWall;
    [SerializeField] private GameObject _unvisitedBlock;
    [SerializeField] private GameObject _floor;
    [SerializeField] private GameObject _ceiling;

    GroundSpawner _groundSpawner;
    CeillingSpawner _ceillingSpawner;

    private Wall[] _walls;

    private string _cellType = "Cell";
    //RoomTypes
    //: Hallway, ImpRoom, PlayerSpawnRoom, PlayerWinRoom, NormalRoom, EnemyRoom,

    MeshRenderer floorRenderer;

    public int x;
    public int y;

   MazeCell (int x, int y) {
        this.x = x;
        this.y = y;
        _leftWall.wallName = "LeftWall";
        _leftWall.isCleared = false;
        _rightWall.wallName = "RightWall";
        _rightWall.isCleared = false;
        _frontWall.wallName = "FrontWall";
        _frontWall.isCleared = false;
        _backWall.wallName = "BackWall";
        _backWall.isCleared = false;

   }

    private void Awake() {
        floorRenderer = _floor.GetComponentInChildren<MeshRenderer>();
        _walls = new Wall[4] {_leftWall,_rightWall,_frontWall, _backWall};
    }

    private void Start() {
        _groundSpawner = GetComponentInChildren<GroundSpawner>();
        _ceillingSpawner = GetComponentInChildren<CeillingSpawner>();

        //Null checks
        if (floorRenderer == null) { Debug.LogError("MazeCell: Floor MeshRenderer is null!"); }
        if (_groundSpawner == null) { Debug.LogError("MazeCell: GroundSpawner is null!"); }
        if (_ceillingSpawner == null) { Debug.LogError("MazeCell: CeilingSpawner is null!"); }
    }

    public bool isVisited { get; private set; }

    public void Visited() {
        isVisited = true;
        ClearCeiling();
        _unvisitedBlock.SetActive(false);
    }

    public void Unvisited() {
        isVisited = false;
        _unvisitedBlock.SetActive(true);
    }

    public void ClearLeftWall()
    {
        _leftWall.gameObject.SetActive(false);
        _leftWall.GetComponent<Wall>().SetIsCleared(true);
    }

    public void ClearRightWall()
    {
        _rightWall.gameObject.SetActive(false);
        _rightWall.GetComponent<Wall>().SetIsCleared(true);
    }

    public void ClearFrontWall()
    {
        _frontWall.gameObject.SetActive(false);
        _frontWall.GetComponent<Wall>().SetIsCleared(true);
    }

    public void ClearBackWall()
    {
        _backWall.gameObject.SetActive(false);
        _backWall.GetComponent<Wall>().SetIsCleared(true);
    }
    public void SetClearLeftWall()
    {
        _leftWall.GetComponent<Wall>().SetIsCleared(true);
    }

    public void SetClearRightWall()
    {
        _rightWall.GetComponent<Wall>().SetIsCleared(true);
    }

    public void SetClearFrontWall()
    {
        _frontWall.GetComponent<Wall>().SetIsCleared(true);
    }

    public void SetClearBackWall()
    {
        _backWall.GetComponent<Wall>().SetIsCleared(true);
    }
    public void ClearCeiling() { _ceiling.SetActive(false); }

    public Transform GetLocationLeftWall() { return _leftWall.transform; }
    public Transform GetLocationRightWall() { return _rightWall.transform; }
    public Transform GetLocationFrontWall() { return _frontWall.transform; }
    public Transform GetLocationBackWall() { return _backWall.transform; }

    //Getters and Setters
    public void SetFloorMaterial(Material mat) { floorRenderer.material = mat; }

    public void SetCellType(string type) { _cellType = type; }
    public string GetCellType() { return _cellType; }

    public Transform GetGroundSpawnerLocation() { return _groundSpawner.transform; }
    public Transform GetCeilingSpawnerLocation() { return _ceillingSpawner.transform; }

    public Wall[] GetWalls()
    {
        return _walls;
    }

}
