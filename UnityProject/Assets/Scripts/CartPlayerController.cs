using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartPlayerController : MonoBehaviour
{
    public GameObject lamp;

    public ParticleSystem breaksEffect;

    // Use this for initialization
    void Start()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            /*if (!breaksEffect.isPlaying)
            {
            }*/
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}