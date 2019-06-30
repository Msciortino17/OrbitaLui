using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_UITransition : MonoBehaviour
{
	RectTransform mRect;
	public Vector3 mTarget;
	public float mSpeedConstant;
	public float mBaseSpeed;
	public float mBaseSpeedDirection;
	public bool mMoving;
	public bool mVerticalMovement;

	public float timer;
	public float delay;
	public bool delayed;

	// Use this for initialization
	void Awake()
	{
		mSpeedConstant = 12.0f;
		mBaseSpeed = 0.01f;
		mMoving = false;
		mRect = GetComponent<RectTransform>();
	}

	// Update is called once per frame
	void Update()
	{
		if (delayed)
		{
			delay -= Time.deltaTime;
			if (delay <= 0.0f)
			{
				delayed = false;
				OffSet();
			}
		}

		if (mMoving)
		{
			timer += Time.deltaTime;
			if (mVerticalMovement)
			{
				float difference = mTarget.y - mRect.position.y;
				if (Mathf.Abs(difference) < 0.01f)
				{
					mRect.position = mTarget;
					mMoving = false;
					//Debug.Log("Time: " + timer);
					return;
				}
				mRect.position = new Vector3(mTarget.x, mRect.position.y + ((difference * mSpeedConstant + mBaseSpeed * mBaseSpeedDirection) * Time.deltaTime), mTarget.z);
			}
			else
			{
				float difference = mTarget.x - mRect.position.x;
				if (Mathf.Abs(difference) < 0.01f)
				{
					mRect.position = mTarget;
					mMoving = false;
					//Debug.Log("whoop");
					return;
				}
				mRect.position = new Vector3(mRect.position.x + ((difference * mSpeedConstant + mBaseSpeed * mBaseSpeedDirection) * Time.deltaTime), mTarget.y, mTarget.z);
			}
		}
	}

	void OnEnable()
	{
		if (delay <= 0.001f)
		{
			OffSet();
		}
	}

	private void OffSet()
	{
		mTarget = mRect.position;
		timer = 0.0f;

		float xValue = Mathf.Abs(mRect.position.x);
		float yValue = Mathf.Abs(mRect.position.y);
		if (yValue > xValue)
		{
			if (mRect.position.y >= 0.0f) // Come from top
			{
				mRect.position = new Vector3(mRect.position.x, 10.0f, mTarget.z);
				mBaseSpeed = 1.0f;
			}
			else // Come in from bottom
			{
				mRect.position = new Vector3(mRect.position.x, -10.0f, mTarget.z);
				mBaseSpeed = -1.0f;
			}
			mVerticalMovement = true;
		}
		else
		{
			if (mRect.position.x >= 0.0f) // Come from right
			{
				mRect.position = new Vector3(10.0f, mRect.position.y, mTarget.z);
				mBaseSpeed = 1.0f;
			}
			else // Come in from left
			{
				mRect.position = new Vector3(-10.0f, mRect.position.y, mTarget.z);
				mBaseSpeed = -1.0f;
			}
			mVerticalMovement = false;
		}

		mMoving = true;
	}

	private void OnDisable()
	{
		mRect.position = mTarget;
	}
}
