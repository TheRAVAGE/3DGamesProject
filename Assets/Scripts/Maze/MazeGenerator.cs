using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private MazeCell _mazeCellPrefab;
    [SerializeField] private int _mazeWidth;
    [SerializeField] private int _mazeLength;

    private MazeCell[,] _mazeGrid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeLength];
        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeLength; z++)
            {
                MazeCell newCell = Instantiate(_mazeCellPrefab, new Vector3(x, 0, z), Quaternion.identity);
                newCell.name = $"MazeCell_{x}_{z}";
                _mazeGrid[x, z] = newCell;
            }
        }
        yield return GenerateMaze(null, _mazeGrid[0,0]);
    }

    private IEnumerator GenerateMaze (MazeCell previousCell, MazeCell currentCell) {
        currentCell.Visited();
        ClearWalls(previousCell, currentCell);

        yield return new WaitForSeconds(0.05f);

        MazeCell nextCell;

        do {
            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null) {
                yield return GenerateMaze(currentCell, nextCell);
            }
        } while (nextCell != null);        
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell) {
        var unvisitedCells = GetUnvisitedCells(currentCell);
        return unvisitedCells.OrderBy(c => Random.Range(1,10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell) {
        int x = (int)currentCell.transform.position.x;
        int z = (int)currentCell.transform.position.z;

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

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell) {
        if (previousCell == null) { return; }

        if (previousCell.transform.position.x < currentCell.transform.position.x) {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        } 
        if (previousCell.transform.position.x > currentCell.transform.position.x) {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }
        if (previousCell.transform.position.z < currentCell.transform.position.z) {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }
        if (previousCell.transform.position.z > currentCell.transform.position.z) {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
