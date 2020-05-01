using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FearZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("kitten"))
        {
            other.GetComponent<AIController>().fear = 1;
        }
    }
}
