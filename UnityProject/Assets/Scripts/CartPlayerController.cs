using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartPlayerController : MonoBehaviour
{
    public GameObject lamp;

    public ParticleSystem breaksEffect;

    public float maximumBreakPower = 100;

    public float breakPowerBurnRate = 20;

    public float topSpeed = 10;
    public float minSpeed = 3;
    public float acceleration = 3;

    private float currentSpeed = 0;

    private float currentBreakPower;

    private bool breaking = false;

    [HideInInspector] public TrackPiece currentTrack;

    void Start()
    {
        currentBreakPower = maximumBreakPower;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (breaksEffect != null && !breaksEffect.isPlaying)
            {
                breaksEffect.Play(true);
            }
            breaking = true;
        }

        if (Input.GetButton("Fire1") && currentBreakPower > 0)
        {
            currentBreakPower -= Time.deltaTime * breakPowerBurnRate;
        }
        else
        {
            if (currentBreakPower < maximumBreakPower)
                currentBreakPower += Time.deltaTime * breakPowerBurnRate;

            if (breaksEffect != null && breaksEffect.isPlaying)
            {
                breaksEffect.Stop(true);
            }

            breaking = false;
        }

        Move();
    }

    private void Move()
    {
        currentSpeed += (breaking ? -1 : 1) * acceleration * Time.deltaTime;
        var realSpeed = Mathf.Lerp(minSpeed, topSpeed, currentSpeed);

        transform.position = Vector3.MoveTowards(transform.position, currentTrack.EndPos, realSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, currentTrack.transform.rotation, 0.1f);
    }
}