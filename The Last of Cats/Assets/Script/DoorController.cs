using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public bool isOpen = false;
    float rotation = 0.0f;
    const float rotateSpeed = 30.0f;

    // Start is called before the first frame update
    void Start()
    {
        isOpen = false;
        rotation = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen && rotation <= 87.0f)
        {
            transform.Rotate(new Vector3(0, - rotateSpeed * Time.deltaTime, 0));
            rotation += rotateSpeed * Time.deltaTime;
        }
    }
}
