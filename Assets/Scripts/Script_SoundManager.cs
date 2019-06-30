using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_SoundManager : MonoBehaviour
{

	public List<string> mSoundFileNames;
	public List<string> mLoopingSounds;
	public Dictionary<string, int> mFileIDs;
	public Dictionary<string, int> mSoundIDs;
	public Dictionary<string, bool> mSoundsPlaying;
	public Dictionary<string, bool> mStopThisSound;
	public Dictionary<string, bool> mStartThisSound;
	public Dictionary<string, float> mCurrentVolumes;

	private float mVolume;
	public float mSoundFadeRate;

	public void SetSoundRate(float _rate)
	{
		mSoundFadeRate = _rate;
	}

	Text mDebugText;

	// Use this for initialization
	void Start()
	{
		// Set up Android Native Audio
		AndroidNativeAudio.makePool();

		//mSoundFileNames.Add("Explosion1");

		mFileIDs = new Dictionary<string, int>();
		mSoundIDs = new Dictionary<string, int>();
		mSoundsPlaying = new Dictionary<string, bool>();
		mStopThisSound = new Dictionary<string, bool>();
		mStartThisSound = new Dictionary<string, bool>();
		mCurrentVolumes = new Dictionary<string, float>();

		mVolume = 1.0f;

		for ( int i = 0; i < mSoundFileNames.Count; i++ )
		{
			mFileIDs[mSoundFileNames[i]] = AndroidNativeAudio.load("Sounds/" + mSoundFileNames[i] + ".wav");
			mSoundsPlaying[mSoundFileNames[i]] = false;
			mStopThisSound[mSoundFileNames[i]] = false;
			mStartThisSound[mSoundFileNames[i]] = false;
			mCurrentVolumes[mSoundFileNames[i]] = mVolume;
		}

		//mDebugText = GameObject.Find("AndroidDebug").GetComponent<Text>();
	}

	void Update()
	{
		//string debugText = "";
		//debugText += "Volume: " + mVolume + "\n";
		//debugText += "RT volume: " + mCurrentVolumes["Rocket_Thruster"] + "\n";
		//debugText += "RT playing: " + mSoundsPlaying["Rocket_Thruster"] + "\n";
		//debugText += "RT stop: " + mStopThisSound["Rocket_Thruster"] + "\n";
		//debugText += "Rate: " + mSoundFadeRate + "\n";

		//mDebugText.text = debugText;

		for (int i = 0; i < mLoopingSounds.Count; i++)
		{
			if (mStopThisSound[mLoopingSounds[i]] && mCurrentVolumes[mLoopingSounds[i]] > 0.0f)
			{
				float volume = mCurrentVolumes[mLoopingSounds[i]];
				volume -= Time.deltaTime * mSoundFadeRate;
				if (volume <= 0.0f)
				{
					volume = 0.0f;
					//AndroidNativeAudio.stop(mSoundIDs[mLoopingSounds[i]]);
					mSoundsPlaying[mLoopingSounds[i]] = false;
					mStopThisSound[mLoopingSounds[i]] = false;
				}
				AndroidNativeAudio.setVolume(mSoundIDs[mLoopingSounds[i]], volume);
				mCurrentVolumes[mLoopingSounds[i]] = volume;
			}


			if (mStartThisSound[mLoopingSounds[i]] && mCurrentVolumes[mLoopingSounds[i]] < mVolume)
			{
				float volume = mCurrentVolumes[mLoopingSounds[i]];
				volume += Time.deltaTime * mSoundFadeRate;
				if (volume >= mVolume)
				{
					volume = mVolume;
					mStartThisSound[mLoopingSounds[i]] = false;
				}
				AndroidNativeAudio.setVolume(mSoundIDs[mLoopingSounds[i]], volume);
				mCurrentVolumes[mLoopingSounds[i]] = volume;
			}
		}
	}

	void OnApplicationQuit()
	{
		// Clean up when done
		for ( int i = 0; i < mSoundFileNames.Count; i++ )
		{
			AndroidNativeAudio.unload(mFileIDs[mSoundFileNames[i]]);
		}
		AndroidNativeAudio.releasePool();
	}

	public bool PlaySound( string _sound, bool _looping = false )
	{
		if (mSoundFileNames.Contains(_sound) && !mStopThisSound[_sound] && !mSoundsPlaying[_sound] && !mStartThisSound[_sound])
		{
			mSoundIDs[_sound] = AndroidNativeAudio.play(mFileIDs[_sound]);
			if (_looping)
			{
				mSoundsPlaying[_sound] = true;
				AndroidNativeAudio.setLoop(mSoundIDs[_sound], -1);
				//AndroidNativeAudio.setVolume(mSoundIDs[_sound], mVolume);
				//mCurrentVolumes[_sound] = mVolume;
				mStartThisSound[_sound] = true;
				if (!mLoopingSounds.Contains(_sound))
				{
					mLoopingSounds.Add(_sound);
				}
			}
			return true;
		}

		Debug.Log("SOUND ERROR: " + _sound + " is not a valid sound.");
		return false;
	}

	public bool StopSound(string _sound)
	{
		if (mSoundFileNames.Contains(_sound))
		{
			mStopThisSound[_sound] = true;
			mStartThisSound[_sound] = false;
			return true;
		}

		Debug.Log("SOUND ERROR: " + _sound + " is not a valid sound.");
		return false;
	}

	public void SetVolume( float _volume )
	{
		mVolume = _volume;
		// Clean up when done
		for ( int i = 0; i < mSoundFileNames.Count; i++ )
		{
			AndroidNativeAudio.setVolume(mSoundIDs[mSoundFileNames[i]], _volume);
		}
	}

	public bool IsSoundPlaying(string _sound)
	{
		if (mSoundsPlaying.ContainsKey(_sound))
		{
			return mSoundsPlaying[_sound];
		}
		return false;
	}

}
