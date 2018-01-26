using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneButton : MonoBehaviour {

	public GameObject panel;

	public void OnClick() {
		if(panel.activeSelf)
			panel.SetActive(false);
		else
			panel.SetActive(true);
	}
}
