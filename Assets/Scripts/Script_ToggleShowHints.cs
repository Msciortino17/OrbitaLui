using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_ToggleShowHints : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{
		if (PlayerPrefs.GetInt("DisplayHints", 1) != 1)
		{
			GetComponent<Toggle>().isOn = false;
		}
	}

	// Update is called once per frame
	void Update()
	{

	}
}
