using UnityEngine;
using TMPro;

public class WinPoint : MonoBehaviour
{
    int[] _itemObtained = new int[3] { 0, 0, 0 }; // Dagger, Fuse, Key

    TMP_Text _UIText;
    TableUI _tableUI;
    GameManager _gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
        _tableUI = GetComponentInChildren<TableUI>();
        _gameManager = FindFirstObjectByType<GameManager>();
        //_keyItem = GetComponent<KeyItem>();
        //itemText = _tableUI.GetComponentInChildren<TMP_Text>();
        _UIText = _tableUI.GetComponentInChildren<TMP_Text>();
        if (_tableUI == null) { Debug.Log("Table UI Component Not found in Win Point"); }
        if (_UIText == null) { Debug.Log("UI Text Component Not found in Win Point"); }
        if (_gameManager == null) { Debug.Log("Game Manager Not Found in Win Point") ; }
        _UIText.text = "Press F To Interact";
    }
    // Update is called once per frame
    void Update() {

    }

    public void SetPickUpStatus(int index, int value) {
        _itemObtained[index] = value;
        foreach (int i in _itemObtained) {
            Debug.Log(i);
        }
        if (_itemObtained[0] == 1 && _itemObtained[1] == 1 && _itemObtained[2] == 1) {
            gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {

            _tableUI.gameObject.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player")) {
            PlayerController _player = other.GetComponent<PlayerController>();
            int[] _inventory = _player.GetInventory();
            if (_player != null && _player.GetInteractInput() == 1) {
                Debug.Log("Player Won");
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            _tableUI.gameObject.SetActive(false);
        }
    }
}
