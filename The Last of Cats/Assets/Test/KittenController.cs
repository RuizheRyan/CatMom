using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum kittenState{ still, fear, beHold, autoMoveForward}

public class KittenController : MonoBehaviour
{
    [SerializeField] GameObject player;

    public kittenState state = kittenState.fear;
    const float offset_height = 0.3f;
    const float offset_length = 0.4f;

    public Vector3 pointingPos = new Vector3(0, 0, 0);
    const float speed = 1;

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case kittenState.still: 
                {

                }break;
            case kittenState.fear: 
                {

                }break;
            case kittenState.beHold:
                {
                    transform.localPosition = player.transform.localPosition + player.transform.forward * offset_length + player.transform.up * offset_height;
                    transform.localRotation = player.transform.localRotation;
                    transform.Rotate(0, 90, 0);
                    return;

                }
                break;
            case kittenState.autoMoveForward: 
                {
                    if (pointingPos != Vector3.zero)
                    {
                        Vector3 direction = Vector3.Normalize(pointingPos - transform.position);
                        transform.forward = direction;
                        transform.position += direction * Time.deltaTime * speed;
                    }
                }
                break;
        }
    }
}
