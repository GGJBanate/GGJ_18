using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Messenger : MonoBehaviour {

	public GameObject view;
	public GameObject input;
	private float textOffset;
	private InputField field;

	public GameObject OwnTextPrefab;
	public GameObject PartnerTextPrefab;

	// Use this for initialization
	void Start () {
		field = input.GetComponent<InputField>();
		this.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("return")) {
			send();
		}
	}

	public void focusInputField() {
		field.Select();
		field.ActivateInputField();
	}

	private void send() {
		if(field != null && view != null) {
			displayMsg (field.text);
			// Send via network here
			field.text = "";
			focusInputField ();
		}
	}

	private void displayMsg(string msg) {
		GameObject chatText = Instantiate(OwnTextPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		chatText.GetComponentInChildren<Text>().text = msg;
		chatText.transform.SetParent(view.transform, false);
	}

}
