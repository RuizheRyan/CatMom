using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    const int numMax = 3;
    [SerializeField] HashSet<GameObject> cat = new HashSet<GameObject>();
    float fear = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" || other.tag == "kitten")
        {
            cat.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" || other.tag == "kitten")
        {
            cat.Remove(other.gameObject);
        }
    }

    private void FixedUpdate()
    {
        bool isFear = false;
        foreach(GameObject c in cat) 
        {
            if (c.tag == "kitten" && c.GetComponent<AIController>().fear > 0)
            {
                isFear = true;
            }
        }

        if (cat.Count == numMax && !isFear)
        {
            SceneManager.LoadScene("forest", LoadSceneMode.Single);
        }
    }
}
