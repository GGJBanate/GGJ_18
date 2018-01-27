using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Messenger : MonoBehaviour {

	public static Messenger Instance { get; private set; }

	public GameObject view;
	public GameObject input;
	private float textOffset;

	private InputField field;

	public GameObject OwnTextPrefab;
	public GameObject PartnerTextPrefab;

	void Awake() {
		Instance = this;
	}

	// Use this for initialization
	void Start () {
		field = input.GetComponent<InputField>();
		this.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Submit")) {
			send();
		}
	}

	public void focusInputField() {
		field.Select();
		field.ActivateInputField();
	}

	public void receiveMsg(string msg) {
		this.gameObject.SetActive (true);
		Debug.Log (msg);
		displayMsg (msg, PartnerTextPrefab);
		focusInputField ();
	}

	private void send() {
	    LocalPlayerNetworkConnection connectionObj = FindObjectsOfType<LocalPlayerNetworkConnection>().First(l => l.isLocalPlayer); 
		if(field != null && view != null) {
			displayMsg (field.text, OwnTextPrefab);
			if (connectionObj != null) {
				Debug.Log ("Sending Message via network");
			    connectionObj.SendChatMessage (field.text);
			} else {
				Debug.Log ("Message was not sent");
			}
			field.text = "";
			focusInputField ();
		}
	}

	private void displayMsg(string msg, GameObject prefab) {
		GameObject chatText = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
		chatText.GetComponentInChildren<Text>().text = msg;
		chatText.transform.SetParent(view.transform, false);
	}

}
