using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_LoadVolume : MonoBehaviour
{
	public string mPlayerPrefName;

	// Use this for initialization
	void Start()
	{
		GetComponent<Slider>().value = PlayerPrefs.GetFloat(mPlayerPrefName, 1.0f);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
