using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCameraController : MonoBehaviour
{
    [SerializeField] bool switchToKitten = true;
    [SerializeField] GameObject target;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" || other.tag == "kitten")
        { 
            CameraController.isPlayer = switchToKitten;
        }

        if (other.tag == "kitten")
        {
            other.GetComponent<KittenController>().pointingPos = target.transform.position;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "kitten" && other.GetComponent<KittenController>().state == kittenState.still)
        {
            other.GetComponent<KittenController>().state = kittenState.autoMoveForward;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "kitten" && other.GetComponent<KittenController>().state == kittenState.beHold)
        {
            other.GetComponent<KittenController>().pointingPos = Vector3.zero;
        }
    }
}
