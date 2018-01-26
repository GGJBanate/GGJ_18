using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartPlayerController : MonoBehaviour
{
    public GameObject lamp;

    public ParticleSystem breaksEffect;

    public double maximumBreakPower = 100;

    public double breakPowerBurnRate = 20;

    private double currentBreakPower;

    // Use this for initialization
    void Start()
    {
        currentBreakPower = maximumBreakPower;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (breaksEffect != null && !breaksEffect.isPlaying)
            {
                breaksEffect.Play(true);
            }
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            if (breaksEffect != null && breaksEffect.isPlaying)
            {
                breaksEffect.Stop(true);
            }
        }

        if (Input.GetButton("Fire1"))
        {
            currentBreakPower -= Time.deltaTime * breakPowerBurnRate;
        }
        else
        {
            if (currentBreakPower < maximumBreakPower)
                currentBreakPower += Time.deltaTime * breakPowerBurnRate;
        }
    }
}