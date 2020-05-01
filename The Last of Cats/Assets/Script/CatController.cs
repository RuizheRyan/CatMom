using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController : MonoBehaviour
{
    public GameObject cameraController;
    public float speedMax;
    public float speed;
    public float runSpeed;
    public float jumpSpeed;
    public float gravity;
    public float rotateSpeed;
    public float runRotateSpeed;
    public GameObject tip;
    public bool isCarrying;
    public bool isCalling;
    public float fallingSpeed;

    //private Vector3 moveDirection = Vector3.zero;
    private float deltaAngle;
    private Animator catAnimator;
    //private CharacterController controller;
    private float buffer, lastTime;
    [SerializeField]
    private Transform mouth;
    private GameObject kitten;
    private bool isGrounded;
    private Rigidbody rb;

    [FMODUnity.EventRef]
    public string callBackSoundPath;
    FMOD.Studio.EventInstance callBackSound;
    FMOD.Studio.PLAYBACK_STATE playState = FMOD.Studio.PLAYBACK_STATE.STOPPED;
    // The purr sound
    [FMODUnity.EventRef]
    public string purrSoundPath;
    FMOD.Studio.EventInstance purrSound;

    void Start()
    {
        mouth = mouth == null ? GameObject.Find("MouthPos").transform : mouth;
        buffer = 0.0f;
        tip = GameObject.Instantiate(tip);
        tip.SetActive(false);
        tip.transform.SetParent(transform);
        catAnimator = GetComponent<Animator>();
        //controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        runSpeed = runSpeed == 0 ? speed * 2 : runSpeed;
        runRotateSpeed = runRotateSpeed == 0 ? rotateSpeed / 2 : runRotateSpeed;

        // Instantiate the call back sound
        callBackSound = FMODUnity.RuntimeManager.CreateInstance(callBackSoundPath);
        // Instantiate the purr sound
        purrSound = FMODUnity.RuntimeManager.CreateInstance(purrSoundPath);
    }

    void FixedUpdate()
    {
        Ray groundRay = new Ray(transform.position + Vector3.up * 0.25f, Vector3.down);
        RaycastHit groundHit;
        if (Physics.Raycast(groundRay, out groundHit, 0.26f))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        Debug.DrawLine(groundRay.origin, groundHit.point, Color.red);

        Ray forwardRay = new Ray(transform.position + transform.forward * -0.1f + Vector3.up * 0.1f, transform.forward);
        RaycastHit forwardHit;
        if (Physics.Raycast(forwardRay, out forwardHit, 0.75f))
        {
            if (forwardHit.transform.CompareTag("kitten"))
            {
                if(kitten != null && forwardHit.transform.name != kitten.name)
                {
                    kitten?.GetComponent<AIController>().SetHighLight(false);
                }
                kitten = forwardHit.transform.gameObject;
                kitten.GetComponent<AIController>().SetHighLight(true);
            }
            else
            {
                kitten?.GetComponent<AIController>().SetHighLight(false);
                kitten = null;
            }
        }
        else
        {
            kitten?.GetComponent<AIController>().SetHighLight(false);
            kitten = null;
        }
        Debug.DrawLine(forwardRay.origin, forwardRay.origin + transform.forward * 0.75f, Color.green);
    }

    private void Update()
    {
        // To call kitten back
        if (Input.GetKey(KeyCode.Q))
        {
            callBackSound.getPlaybackState(out playState);
            if (playState != FMOD.Studio.PLAYBACK_STATE.PLAYING) callBackSound.start();

            isCalling = true;
            if(kitten) putDown(kitten);
        }

        if (kitten != null)
        {
            AIController ac = kitten.gameObject.GetComponent<AIController>();
            if ((Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.LeftControl)) && !isCarrying)
            {
                //c# nullable syntax same thing as checking if (kitten != null) kitten.comfort
                ac?.Comfort();
                FMOD.Studio.PLAYBACK_STATE playState = FMOD.Studio.PLAYBACK_STATE.STOPPED;
                purrSound.getPlaybackState(out playState);
                if (playState != FMOD.Studio.PLAYBACK_STATE.PLAYING) purrSound.start();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (ac.status != AIController.AIStatus.inMouth && !isCarrying)
                {
                    isCarrying = true;
                    ac.status = AIController.AIStatus.inMouth; 
                    var kittenTrans = kitten.transform;
                    kittenTrans.SetParent(mouth);
                    kitten.GetComponent<Rigidbody>().isKinematic = true;
                    kittenTrans.GetComponent<Collider>().enabled = false;
                    kittenTrans.localPosition = Vector3.zero;
                    kittenTrans.localRotation = Quaternion.identity;
                }
                else
                {
                    putDown(kitten);
                }
            }
            else
            {
                purrSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }

        #region move

        var rotation = Quaternion.Euler(0, cameraController.transform.eulerAngles.y, 0);
        var targetDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        targetDir = rotation * targetDir.normalized * Mathf.Min(targetDir.magnitude, 1);

        transform.forward = Vector3.RotateTowards(transform.forward, targetDir, rotateSpeed * Time.deltaTime, 0);

        //deltaAngle = (Vector3.Dot(transform.forward, targetDir.normalized) + 1) * Vector3.Dot(transform.right, targetDir) > 0 ? 1 : -1;
        if(Vector3.Dot(transform.right, targetDir) > 0)
        {
            deltaAngle = Vector3.Dot(targetDir.normalized, -transform.forward) * 0.25f + 0.75f;
        }
        else
        {
            deltaAngle = Vector3.Dot(targetDir.normalized, transform.forward) * 0.25f + 0.25f;
        }
        deltaAngle = deltaAngle * 1.2f - 0.1f;
        if (targetDir.magnitude != 0)
        {
            catAnimator.SetBool("isWalk", true);
            catAnimator.SetFloat("walkSpeed", targetDir.magnitude);
            var tempAngle = catAnimator.GetFloat("deltaAngle");
            var deltaStep = Time.deltaTime * 1.5f;
            if(deltaAngle > tempAngle + 0.01f)
            {
                catAnimator.SetFloat("deltaAngle", tempAngle + deltaStep);
            }
            else if(deltaAngle < tempAngle - 0.01f)
            {
                catAnimator.SetFloat("deltaAngle", tempAngle - deltaStep);
            }
            else
            {
                catAnimator.SetFloat("deltaAngle", deltaAngle);
            }
            //catAnimator.SetFloat("deltaAngle", (deltaAngle + 2) / 4 > tempAngle ? tempAngle + deltaStep : tempAngle - deltaStep);
        }
        else
        {
            catAnimator.SetBool("isWalk", false);
            catAnimator.SetFloat("walkSpeed", 0);
        }

        var tempVel = rb.velocity;
        tempVel = transform.forward * (Input.GetKey(KeyCode.LeftShift) ? runSpeed : speed) * targetDir.magnitude;
        tempVel.y = rb.velocity.y - Time.deltaTime * fallingSpeed;
        rb.velocity = tempVel;

        catAnimator.SetBool("isRun", (Input.GetKey(KeyCode.LeftShift) ? true : false));
        #endregion

        if (isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
            }
        }

        if ((Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.LeftControl)) && !isCarrying) 
        {
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

        //if(other.tag == "kitten")
        //{
        //    kitten = other.gameObject;
        //}
    }

    void putDown(GameObject kitten)
    {
        AIController ac = kitten.gameObject.GetComponent<AIController>();

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
        kitten.GetComponent<Rigidbody>().isKinematic = false;
        kittenTrans.GetComponent<Collider>().enabled = true;
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

        //if (other.tag == "kitten")
        //{
        //    kitten = null;
        //}
    }
}
