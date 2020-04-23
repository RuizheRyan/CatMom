using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kitten : MonoBehaviour
{
    [Range(0,1)] public float fear;
    [SerializeField] private float fearChangeRate;
    private Animator anim;
    [SerializeField] private ParticleSystem comfortVFX;

    public void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    public void Update()
    {
        anim.SetFloat("fear",fear);    
    }

    public void Comfort()
    {
        if (fear>0)
        {
            fear = Mathf.Max(0,fear - (fearChangeRate * Time.deltaTime));
            if (!comfortVFX.isPlaying)
            {
                comfortVFX.Play();
            }
        }
    }
}
