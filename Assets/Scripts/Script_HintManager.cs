using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_HintManager : MonoBehaviour
{
	// For managing stuff with the UI elements
	public Transform mHintTransformLeft;
	Text mHintTextLeft;
	Image mHintBackdropLeft;
	public Transform mHintTransformRight;
	Text mHintTextRight;
	Image mHintBackdropRight;

	// For managing fading in and out
	//bool mVisible;
	public bool mInvisible;
	public float mFadeSpeed;
	public int mFadeDirection;
	float mCurrentAlpha;

	// For managing the hints themselves
	public Queue<string> mHints;

	// Whether or not hints are enabled
	public bool mDisplayHints;

	// Reference to the game manager
	private Script_GameManager mGameManager;

	// Flags and timers used for displaying various hints
	public bool mFlagThruster;
	public bool mFlagRotation;
	public bool mFlagSRB;
	public bool mFlagDriftTooFar;
	public bool mFlagShipCrash;
	public bool mFlagFinishLap;
	public bool mFlagExtraLife;
	public bool mFlagSpareParts;
	public bool mFlagFinalHint;
	private float mThrusterTimer;
	private float mDriftTooFarTimer;

	public Transform ToggleShowHints;

	public float mHintTimer;
	public bool mDisplayOnLeft;

	// Use this for initialization
	void Start()
	{
		// Grab references to UI elements
		mHintTextLeft = mHintTransformLeft.GetComponentInChildren<Text>();
		mHintBackdropLeft = mHintTransformLeft.GetComponentInChildren<Image>();
		mHintTextRight = mHintTransformRight.GetComponentInChildren<Text>();
		mHintBackdropRight = mHintTransformRight.GetComponentInChildren<Image>();
		mGameManager = GameObject.Find("Game Manager").GetComponent<Script_GameManager>();

		// Set initial state for variables
		//mVisible = false;
		mInvisible = true;
		mFadeDirection = 0;
		mCurrentAlpha = 0.0f;
		mHintTimer = 0.0f;
		mDisplayOnLeft = false;

		mHints = new Queue<string>();

		mDisplayHints = PlayerPrefs.GetInt("DisplayHints", 1) == 1;

		AddHint("Welcome to OrbitaLui. Tap on these hints to dismiss them.");
		AddHint("Try to fly your ship around the planet.");

		ResetHints();
	}
	
	// Update is called once per frame
	void Update()
	{
		if (!mGameManager.mGameOver)
		{
			if (!mHintBackdropLeft.enabled)
			{
				mHintBackdropLeft.enabled = true;
			}
			if (!mHintTextLeft.enabled)
			{
				mHintTextLeft.enabled = true;
			}
			if (!mHintBackdropRight.enabled)
			{
				mHintBackdropRight.enabled = true;
			}
			if (!mHintTextRight.enabled)
			{
				mHintTextRight.enabled = true;
			}

			UpdateTextBox();

			// If the player is idle for too long and does nothing, tell them how to use thrusters
			if (mFlagThruster)
			{
				mThrusterTimer -= Time.deltaTime;
				if (mThrusterTimer <= 0.0f)
				{
					mFlagThruster = false;
					AddHint("Press and hold the button on the right to activate thrusters.");
				}

				if (mGameManager.mControlFireBooster || Input.GetKey(KeyCode.UpArrow))
				{
					mFlagThruster = false;
				}
			}

			// If the player manages to take off and die without trying to rotate, explain that to them too
			if (mFlagRotation)
			{
				if (mGameManager.mControlRotateClockwise || mGameManager.mControlRotateCounterClockwise ||
					Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
				{
					mFlagRotation = false;
				}

				if (mGameManager.mLives < 5)
				{
					mFlagRotation = false;
					AddHint("Use the buttons on the left to rotate and steer the ship.");
				}
			}

			// If the player hasn't gone too far
			if (mFlagDriftTooFar)
			{
				mDriftTooFarTimer -= Time.deltaTime;
				if (mDriftTooFarTimer <= 0.0f)
				{
					AddHint("Don't fly too far from Earth or else ground control will lose track of you!");
					mFlagDriftTooFar = false;
				}
			}

			// If all hints have been shown, tell the player how to turn them off
			if (!mFlagThruster && !mFlagRotation && !mFlagSRB && !mFlagDriftTooFar && !mFlagShipCrash &&
				!mFlagFinishLap && !mFlagExtraLife && !mFlagSpareParts && mFlagFinalHint)
			{
				mFlagFinalHint = false;
				AddHint("That's all of the hints there are, you can turn them off in the options menu.");
			}

		}
		else
		{
			if (mHintBackdropLeft.enabled)
			{
				mHintBackdropLeft.enabled = false;
			}
			if (mHintTextLeft.enabled)
			{
				mHintTextLeft.enabled = false;
			}
			if (mHintBackdropRight.enabled)
			{
				mHintBackdropRight.enabled = false;
			}
			if (mHintTextRight.enabled)
			{
				mHintTextRight.enabled = false;
			}
		}
	}

	public void ResetHints()
	{
		mFlagThruster = true;
		mFlagRotation = true;
		mFlagSRB = true;
		mFlagDriftTooFar = true;
		mFlagShipCrash = true;
		mFlagFinishLap = true;
		mFlagExtraLife = true;
		mFlagSpareParts = true;
		mFlagFinalHint = true;

		mThrusterTimer = 8.0f;
		mDriftTooFarTimer = 30.0f;
	}

	void UpdateTextBox()
	{
		// Start counting down the timer when fully visible. When we reach 0, move onto the next hint.
		if (mFadeDirection == 0)
		{
			if (mHintTimer > 0.0f)
			{
				mHintTimer -= Time.deltaTime;
				if (mHintTimer <= 0.0f)
				{
					mHintTimer = 0.0f;
					FadeOut();
				}
			}
			return;
		}

		// Adjust the new opacity
		mCurrentAlpha += Time.deltaTime * mFadeDirection * mFadeSpeed;

		// Apply the new opacity to the UI colors
		if (mDisplayOnLeft)
		{
			Color textColor = mHintTextLeft.color;
			textColor.a = mCurrentAlpha;
			mHintTextLeft.color = textColor;
			Color imageColor = mHintBackdropLeft.color;
			imageColor.a = mCurrentAlpha;
			mHintBackdropLeft.color = imageColor;
		}
		else
		{
			Color textColor = mHintTextRight.color;
			textColor.a = mCurrentAlpha;
			mHintTextRight.color = textColor;
			Color imageColor = mHintBackdropRight.color;
			imageColor.a = mCurrentAlpha;
			mHintBackdropRight.color = imageColor;
		}
		
		// Stop when we have maxed out opacity either fully visible or not
		if (mFadeDirection == 1 && mCurrentAlpha >= 1.0f)
		{
			mCurrentAlpha = 1.0f;
			mFadeDirection = 0;
			//mVisible = true;
		}
		else if (mFadeDirection == -1 && mCurrentAlpha <= 0.0f)
		{
			mCurrentAlpha = 0.0f;
			mFadeDirection = 0;
			mInvisible = true;

			// Update to the next hint if we have one
			if (mHints.Count > 0)
			{
				FadeIn();
				if (mDisplayOnLeft)
				{
					mHintTextLeft.text = mHints.Dequeue();
				}
				else
				{
					mHintTextRight.text = mHints.Dequeue();
				}
			}
		}
	}

	public void FadeIn()
	{
		mFadeDirection = 1;
		mHintTimer = 3.0f;
		mInvisible = false;
		//mDisplayOnLeft = !mDisplayOnLeft;
	}

	public void FadeOut()
	{
		mFadeDirection = -1;
		//mVisible = false;
	}

	public void AddHint(string _hint, bool overridedisplay = false)
	{
		// Don't accumulate hints if they aren't enabled
		if (!mDisplayHints && !overridedisplay)
		{
			return;
		}

		mHints.Enqueue(_hint);

		// If no hints are currently being displayed, show this one right away!
		if (mInvisible)
		{
			FadeIn();
			if (mDisplayOnLeft)
			{
				mHintTextLeft.text = mHints.Dequeue();
			}
			else
			{
				mHintTextRight.text = mHints.Dequeue();
			}
		}
	}

	public void Clear()
	{
		mHints.Clear();
		FadeOut();
	}

	public void ShowHints(bool _showHints)
	{
		mDisplayHints = _showHints;
		PlayerPrefs.SetInt("DisplayHints", _showHints ? 1 : 0);
		Debug.Log("Setting: " + (_showHints ? 1 : 0));

		//mHintBackdrop.enabled = _showHints;
		//mHintText.enabled = _showHints;
	}
}
