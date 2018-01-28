using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class ControlRoomController : MonoBehaviour
{
    public TrackData track;
    public Dictionary<Pos, TrackPieceData> map;


	public GameObject intro;
	private Text introText;
	private Text pressEnter;
	private Text waitingMessage;
	private Text readyMessage;

	private bool started;
    private TrackMapDisplay mapScreen;

    private LocalPlayerNetworkConnection localPlayer;

    public void Init()
    {
    	mapScreen = GameObject.Find("MapScreen").GetComponent<TrackMapDisplay>();
    	mapScreen.init(map);
    }
	public void Start() {
        localPlayer = FindObjectsOfType<LocalPlayerNetworkConnection>().First(l => l.isLocalPlayer);

        intro = Instantiate (intro);
		intro.transform.SetAsLastSibling ();

		Text[] introTexts = intro.GetComponentsInChildren<Text> ();
		introText = introTexts[0];
		pressEnter = introTexts [1];
		waitingMessage = introTexts [2];
		readyMessage = introTexts [3];
		readyMessage.gameObject.SetActive (false);
	}

	public void Update() {
		if (GameServer.Instance.gameStatus == GameStatus.Waiting) {
			if (Input.GetButtonDown ("Submit")) {
				LocalPlayerNetworkConnection connectionObj = FindObjectsOfType<LocalPlayerNetworkConnection> ().First (l => l.isLocalPlayer);
				connectionObj.StartGame ();
				readyMessage.gameObject.SetActive (true);
				introText.gameObject.SetActive (false);
				pressEnter.gameObject.SetActive (false);
			}
			waitingMessage.gameObject.SetActive (true);

			return;
		} else if (!started) {
			waitingMessage.gameObject.SetActive(false);
			intro.SetActive (false);
			started = true;
		}

	    if (GameServer.Instance.gameStatus == GameStatus.Ongoing && Input.GetButtonDown("BarrierSwitch"))
	        localPlayer.SetBarrier();

	    mapScreen.map = this.map;
	}
}
