using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneButton : MonoBehaviour {

	public GameObject panel;

	public void OnClick() {
		if (panel.activeSelf) {
			panel.SetActive (false);
		} else {
			panel.SetActive (true);
			panel.GetComponent<Messenger> ().focusInputField ();
		}
	}
}
