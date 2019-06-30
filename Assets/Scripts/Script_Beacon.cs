using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Beacon : MonoBehaviour
{
	public float mTimer;
	public float mLapValue;
	public Script_GameManager mGameManager;

	// Use this for initialization
	void Start()
	{
		mLapValue = (Mathf.PI * 2.0f);
	}

	// Update is called once per frame
	void Update()
	{
		mTimer += Time.deltaTime / 2.0f;
		if (mTimer > mLapValue)
		{
			mTimer -= mLapValue;
		}

		Vector3 theVector = new Vector3(Mathf.Cos(mTimer) * mGameManager.mMaxDistance, Mathf.Sin(mTimer) * mGameManager.mMaxDistance, 0.0f);
		transform.position = theVector;
		transform.Rotate(Vector3.forward, 0.5f);
		transform.Rotate(Vector3.up, mLapValue);
	}
}
