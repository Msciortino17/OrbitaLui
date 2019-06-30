using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_SoundEffect : MonoBehaviour
{
	public List<AudioClip> mAudioClips;
	private AudioSource mAudioSource;

	// Use this for initialization
	void Start()
	{
		mAudioSource = GetComponent<AudioSource>();
		mAudioSource.volume = GameObject.Find("Game Manager").GetComponent<Script_GameManager>().mSoundVolume;
	}

	// Update is called once per frame
	void Update()
	{
		if (!mAudioSource.isPlaying)
		{
			Destroy(gameObject);
		}
	}

	public void PlaySound(int _index)
	{
		mAudioSource = GetComponent<AudioSource>();
		mAudioSource.clip = mAudioClips[_index];
		mAudioSource.Play();
	}
}
