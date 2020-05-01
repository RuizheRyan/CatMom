using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        float fear = 0;
        Vector3 dir = player.transform.forward;
        foreach (var i in kittens)
        {
            //// Follow mother
            ////if (Input.GetMouseButtonDown(0) && !player.GetComponent<CatController>().isCarrying && i.GetComponent<AIController>().status != AIController.AIStatus.fear)
            //if (i.GetComponent<AIController>().status == AIController.AIStatus.idle)
            //{
            //    i.GetComponent<AIController>().setStatus(AIController.AIStatus.follow);
            //}

            // To record the max fear value
            if (i.GetComponent<AIController>().fear > fear)
            {
                fear = i.GetComponent<AIController>().fear;
                dir = i.transform.position - player.transform.position;
            }

            // To call kitten back
            if (i.GetComponent<AIController>().status != AIController.AIStatus.fear && player.GetComponent<CatController>().isCalling)
            {
                i.GetComponent<AIController>().setStatus(AIController.AIStatus.follow);
            }
        }

        // if any kitten is fear and the mother move opposite, slow the mother
        if (Cross(dir, player.transform.forward) <= 0.0f) player.GetComponent<CatController>().speed = player.GetComponent<CatController>().speedMax * (1.0f - 0.7f * fear);
        else player.GetComponent<CatController>().speed = player.GetComponent<CatController>().speedMax;

        player.GetComponent<CatController>().isCalling = false;
    }

    float Cross(Vector3 v1, Vector3 v2) 
    {
        return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
    }
}
