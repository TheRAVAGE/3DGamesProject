using UnityEngine;

public class KeyItem : MonoBehaviour
{
    private string _name;
    private GameObject _itemPrefab;
    public string Name { get { return name; } }
    KeyItem(string name) {
        this._name = name;
    }
}
