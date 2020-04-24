using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopController : MonoBehaviour
{
    [SerializeField] GameObject target;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "kitten")
        {
            other.GetComponent<KittenController>().pointingPos = target.transform.position;
            other.GetComponent<KittenController>().state = kittenState.fear;
        }
    }
}
