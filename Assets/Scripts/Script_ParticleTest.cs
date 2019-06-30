using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_ParticleTest : MonoBehaviour
{
	public Transform mParticleSystem;
	ParticleSystem mPS;

	// Use this for initialization
	void Start()
	{
		mPS = mParticleSystem.GetComponent<ParticleSystem>();
		mPS.Stop();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.L))
		{
			if (mPS.isPaused)
			{
				mPS.Play();
				mPS.Stop();
			}
			else
			{
				mPS.Pause();
			}
		}

		if (Input.GetKey(KeyCode.P) && !mPS.isPaused)
		{
			mPS.Emit(1);
			if (!mPS.isPlaying)
			{
				//mPS.Play(true);
			}
		}
		else
		{
			if (mPS.isPlaying)
			{
				//mPS.Stop(true);
			}
		}
	}
}
