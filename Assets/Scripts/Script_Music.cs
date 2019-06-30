using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Music : MonoBehaviour
{
	AudioSource mAudioSource;
	public List<AudioClip> mSongs;
	public int lastSong;

	// Use this for initialization
	void Start()
	{
		lastSong = -1;
		mAudioSource = GetComponent<AudioSource>();
		PlaySong();
	}

	// Update is called once per frame
	void Update()
	{
		if (!mAudioSource.isPlaying)
		{
			PlaySong();
		}
	}

	public void PlaySong()
	{
		int songIndex = lastSong;
		while (songIndex == lastSong)
		{
			songIndex = Random.Range(0, mSongs.Count);
		}
		lastSong = songIndex;
		mAudioSource.clip = mSongs[songIndex];
		mAudioSource.Play();
	}
}
