using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEditor;

public class AIController : MonoBehaviour
{
    public float fearDistance;
    Transform player;
    public enum AIStatus { follow, fear, idle, inMouth };
    public AIStatus status;
    public float speed;
    public float rotateSpeed;

    [Range(0, 1)] public float fear;
    [SerializeField] private float fearChangeRate;

    public Animator anim;
    private Rigidbody rb;
    [SerializeField] private ParticleSystem comfortVFX;

    // The fear sound
    [FMODUnity.EventRef]
    public string fearSoundPath;
    FMOD.Studio.EventInstance fearSound;

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
    }

    // Update is called once per frame
    void Update()
    {
        // Change the volume of the sound
        fearSound.setParameterByName("Distance", fear);

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
                    }
                }
                break;
            case AIStatus.follow:
                {
                    if((player.position - transform.position).magnitude > 0.8f)
                    {
                        anim.SetBool("isWalk", true);
                        var targetDirection = player.position - transform.position;
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
                    }
                    //fear when too far from player
                    if ((player.position - transform.position).magnitude > fearDistance)
                    {
                        fear += 1 * Time.deltaTime;
                    }
                    if (fear >= 1)
                    {
                        anim.SetBool("isWalk", false);
                        rb.velocity = new Vector3(0, -1, 0);
                        setStatus(AIStatus.fear);
                    }
                }
                break;
            case AIStatus.idle:
                {
                    anim.SetBool("inMouth", false);
                    anim.SetBool("isFear", false);
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
                        fear += 1 * Time.deltaTime;
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
        }
    }

    public void Comfort()
    {
        if (fear > 0)
        {
            fear = Mathf.Max(0, fear - (fearChangeRate * Time.deltaTime));
            if (!comfortVFX.isPlaying)
            {
                comfortVFX.Play();
            }
        }
    }

    public void setStatus(AIStatus status) 
    {
        this.status = status;
        if (status == AIStatus.fear) { fearSound.start(); }
    }
}
