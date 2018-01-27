using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneButton : MonoBehaviour {

	public GameObject panel;

	void Update() {
		if(Input.GetButtonDown("Submit")) {
			panel.SetActive (true);
			panel.GetComponent<Messenger> ().focusInputField ();
		}
	}

	public void Toggle() {
		if (panel.activeSelf) {
			panel.SetActive (false);
		} else {
			panel.SetActive (true);
			panel.GetComponent<Messenger> ().focusInputField ();
		}
	}
}
