using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject kitten;

    public static bool toHoldKitten = false;
    public static bool comfortKitten = false;

    // Update is called once per frame
    void Update()
    {
        if (toHoldKitten)
        {
            Vector3 distance = player.transform.position - kitten.transform.position;
            if (distance.x * distance.x + distance.y * distance.y + distance.z * distance.z <= 1)
            {
                if (kitten.GetComponent<KittenController>().state != kittenState.beHold) kitten.GetComponent<KittenController>().state = kittenState.beHold;
                else kitten.GetComponent<KittenController>().state = kittenState.still;
            }
            toHoldKitten = false;
        }

        if (comfortKitten)
        {
            if (kitten.GetComponent<KittenController>().state == kittenState.fear)
            {
                kitten.GetComponent<KittenController>().state = kittenState.autoMoveForward;
            }
            comfortKitten = false;
        }
    }
}
