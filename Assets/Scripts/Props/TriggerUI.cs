using TMPro;
using UnityEngine;

public class TriggerUI : MonoBehaviour
{
    [SerializeField] private string ItemName;


    TableUI _tableUI;
    //KeyItem _keyItem;
    TMP_Text itemText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _tableUI = GetComponentInChildren<TableUI>();
        //_keyItem = GetComponent<KeyItem>();
        itemText = _tableUI.GetComponentInChildren<TMP_Text>();

        //NULL CHECKS
        //if (_keyItem == null) { Debug.Log("Key Item Not Found in Trigger UI"); }
        if (_tableUI == null) { Debug.Log("Table Ui Not Found in Table"); }
        if (itemText == null) { Debug.Log("Item Text Not Found in Table UI"); }


        _tableUI.gameObject.SetActive(false);
        itemText.text = ItemName + " : Press F to Pick Up";
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other) {
        if ( other.CompareTag("Player")) {
            _tableUI.gameObject.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player")) {
            PlayerController _player = other.GetComponent<PlayerController>();
            if (_player!=null && _player.GetInteractInput() == 1) {
                Debug.Log("Interacted!!");
                _player.SetItemPicked(ItemName);
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if ( other.CompareTag("Player")) {
            _tableUI.gameObject.SetActive(false);
        }
    }
}
