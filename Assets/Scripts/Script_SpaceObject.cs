using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_SpaceObject : MonoBehaviour
{
	public Script_GameManager mGameManager;

	public Vector2 mFinalVelocity;
	public Vector2 mPreviousVelocity;
	public Vector2 mGravityVelocity;
	public Vector2 mControllingVelocity;

	public float mRotationalSpeed;
	public float mRotationalDrag;

	public bool mFreeze;
	public bool mPauseFlag;

	// Use this for initialization
	void Start()
	{
		//mFinalVelocity = Vector2.zero;
		//mPreviousVelocity = Vector2.zero;
		//mGravityVelocity = Vector2.zero;
		//mControllingVelocity = Vector2.zero;

		mFreeze = true;
		mPauseFlag = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (mGameManager.mPaused && !mFreeze)
		{
			mFreeze = true;
			mPauseFlag = true;
		}

		if (!mGameManager.mPaused && mPauseFlag)
		{
			mFreeze = false;
			mPauseFlag = false;
		}

		if (!mFreeze)
		{
			UpdateVelocity();
		}
	}

	private void UpdateVelocity()
	{
		// Movement
		mFinalVelocity = mPreviousVelocity;

		mGravityVelocity = mGameManager.mPlanetCenter - GetPosition();
		mGravityVelocity.Normalize();
		mGravityVelocity *= mGameManager.mGravitationalForce;
		Debug.DrawRay(transform.position, mGravityVelocity * 100.0f, Color.green);
		
		mFinalVelocity += mControllingVelocity;
		mFinalVelocity += mGravityVelocity;

		mPreviousVelocity = mFinalVelocity;

		Vector3 translation = Vector3.zero;
		translation.x = mFinalVelocity.x;
		translation.y = mFinalVelocity.y;
		transform.Translate(translation * Time.deltaTime, Space.World);

		Debug.DrawRay(transform.position, mFinalVelocity, Color.blue);


		// Rotation
		mRotationalSpeed -= mRotationalSpeed * mRotationalDrag * Time.deltaTime;
		if (Mathf.Abs(mRotationalSpeed) < 0.5f)
		{
			mRotationalSpeed = 0.0f;
		}

		transform.Rotate(transform.forward, mRotationalSpeed * Time.deltaTime);

	}

	public Vector2 GetPosition()
	{
		Vector2 adjustedPosition = Vector2.zero;
		adjustedPosition.x = transform.position.x;
		adjustedPosition.y = transform.position.y;
		return adjustedPosition;
	}

	public void DuplicateSettings(Script_SpaceObject _otherObject)
	{
		mFinalVelocity = _otherObject.mFinalVelocity;
		mPreviousVelocity = _otherObject.mPreviousVelocity;
		mGravityVelocity = _otherObject.mGravityVelocity;
		mControllingVelocity = _otherObject.mControllingVelocity;
		mGameManager = _otherObject.mGameManager;
	}
}
