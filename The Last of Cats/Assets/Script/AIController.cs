using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    Transform player;
    public enum AIStatus { follow, fear, idle, inMouth };
    public AIStatus status;
    public float speed;
    public float rotateSpeed;

    [Range(0, 1)] public float fear;
    [SerializeField] private float fearChangeRate;

    private Animator anim;
    private Rigidbody rb;
    [SerializeField] private ParticleSystem comfortVFX;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectsWithTag("Player")[0].transform;
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (status)
        {
            case AIStatus.fear:
                {
                    anim.SetBool("isFear", true);
                    if(fear <= 0)
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
                        var targetOrientation = (player.position - transform.position).normalized;
                        var deltaAngle = Vector3.SignedAngle(new Vector3(targetOrientation.x, 0, targetOrientation.z), transform.forward, -Vector3.up);
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
                            transform.forward = targetOrientation;
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
                    if ((player.position - transform.position).magnitude > 8f)
                    {
                        fear += 1 * Time.deltaTime;
                    }
                    if (fear >= 1)
                    {
                        anim.SetBool("isWalk", false);
                        rb.velocity = new Vector3(0, -1, 0);
                        status = AIStatus.fear;
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
                        status = AIStatus.fear;
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
}
