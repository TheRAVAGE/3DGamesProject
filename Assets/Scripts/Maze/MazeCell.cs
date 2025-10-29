using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [Header("Maze Objects")]
    [SerializeField] private GameObject _leftWall;
    [SerializeField] private GameObject _righttWall;
    [SerializeField] private GameObject _FrontWall;
    [SerializeField] private GameObject _backWall;
    [SerializeField] private GameObject _unvisitedBlock;
    [SerializeField] private GameObject _floor;

    MeshRenderer floorRenderer;

    public int x;
    public int y;

   MazeCell (int x, int y) {
        this.x = x;
        this.y = y;
    }

    private void Awake() {
        floorRenderer = _floor.GetComponentInChildren<MeshRenderer>();
    }

    public bool isVisited { get; private set; }

    public void Visited() {
        isVisited = true;
        _unvisitedBlock.SetActive(false);
    }

    public void Unvisited() {
        isVisited = false;
        _unvisitedBlock.SetActive(true);
    }

    public void ClearLeftWall() {   _leftWall.SetActive(false);    }
    public void ClearRightWall() {  _righttWall.SetActive(false); }
    public void ClearFrontWall() {  _FrontWall.SetActive(false); }
    public void ClearBackWall() {   _backWall.SetActive(false); }

    public Transform GetLocationLeftWall() { return _leftWall.transform; }
    public Transform GetLocationRightWall() { return _righttWall.transform; }
    public Transform GetLocationFrontWall() { return _FrontWall.transform; }
    public Transform GetLocationBackWall() { return _backWall.transform; }

    public void SetFloorMaterial(Material mat) { floorRenderer.material = mat; }

}
