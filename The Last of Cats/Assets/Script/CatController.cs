using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController : MonoBehaviour
{
    public GameObject cameraController ;
    public float speed;
    public float jumpSpeed;
    public float gravity;
    public float rotateSpeed;
    public GameObject hint;
    private Vector3 moveDirection = Vector3.zero;
    private float deltaAngle;
    private Animator catAnimator;
    private CharacterController controller;
    private float runSpeed;
    private float buffer, lastTime;

    void Start()
    {
        buffer = 0.0f;
        runSpeed = speed * 1.7f;
        hint = GameObject.Instantiate(hint);
        hint.SetActive(false);
        hint.transform.SetParent(transform);
        catAnimator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }
    void LateUpdate()
    {
        
    }

    private void Update()
    {
        if (controller.isGrounded)
        {
            var rotation = Quaternion.Euler(0, cameraController.transform.eulerAngles.y, 0);
            var dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            dir = rotation * dir.normalized * Mathf.Min(dir.magnitude, 1);
            
            deltaAngle = Vector3.SignedAngle(new Vector3(dir.x, 0, dir.z), transform.forward, -Vector3.up);
            if (deltaAngle > 5)
            {
                transform.Rotate(Vector3.up, Time.deltaTime * rotateSpeed);
            }
            else if (deltaAngle < -5)
            {
                transform.Rotate(Vector3.up, Time.deltaTime * -rotateSpeed);
            }
            
            moveDirection = transform.forward * dir.magnitude;
            if (moveDirection != Vector3.zero)
            {
                catAnimator.SetBool("isWalk", true);
                catAnimator.SetFloat("walkSpeed", moveDirection.magnitude);
                catAnimator.SetFloat("deltaAngle",( deltaAngle / 360  * dir.magnitude)+ 0.5f);
            }
            else
            {
                catAnimator.SetBool("isWalk", false);
                catAnimator.SetFloat("walkSpeed", 0);
            }
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * (Input.GetKey(KeyCode.LeftShift) ? runSpeed : speed) * Time.deltaTime);

        catAnimator.SetBool("isRun", (Input.GetKey(KeyCode.LeftShift) ? true : false));

        //lastTime = gameManager._time;

        if (Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.LeftControl) ) {
            
            catAnimator.SetBool("isClean", true);
            if(buffer >= 1)
            {
                buffer = 0.0f;
            }
        }
        else if (!Input.GetKey(KeyCode.C) && !Input.GetKey(KeyCode.LeftControl))
        {
            catAnimator.SetBool("isClean", false);
        }

        // Hold & put down the kitten
        if (Input.GetKeyUp(KeyCode.RightControl))
        {
            GameManager.toHoldKitten = true;
        }

        // Comfort the kitten
        if (Input.GetKeyUp(KeyCode.RightAlt))
        {
            GameManager.comfortKitten = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("trigger"))
        {
            hint.SetActive(true);
            hint.transform.position = transform.position + Vector3.up * 0.6f + transform.forward * 0.3f;
            hint.transform.LookAt(Camera.main.transform);
        }
        else
        {
            hint.SetActive(false);
        }

        if(other.tag == "kitten" && (Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.LeftControl)))
        {
            Kitten kitten = other.gameObject.transform.root.gameObject.GetComponent<Kitten>();
            
            //c# nullable syntax same thing as checking if (kitten != null) kitten.comfort
            kitten?.Comfort();
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("trigger"))
        {
            other.transform.GetComponentInParent<Renderer>().material.SetFloat("_Emission", 1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("trigger"))
        {
            hint.SetActive(false);
            other.transform.GetComponentInParent<Renderer>().material.SetFloat("_Emission", 0) ;
        }
    }
}
