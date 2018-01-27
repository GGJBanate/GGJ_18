using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CartPlayerController : MonoBehaviour
{
    public GameObject lamp;
    public Camera playerCamera;

    public GameObject LeverLeft;
    public GameObject LeverRight;
    public GameObject LeverForward;

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
        UpdateLever();
    }

    private void UpdateLever()
    {
        var switchDirection = -1;
        for (int index = 0; index < currentTrack.nextPieces.Count; index++)
        {
            TrackPiece piece = currentTrack.nextPieces[index];
            if (piece.entryCollider.active) switchDirection = index;
        }

        if (currentTrack.nextPieces.Count == 1)
        {
            LeverLeft.SetActive(false);
            LeverRight.SetActive(false);
            LeverForward.SetActive(false);
            return;
        }

        LeverLeft.SetActive(switchDirection == 0);
        LeverRight.SetActive(switchDirection == 2);
        LeverForward.SetActive(switchDirection == 1);

        if (currentTrack.nextPieces.Count == 2)
        {
            if (switchDirection == 2)
                switchDirection -= 1;

            LeverRight.SetActive(switchDirection == 1);
            LeverForward.SetActive(false);
        }
    }

    private void Move()
    {
        GameServer gameServer = GameServer.Instance;
        if(gameServer.gameStatus == GameStatus.Won || gameServer.gameStatus == GameStatus.GameOver){
            //Nowhere to move now...
            return;
        }
        currentVelocity += (breaking ? -deceleration : acceleration) * Time.deltaTime;
        currentVelocity = Mathf.Clamp(currentVelocity, 0, 1);
        var realSpeed = (topSpeed - minSpeed) * currentVelocity + minSpeed;

        transform.position = Vector3.MoveTowards(transform.position, currentTrack.EndPos, realSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, currentTrack.transform.rotation, 0.1f);

        //Check GameStatus
        switch(currentTrack.type){
            case TrackType.Broken : break;
        }
    }

    public void SetNextSwitch(int switchDirection)
    {
        if (currentTrack.nextPieces.Count == 1) return;
        if (currentTrack.nextPieces.Count == 2 && switchDirection == 2)
            switchDirection -= 1;

        for (int index = 0; index < currentTrack.nextPieces.Count; index++)
        {
            TrackPiece piece = currentTrack.nextPieces[index];
            piece.entryCollider.active = index == switchDirection;
        }
    }
}