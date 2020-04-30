using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractionController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // attract the kitten which not to be hold by mother
        if (other.tag == "kitten" && other.GetComponent<AIController>().status != AIController.AIStatus.inMouth)
        {
            print(other.name);

            other.GetComponent<AIController>().setStatus(AIController.AIStatus.attracted, transform.position);
        }
    }
}
