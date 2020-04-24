using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //singleton
    public static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<GameManager>();
            if (_instance == null)
            {
                var obj = new GameObject();
                obj.AddComponent<GameManager>();
                _instance = obj.GetComponent<GameManager>();
            }
            return _instance;
        }
    }
    [SerializeField] GameObject player;
    [SerializeField] GameObject[] kittens;
    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player");
        //kittens = GameObject.FindGameObjectsWithTag("kitten");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !player.GetComponent<CatController>().isCarrying)
        {
            foreach(var i in kittens)
            {
                if(i.GetComponent<AIController>().status != AIController.AIStatus.fear)
                {
                    i.GetComponent<AIController>().status = AIController.AIStatus.follow;
                }
            }
        }
    }
}
