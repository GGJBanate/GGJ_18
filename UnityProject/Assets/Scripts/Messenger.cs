using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Messenger : MonoBehaviour {

	public GameObject view;
	public GameObject input;
	private float textOffset;

	public GameObject OwnTextPrefab;
	public GameObject PartnerTextPrefab;

	// Use this for initialization
	void Start () {
		this.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("return")) {
			send();
		}
	}

	private void send() {
		if(input != null && view != null) {
			InputField field = input.GetComponent<InputField>();
			displayMsg (field.text);
			// Send via network here
			field.text = "";
			field.Select();
			field.ActivateInputField();
		}
	}

	private void displayMsg(string msg) {
		GameObject chatText = Instantiate(OwnTextPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		chatText.GetComponentInChildren<Text>().text = msg;
		chatText.transform.SetParent(view.transform, false);
	}
}
