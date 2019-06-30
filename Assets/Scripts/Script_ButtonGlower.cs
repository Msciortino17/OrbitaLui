using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Script_ButtonGlower : MonoBehaviour
{
	// Script and data references
	public Button mButton;
	public ColorBlock mButtonColorBlock;

	// Internel logic and variables
	public float mTimer;
	public float mColorMultiplierNormal;
	public float mColorMultiplierBright;

	// Use this for initialization
	void Start()
	{
		// Grab references
		mButton = GetComponent<Button>();

		// Setup initial set of colors
		mButtonColorBlock = mButton.colors;
		mColorMultiplierNormal = mButtonColorBlock.colorMultiplier;
		mColorMultiplierBright = mButtonColorBlock.colorMultiplier / 2.0f;

		mTimer = 0.0f;
	}

	// Update is called once per frame
	void Update()
	{
		// Keep increasing the time. When it reaches the threshold, brighten things up.
		// Once the player touches the button, it'll calm down and reset
		mTimer += Time.deltaTime;
		if (mTimer > 2.0f)
		{
			mButtonColorBlock.colorMultiplier = mColorMultiplierBright;
			mButton.colors = mButtonColorBlock;
		}
		if (EventSystem.current.currentSelectedGameObject == gameObject)
		{
			mTimer = 0.0f;
			mButtonColorBlock.colorMultiplier = mColorMultiplierNormal;
			mButton.colors = mButtonColorBlock;
		}
	}
}
