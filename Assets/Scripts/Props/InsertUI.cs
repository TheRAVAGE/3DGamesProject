using TMPro;
using UnityEngine;

public class InsertUI : MonoBehaviour {
    [SerializeField] private string _itemName;
    [SerializeField] private WinPoint _winPoint;


    TableUI _tableUI;
    KeyItem _keyItem;
    TMP_Text _itemText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        _tableUI = GetComponentInChildren<TableUI>();
        _keyItem = GetComponentInChildren<KeyItem>();
        _itemText = _tableUI.GetComponentInChildren<TMP_Text>();

        //NULL CHECKS
        if (_keyItem == null) { Debug.Log("Key Item Not Found in Trigger UI"); }
        if (_tableUI == null) { Debug.Log("Table Ui Not Found in Table"); }
        if (_itemText == null) { Debug.Log("Item Text Not Found in Table UI"); }
        if (_winPoint == null) { Debug.Log("Win Point Not Found in Insert UI"); }


        _tableUI.gameObject.SetActive(false);
        _keyItem.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (_keyItem.enabled ==true) { _itemText.text = _itemName + "already Placed"; }
            int[] _inventory = other.GetComponent<PlayerController>().GetInventory();
            if (_itemName == "Dagger" && _inventory[0] > 0) {
                _itemText.text = _itemName + " : Press F to Insert.";
            } else if (_itemName == "Fuse" && _inventory[1] > 0) {
                _itemText.text = _itemName + " : Press F to Insert.";
            } else if (_itemName == "Key" && _inventory[2] > 0) {
                _itemText.text = _itemName + " : Press F to Insert.";
            } else {
                _itemText.text = "You don't have the required item.";
            }
            _tableUI.gameObject.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player")) {
            PlayerController _player = other.GetComponent<PlayerController>();
            int[] _inventory = _player.GetInventory();
            if (_player != null && _player.GetInteractInput() == 1) {
                Debug.Log("Interacted!!");
                if (_itemName == "Dagger" && _inventory[0] > 0) {
                    _winPoint.SetPickUpStatus(0, 1);
                    _player.SetInventory(0, 0);
                    _keyItem.gameObject.SetActive(true);
                } else if (_itemName == "Fuse" && _inventory[1] > 0) {
                    _winPoint.SetPickUpStatus(1, 1);
                    _player.SetInventory(1, 0);
                    _keyItem.gameObject.SetActive(true);
                } else if (_itemName == "Key" && _inventory[2] > 0) {
                    _winPoint.SetPickUpStatus(2, 1);
                    _player.SetInventory(2, 0);
                    _keyItem.gameObject.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            _tableUI.gameObject.SetActive(false);
        }
    }
}
