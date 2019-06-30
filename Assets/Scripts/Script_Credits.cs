using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Credits : MonoBehaviour
{
	public Transform mCredits;
	public bool mShowCredits;

	// Use this for initialization
	void Start()
	{
		mShowCredits = false;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void ToggleCredits()
	{
		mShowCredits = !mShowCredits;
		mCredits.gameObject.SetActive(mShowCredits);
	}
}
