using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ControlRoomController : MonoBehaviour
{
    public GameObject intro;
    private Text introText;

    private LocalPlayerNetworkConnection localPlayer;
    public Dictionary<Pos, TrackPieceData> map;
    private TrackMapDisplay mapScreen;
    private Text pressEnter;
    private Text readyMessage;

    private bool started;
    public TrackData track;
    private Text waitingMessage;

    public void Init()
    {
        mapScreen = GameObject.Find("MapScreen").GetComponent<TrackMapDisplay>();
        mapScreen.init(map);
    }

    public void Start()
    {
        localPlayer = FindObjectsOfType<LocalPlayerNetworkConnection>().First(l => l.isLocalPlayer);

        intro = Instantiate(intro);
        intro.transform.SetAsLastSibling();

        Text[] introTexts = intro.GetComponentsInChildren<Text>();
        introText = introTexts[0];
        pressEnter = introTexts[1];
        waitingMessage = introTexts[2];
        readyMessage = introTexts[3];
        readyMessage.gameObject.SetActive(false);
    }

    public void Update()
    {
        if (GameServer.Instance.gameStatus == GameStatus.Waiting)
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
            started = true;
        }

        if (GameServer.Instance.gameStatus == GameStatus.Ongoing && Input.GetButtonDown("BarrierSwitch"))
            localPlayer.SetBarrier();

        mapScreen.map = map;
    }
}