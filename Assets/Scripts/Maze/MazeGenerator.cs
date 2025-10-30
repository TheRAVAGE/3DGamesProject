using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;

public class MazeGenerator : MonoBehaviour
{
    [Header("GameManager")]
    [SerializeField] private GameManager _gameManager;

    [Header("Map Generation")]
    [SerializeField] private MazeCell _mazeCellPrefab;
    [SerializeField] private int _mazeSize = 20;

    [Header("Enemy")]
    [SerializeField] private int _minEnemyDistanceFromPlayerSpawn = 2;

    private MazeCell[,] _mazeGrid;

    private MazeCell _startCell;
    private MazeCell _previousCellBuffer;
    private MazeCell _lastNextCell;
    private MazeCell _lastKeyRoomCell;
    private MazeCell _enemyRoomCell;

    private int _leftWallDestroyed = 0;
    private int _rightWallDestroyed = 0;
    private int _frontWallDestroyed = 0;
    private int _backWallDestroyed = 0;
    private int _mazeWidth;
    private int _mazeLength;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        _mazeLength = _mazeSize;
        _mazeWidth = _mazeSize;
        _mazeGrid = new MazeCell[_mazeWidth, _mazeLength];
        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeLength; z++)
            {
                MazeCell newCell = Instantiate(_mazeCellPrefab, new Vector3(x*10, 0, z*10), Quaternion.identity);
                newCell.x = x;
                newCell.y = z;
                newCell.name = $"MazeCell: {x}_{z}";
                _mazeGrid[x, z] = newCell;
            }
        }
        _startCell = _mazeGrid[0, 0];

        yield return GenerateMaze(null, null, _mazeGrid[0,0]);

        //Maze Generation Complete
        _startCell.SetCellType("PlayerSpawnRoom");
        _mazeGrid[_mazeWidth-1, _mazeLength-1].SetCellType("PlayerWinRoom");

        //Sending the map to GameManager
        _gameManager.SetMapGenerated(true, _mazeGrid, _mazeSize);

    }

    private IEnumerator GenerateMaze (MazeCell previousCellBuffer, MazeCell previousCell, MazeCell currentCell) {


        //Debug.Log($"PreviousCellBuffer : {previousCellBuffer?.name ?? "null"} PreviousCell: {previousCell?.name ?? "null"} CurrentCell: {currentCell.name}");

        currentCell.Visited();
        ClearWalls(previousCellBuffer, previousCell, currentCell);


        yield return new WaitForSeconds(0.01f);


        MazeCell _nextCell;

        do {
            _nextCell = GetNextUnvisitedCell(currentCell);


            if (_nextCell != null) {
                previousCellBuffer = previousCell;
                _lastNextCell = _nextCell;
                _lastKeyRoomCell = _nextCell;
                yield return GenerateMaze(previousCellBuffer, currentCell, _nextCell);

                _lastKeyRoomCell.SetCellType("ImpRoom");
                _gameManager.AddImpRoomToCollection(_lastKeyRoomCell);
                Debug.Log($"ImpRoom Assigned at ({_lastKeyRoomCell.x}, {_lastKeyRoomCell.y})");
            }

        } while (_nextCell != null);
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell) {
        var unvisitedCells = GetUnvisitedCells(currentCell);
        return unvisitedCells.OrderBy(c => Random.Range(1,10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell) {
        int x = (int)currentCell.x;
        int z = (int)currentCell.y;

        if (x+1 < _mazeWidth) {
            var cellToRight = _mazeGrid[x+1, z];

            if (!cellToRight.isVisited) {
                yield return cellToRight;
            }
        }
        if (x - 1 >= 0) {
            var cellToLeft = _mazeGrid[x - 1, z];

            if (!cellToLeft.isVisited) {
                yield return cellToLeft;
            }
        }
        if (z + 1 < _mazeLength) {
            var cellToFront = _mazeGrid[x, z + 1];
            if (!cellToFront.isVisited) {
                yield return cellToFront;
            }
        }
        if (z - 1 >= 0) {
            var cellToBack = _mazeGrid[x, z - 1];
            if (!cellToBack.isVisited) {
                yield return cellToBack;
            }
        }
    }

    private void ClearWalls(MazeCell previousCellBuffer, MazeCell previousCell, MazeCell currentCell) {
        if (previousCell == null) { return; }

        Vector3 _currentCellRotation;
        Vector3 _currentCellPosition;

        if (previousCell.x < currentCell.x) {
            _rightWallDestroyed = 0;
            _frontWallDestroyed = 0;
            _backWallDestroyed = 0;

            _leftWallDestroyed++;
            _currentCellRotation = currentCell.GetLocationLeftWall().rotation.eulerAngles;
            _currentCellPosition = currentCell.GetLocationLeftWall().position;

            //Debug.Log($"Current Cell Rotation: {_currentCellRotation} and CurrentCell Position: {_currentCellPosition}");
            //Replace Walls Here

            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();


            if (_leftWallDestroyed >= 2) {

                //previousCellBuffer.SetFloorMaterial(_straightLineMaterial);
                //previousCell.SetFloorMaterial(_straightLineMaterial);
                //currentCell.SetFloorMaterial(_straightLineMaterial);

                previousCellBuffer.SetCellType("Hallway");
                previousCell.SetCellType("Hallway");
                currentCell.SetCellType("Hallway");
            }
            return;
        }
        if (previousCell.x > currentCell.x) {
            _leftWallDestroyed = 0;
            _frontWallDestroyed = 0;
            _backWallDestroyed = 0;

            _rightWallDestroyed++;
            _currentCellRotation = currentCell.GetLocationRightWall().rotation.eulerAngles;
            _currentCellPosition = currentCell.GetLocationRightWall().position;

            //Debug.Log($"Current Cell Rotation: {_currentCellRotation} and CurrentCell Position: {_currentCellPosition}");
            //Replace Walls Here

            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();


            if (_rightWallDestroyed >= 2) {
                //previousCellBuffer.SetFloorMaterial(_straightLineMaterial);
                //previousCell.SetFloorMaterial(_straightLineMaterial);
                //currentCell.SetFloorMaterial(_straightLineMaterial);

                previousCellBuffer.SetCellType("Hallway");
                previousCell.SetCellType("Hallway");
                currentCell.SetCellType("Hallway");
            }
            return;
        }
        if (previousCell.y < currentCell.y) {
            _leftWallDestroyed = 0;
            _rightWallDestroyed = 0;
            _frontWallDestroyed = 0;

            _backWallDestroyed++;
            _currentCellRotation = currentCell.GetLocationBackWall().rotation.eulerAngles;
            _currentCellPosition = currentCell.GetLocationBackWall().position;

            //Debug.Log($"Current Cell Rotation: {_currentCellRotation} and CurrentCell Position: {_currentCellPosition}");
            //Replace Walls Here


            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();


            if (_backWallDestroyed >= 2) {
                //previousCellBuffer.SetFloorMaterial(_straightLineMaterial);
                //previousCell.SetFloorMaterial(_straightLineMaterial);
                //currentCell.SetFloorMaterial(_straightLineMaterial);

                previousCellBuffer.SetCellType("Hallway");
                previousCell.SetCellType("Hallway");
                currentCell.SetCellType("Hallway");
            }
            return;
        }
        if (previousCell.y > currentCell.y) {
            _leftWallDestroyed = 0;
            _rightWallDestroyed = 0;
            _backWallDestroyed = 0;

            _frontWallDestroyed++;
            _currentCellRotation = currentCell.GetLocationFrontWall().rotation.eulerAngles;
            _currentCellPosition = currentCell.GetLocationFrontWall().position;

            //Debug.Log($"Current Cell Rotation: {_currentCellRotation} and CurrentCell Position: {_currentCellPosition}");
            //Replace Walls Here


            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();


            if (_frontWallDestroyed >= 2) {
                //previousCellBuffer.SetFloorMaterial(_straightLineMaterial);
                //previousCell.SetFloorMaterial(_straightLineMaterial);
                //currentCell.SetFloorMaterial(_straightLineMaterial);

                previousCellBuffer.SetCellType("Hallway");
                previousCell.SetCellType("Hallway");
                currentCell.SetCellType("Hallway");
            }

            return;
        }
    }

    //Getters and Setters

}
