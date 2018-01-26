using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Messenger : MonoBehaviour {

	private GameObject view;
	private GameObject input;
	private float textOffset;

	public GameObject ChatTextPrefab;

	// Use this for initialization
	void Start () {
		view = GameObject.FindWithTag("ViewContent");
		input = GameObject.FindWithTag("Input");
		this.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("return")) {
			Debug.Log("Return was pressed");
			send();
		}
	}

	private void send() {
		if(input != null && view != null) {
			Debug.Log("Sending stuff");
			Text text = input.GetComponentInChildren<Text>();
			GameObject chatText = Instantiate(ChatTextPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			chatText.GetComponent<Text>().text = text.text;
			chatText.transform.SetParent(view.transform, false);
			chatText.transform.position = new Vector3(chatText.transform.position.x, chatText.transform.position.y+textOffset, chatText.transform.position.z);
			float textHeight = chatText.GetComponent<RectTransform>().rect.height;
			textOffset -= textHeight;
			Rect rect = view.GetComponent<RectTransform>().rect;
			rect.height += textHeight;
			text.text = "";
		}
	}
}
