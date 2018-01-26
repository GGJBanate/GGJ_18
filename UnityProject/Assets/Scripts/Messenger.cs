using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Messenger : MonoBehaviour {

	public GameObject view;
	public GameObject input;
	private float textOffset;

	public GameObject ChatTextPrefab;

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
			string text = input.GetComponent<InputField>().text;
			Debug.Log(text);
			GameObject chatText = Instantiate(ChatTextPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			chatText.GetComponent<Text>().text = text;
			chatText.transform.SetParent(view.transform, false);
			input.GetComponent<InputField>().text = "";
			input.GetComponent<InputField>().Select();
		}
	}
}
