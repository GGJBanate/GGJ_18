using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CartPlayerController : MonoBehaviour
{
    public float acceleration = 3;

    private bool breaking;

    public Image breakPowerBar;

    public float breakPowerBurnRate = 30;
    public float breakPowerRegenRate = 10;

    public ParticleSystem breaksEffect;

    private float currentBreakPower;

    [HideInInspector] public TrackPiece currentTrack;

    private float currentVelocity;
    public float deceleration = 6;

    public GameObject intro;
    private Text introText;
    public GameObject lamp;
    public GameObject LeverForward;

    public GameObject LeverLeft;
    public GameObject LeverRight;

    private LocalPlayerNetworkConnection localPlayer;

    public float maximumBreakPower = 100;
    public float minSpeed = 3;
    public Camera playerCamera;
    private Text pressEnter;
    private Text readyMessage;

    private bool started;

    public float topSpeed = 10;
    private Text waitingMessage;

    private void Start()
    {
        localPlayer = FindObjectsOfType<LocalPlayerNetworkConnection>().First(l => l.isLocalPlayer);
        currentBreakPower = maximumBreakPower;
        intro = Instantiate(intro);
        intro.transform.SetAsLastSibling();

        Text[] introTexts = intro.GetComponentsInChildren<Text>();
        introText = introTexts[0];
        pressEnter = introTexts[1];
        waitingMessage = introTexts[2];
        readyMessage = introTexts[3];
        readyMessage.gameObject.SetActive(false);
    }

    private IEnumerator Shake()
    {
        while (gameObject)
        {
            playerCamera.transform.DOShakePosition(.2f, .03f, 30);

            yield return new WaitForSeconds(.2f);
        }
    }

    private void Update()
    {
        if (GameServer.Instance.gameStatus != GameStatus.Ongoing)
        {
            if (Input.GetButtonDown("Submit"))
            {
                LocalPlayerNetworkConnection connectionObj =
                    FindObjectsOfType<LocalPlayerNetworkConnection>().First(l => l.isLocalPlayer);
                connectionObj.StartGame();
                readyMessage.gameObject.SetActive(true);
                introText.gameObject.SetActive(false);
                pressEnter.gameObject.SetActive(false);
            }

            waitingMessage.gameObject.SetActive(true);

            return;
        }

        if (!started)
        {
            waitingMessage.gameObject.SetActive(false);
            intro.SetActive(false);
            StartCoroutine(Shake());
            started = true;
        }

        if (Input.GetButtonDown("Break"))
        {
            if (breaksEffect != null && !breaksEffect.isPlaying)
                breaksEffect.Play(true);

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
                breaksEffect.Stop(true);

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
        if (gameServer.gameStatus != GameStatus.Ongoing)
            //Nowhere to move now...
            return;

        currentVelocity += (breaking ? -deceleration : acceleration) * Time.deltaTime;
        currentVelocity = Mathf.Clamp(currentVelocity, 0, 1);
        var realSpeed = (topSpeed - minSpeed) * currentVelocity + minSpeed;

        transform.position = Vector3.MoveTowards(transform.position, currentTrack.EndPos, realSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, currentTrack.transform.rotation, 0.1f);

        //Check and Change GameStatus
        UpdateGameStatus();
    }

    private void ChangeStateAtEndTo(GameStatus newStatus)
    {
        if (currentTrack.EndPos == transform.position)
        {
            localPlayer.SetGameStatus(newStatus);
            if (newStatus == GameStatus.GameOver)
                DropPlayer();
        }
    }

    private void UpdateGameStatus()
    {
        switch (currentTrack.type)
        {
            case TrackType.Finish:
                ChangeStateAtEndTo(GameStatus.Won);
                break;
            case TrackType.DeadEnd:
                ChangeStateAtEndTo(GameStatus.GameOver);
                break;
            default: break;
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

    public void OnTriggerEnter(Collider other)
    {
        var realSpeed = (topSpeed - minSpeed) * currentVelocity + minSpeed;

        if (other.gameObject.tag == "BarrierArm")
        {
            localPlayer.SetGameStatus(GameStatus.GameOver);
        }
        else if (other.gameObject.tag == "Hole")
        {
            if (realSpeed < topSpeed / 2)
            {
                localPlayer.SetGameStatus(GameStatus.GameOver);
                DropPlayer();
            }
        }
        else if (other.gameObject.tag == "Dangerous")
        {
            if (realSpeed > topSpeed / 2)
            {
                localPlayer.SetGameStatus(GameStatus.GameOver);
                DropPlayer();
            }
        }
    }

    private void DropPlayer()
    {
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;
    }
}