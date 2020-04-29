using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform player;
    private Material catMat;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        catMat = player.GetComponentInChildren<Renderer>().material;
    }


    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = player.position + Vector3.up * 0.25f;
        Ray ray = new Ray(transform.position, -transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 3, ~(1 << 8)))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red);
            Camera.main.transform.localPosition = new Vector3(0, 0, Mathf.Min(0.1f, 0.1f - hit.distance));
            catMat.SetFloat("_Alpha", Mathf.Clamp01((hit.distance - 0.7f) / 0.5f));
        }
        else
        {
            Camera.main.transform.localPosition = new Vector3(0, 0, -3);
            catMat.SetFloat("_Alpha", 1);
        }

        if(Input.GetAxis("Mouse X") != 0)
        {
            transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * 3, Space.World);
        }
        if (Input.GetAxis("Mouse Y") < 0 && (transform.eulerAngles.x < 60 || transform.eulerAngles.x > 340))
        {
            transform.Rotate(Vector3.right, -Input.GetAxis("Mouse Y") * 1.5f);
        }
        if (Input.GetAxis("Mouse Y") > 0 && (transform.eulerAngles.x > 350 || (transform.eulerAngles.x > 0 && transform.eulerAngles.x < 70)))
        {
            transform.Rotate(Vector3.right, -Input.GetAxis("Mouse Y") * 1f);
        }
    }
}