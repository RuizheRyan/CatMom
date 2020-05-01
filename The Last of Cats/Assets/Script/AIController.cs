using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEditor;

public class AIController : MonoBehaviour
{
    public float fearDistance;
    Transform player;
    public enum AIStatus { follow, fear, idle, inMouth, attracted };
    public AIStatus status;
    public float speed;
    public float rotateSpeed;

    [Range(0, 1)] public float fear;
    [SerializeField] private float feartime = 3.0f;
    [SerializeField] private float fearChangeRate;

    public Animator anim;
    private Rigidbody rb;
    [SerializeField] private ParticleSystem comfortVFX;

    // The fear sound
    [FMODUnity.EventRef]
    public string fearSoundPath;
    FMOD.Studio.EventInstance fearSound;

    // The purr sound
    [FMODUnity.EventRef]
    public string purrSoundPath;
    FMOD.Studio.EventInstance purrSound;

    // The comfort sound
    [FMODUnity.EventRef]
    public string comfortSoundPath;
    FMOD.Studio.EventInstance comfortSound;

    public Vector3 targetPosition;
    
    Material material;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectsWithTag("Player")[0].transform;
        if(anim == null)
        {
            anim = gameObject.GetComponent<Animator>();
        }

        // Instantiate the fear sound
        fearSound = FMODUnity.RuntimeManager.CreateInstance(fearSoundPath);
        fearSound.setParameterByName("Pitch", Random.value);

        // Instantiate the purr sound
        purrSound = FMODUnity.RuntimeManager.CreateInstance(purrSoundPath);
        comfortSound = FMODUnity.RuntimeManager.CreateInstance(comfortSoundPath);

        material =  GetComponentInChildren<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        switch (status)
        {
            case AIStatus.fear:
                {
                    anim.SetBool("isFear", true);
                    anim.SetBool("inMouth", false);
                    rb.velocity = new Vector3(0, -1, 0);
                    if (fear <= 0)
                    {
                        status = AIStatus.idle;
                        anim.SetBool("isFear", false);
                    }

                    // Change the volume of the sound
                    fearSound.setParameterByName("Distance", fear);
                }
                break;
            case AIStatus.follow:
                {
                    moveTo(player.position);
                }
                break;
            case AIStatus.idle:
                {
                    anim.SetBool("inMouth", false);
                    var vel = rb.velocity;
                    vel.y = -1;
                    rb.velocity = vel;
                    if (fear >= 0.5)
                    {
                        setStatus(AIStatus.fear);
                    }
                    //fear when too far from player
                    if ((player.position - transform.position).magnitude > fearDistance)
                    {
                        fear += 1 * Time.deltaTime / feartime;
                    }
                    if (fear >= 1)
                    {
                        anim.SetBool("isWalk", false);
                        rb.velocity = new Vector3(0, -1, 0);
                        setStatus(AIStatus.fear);
                    }
                }
                break;
            case AIStatus.inMouth:
                {
                    anim.SetBool("isWalk", false);
                    anim.SetBool("inMouth", true);
                    anim.SetBool("isFear", false);
                    rb.velocity = Vector3.zero;
                }
                break;
            case AIStatus.attracted:
                {
                    moveTo(targetPosition);
                }
                break;
        }
    }

    void moveTo(Vector3 targetPosition) 
    {
        if ((targetPosition - transform.position).magnitude > 0.8f)
        {
            anim.SetBool("isWalk", true);
            var targetDirection = targetPosition - transform.position;
            targetDirection.y = 0;
            targetDirection = targetDirection.normalized;
            var deltaAngle = Vector3.SignedAngle(new Vector3(targetDirection.x, 0, targetDirection.z), transform.forward, -Vector3.up);
            if (deltaAngle > 5)
            {
                transform.Rotate(Vector3.up, Time.deltaTime * rotateSpeed);
            }
            else if (deltaAngle < -5)
            {
                transform.Rotate(Vector3.up, Time.deltaTime * -rotateSpeed);
            }
            else
            {
                transform.forward = targetDirection;
            }
            rb.velocity = transform.forward * speed;
            transform.forward = rb.velocity.normalized;
            var vel = rb.velocity;
            vel.y = -1;
            rb.velocity = vel;

        }
        else
        {
            anim.SetBool("isWalk", false);
            rb.velocity = new Vector3(0, -1, 0);
            anim.SetFloat("Attracted", status == AIStatus.attracted ? 1 : 0);
        }

        //fear when too far from player
        if ((player.position - transform.position).magnitude > fearDistance)
        {
            fear += 1 * Time.deltaTime / feartime;
        }
        if (fear >= 1)
        {
            anim.SetBool("isWalk", false);
            rb.velocity = new Vector3(0, -1, 0);
            setStatus(AIStatus.fear);
        }
    }

    public void Comfort()
    {
        if (fear > 0)
        {
            fear = Mathf.Max(0, fear - (fearChangeRate * Time.deltaTime));
            FMOD.Studio.PLAYBACK_STATE state;
            comfortSound.getPlaybackState(out state);
            if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
            {
                fearSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                comfortSound.start();
            }

            FMOD.Studio.PLAYBACK_STATE playState = FMOD.Studio.PLAYBACK_STATE.STOPPED;
            purrSound.getPlaybackState(out playState);
            if (playState != FMOD.Studio.PLAYBACK_STATE.PLAYING) purrSound.start();
        }
    }

    public void setStatus(AIStatus status) 
    {
        this.status = status;

        // play fear sound
        if (status == AIStatus.fear) fearSound.start();
        else fearSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

    }

    public void setStatus(AIStatus status, Vector3 position) 
    {
        this.status = status;
        if (status == AIStatus.attracted) 
        {
            targetPosition = position;
        }
    }

    public void SetHighLight(bool isHighLight)
    {
        material.SetFloat("_RimLight", isHighLight ? 1 : 0);
        return;
    }
}
