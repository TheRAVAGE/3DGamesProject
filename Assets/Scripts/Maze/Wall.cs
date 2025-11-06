using Unity.VisualScripting;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private string _wallName;
    private bool _isCleared;

    public string wallName;
    public bool isCleared;

    Wall(string wallName, bool isCleared)
    {
        this._wallName = wallName;
        this._isCleared = isCleared;
    }

    public void SetIsCleared(bool value)
    {
        _isCleared = value;
    }
    public bool GetIsCleared()
    {
        return _isCleared;
    }

    public void SetWallName(string wallName)
    {
        _wallName = wallName;
    }
    public string GetWallName()
    {
        return _wallName;
    }
}
