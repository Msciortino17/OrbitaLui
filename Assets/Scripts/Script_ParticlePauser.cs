using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_ParticlePauser : MonoBehaviour
{
	private Script_GameManager mGameManager;
	private ParticleSystem mParticleSystem;
	public bool mPauseFlag;
	public bool mLooping;

	// Use this for initialization
	void Start()
	{
		mGameManager = GameObject.Find("Game Manager").GetComponent<Script_GameManager>();
		mParticleSystem = transform.GetComponent<ParticleSystem>();
		mPauseFlag = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (mGameManager.mPaused)
		{
			mParticleSystem.Pause();
			mPauseFlag = true;
		}
		else
		{
			if (mPauseFlag)
			{
				mParticleSystem.Play();
				if (mLooping)
				{
					mParticleSystem.Stop();
				}
				mPauseFlag = false;
			}
		}
	}
}
