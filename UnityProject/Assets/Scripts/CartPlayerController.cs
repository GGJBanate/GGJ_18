using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CartPlayerController : MonoBehaviour
{
    public GameObject lamp;
    public Camera playerCamera;

    public ParticleSystem breaksEffect;

    public Image breakPowerBar;

    public float maximumBreakPower = 100;

    public float breakPowerBurnRate = 30;
    public float breakPowerRegenRate = 10;

    public float topSpeed = 10;
    public float minSpeed = 3;
    public float acceleration = 3;
    public float deceleration = 6;

    private float currentVelocity = 0;

    private float currentBreakPower;

    private bool breaking = false;

    [HideInInspector] public TrackPiece currentTrack;

    void Start()
    {
        currentBreakPower = maximumBreakPower;

        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        while (gameObject)
        {
            playerCamera.transform.DOShakePosition(.2f, .03f, 30);

            yield return new WaitForSeconds(.2f);
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Break"))
        {
            if (breaksEffect != null && !breaksEffect.isPlaying)
            {
                breaksEffect.Play(true);
            }

            breaking = true;
        }

        if (Input.GetButton("Break") && currentBreakPower > 0)
        {
            currentBreakPower -= Time.deltaTime * breakPowerBurnRate;
        }
        else
        {
            if (currentBreakPower < maximumBreakPower)
                currentBreakPower += Time.deltaTime * breakPowerRegenRate;

            if (breaksEffect != null && breaksEffect.isPlaying)
            {
                breaksEffect.Stop(true);
            }

            breaking = false;
        }


        Vector3 newScale = breakPowerBar.rectTransform.localScale;
        newScale.y = 1 - currentBreakPower / maximumBreakPower;
        breakPowerBar.rectTransform.localScale = newScale;
        Color barColor = Color.Lerp(Color.yellow, Color.red, newScale.y);
        breakPowerBar.color = barColor;

        Move();
    }

    private void Move()
    {
        currentVelocity += (breaking ? -deceleration : acceleration) * Time.deltaTime;
        currentVelocity = Mathf.Clamp(currentVelocity, 0, 1);
        var realSpeed = (topSpeed - minSpeed) * currentVelocity + minSpeed;

        transform.position = Vector3.MoveTowards(transform.position, currentTrack.EndPos, realSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, currentTrack.transform.rotation, 0.1f);
    }
}