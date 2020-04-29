using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController : MonoBehaviour
{
    public GameObject cameraController ;
    public float speedMax;
    public float speed;
    public float jumpSpeed;
    public float gravity;
    public float rotateSpeed;
    public GameObject tip;
    public bool isCarrying;
    private Vector3 moveDirection = Vector3.zero;
    private float deltaAngle;
    private Animator catAnimator;
    private CharacterController controller;
    private float runSpeed;
    private float buffer, lastTime;
    [SerializeField]
    private Transform mouth;
    private GameObject kitten;
    private bool isGrounded;
    void Start()
    {
        mouth = mouth == null ? GameObject.Find("MouthPos").transform : mouth;
        buffer = 0.0f;
        tip = GameObject.Instantiate(tip);
        tip.SetActive(false);
        tip.transform.SetParent(transform);
        catAnimator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }
    void LateUpdate()
    {
        
    }

    private void Update()
    {
        if (kitten != null)
        {
            AIController ac = kitten.gameObject.GetComponent<AIController>();
            if (Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.LeftControl))
            {
                //c# nullable syntax same thing as checking if (kitten != null) kitten.comfort
                ac?.Comfort();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (ac.status != AIController.AIStatus.inMouth && !isCarrying)
                {
                    isCarrying = true;
                    ac.status = AIController.AIStatus.inMouth; 
                    var kittenTrans = kitten.transform;
                    kittenTrans.SetParent(mouth);
                    kittenTrans.GetComponent<Collider>().enabled = false;
                    kittenTrans.localPosition = Vector3.zero;
                    kittenTrans.localRotation = Quaternion.identity;
                }
                else
                {
                    isCarrying = false;
                    if (ac.fear >= 1)
                    {
                        ac.status = AIController.AIStatus.fear;
                        ac.anim.SetBool("isFear", true);
                        ac.anim.SetBool("inMouth", false);
                    }
                    else
                    {
                        ac.status = AIController.AIStatus.idle;
                        ac.anim.SetBool("inMouth", false);
                        ac.anim.SetBool("isFear", false);
                    }
                    //ac.status = ac.fear >= 1 ? AIController.AIStatus.fear : AIController.AIStatus.idle;
                    var kittenTrans = kitten.transform;
                    kittenTrans.SetParent(null);
                    kittenTrans.rotation = Quaternion.identity;
                    kittenTrans.GetComponent<Collider>().enabled = true;
                }
            }
        }

        #region move
        Ray groundRay = new Ray(transform.position + Vector3.up * 0.25f, Vector3.down);
        RaycastHit groundHit;
        if (Physics.Raycast(groundRay, out groundHit, 0.25f))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
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
            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * (Input.GetKey(KeyCode.LeftShift) ? runSpeed : speed) * Time.deltaTime);

        catAnimator.SetBool("isRun", (Input.GetKey(KeyCode.LeftShift) ? true : false));
        #endregion
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
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("trigger"))
        {
            tip.SetActive(true);
            tip.transform.position = transform.position + Vector3.up * 0.6f + transform.forward * 0.3f;
            tip.transform.LookAt(Camera.main.transform);
        }
        else
        {
            tip.SetActive(false);
        }

        if(other.tag == "kitten")
        {
            kitten = other.gameObject;
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
            tip.SetActive(false);
            other.transform.GetComponentInParent<Renderer>().material.SetFloat("_Emission", 0) ;
        }

        if (other.tag == "kitten")
        {
            kitten = null;
        }
    }
}
