using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField] GameObject key;
    [SerializeField] GameObject door;

    static readonly Vector3[] keyPosition = {
        new Vector3(0, 0.12f, 0),
        new Vector3(0, 0.06f, 0),
    };

    // Start is called before the first frame update
    void Start()
    {
        key = transform.Find("Key").gameObject;
        key.transform.localPosition = keyPosition[0];
    }

    private void OnTriggerEnter(Collider other)
    {
        key.transform.localPosition = keyPosition[1];

        // Open the door
        door.GetComponent<DoorController>().isOpen = true;
    }
}
