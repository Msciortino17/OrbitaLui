using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_RocketChunk : MonoBehaviour
{
	public float mTimer;
	public float mTimerMax;
	Script_GameManager mGameManager;

	public List<Sprite> mRocketSprites;
	public Transform mParticleExplosion;
	public Transform mSoundEffectPrefab;
	Script_SoundManager mSoundManager;

	// Use this for initialization
	void Start()
	{
		GetComponent<Script_SpaceObject>().mFreeze = false;
		mGameManager = GetComponent<Script_SpaceObject>().mGameManager;
		mSoundManager = mGameManager.GetComponent<Script_SoundManager>();
	}

	// Update is called once per frame
	void Update()
	{
		if (!mGameManager.mPaused)
		{
			mTimer += Time.deltaTime;
			if (mTimer >= mTimerMax)
			{
				DestroyChunk();
			}
			else if (mTimer >= 0.03f)
			{
				GetComponent<Script_SpaceObject>().mControllingVelocity = Vector2.zero;
			}
			CheckDistance();
		}
	}

	public void SetRocketChunk(int type)
	{
		GetComponentInChildren<SpriteRenderer>().sprite = mRocketSprites[type];
	}

	private void CheckDistance()
	{
		Vector2 toPlanet = mGameManager.mPlanetCenter - GetPosition();
		float distanceToPlanet = toPlanet.magnitude;

		if (distanceToPlanet < mGameManager.mPlanetRadius)
		{
			DestroyChunk();
		}
	}

	private Vector2 GetPosition()
	{
		Vector2 adjustedPosition = Vector2.zero;
		adjustedPosition.x = transform.position.x;
		adjustedPosition.y = transform.position.y;
		return adjustedPosition;
	}

	public void DestroyChunk()
	{
		Destroy(gameObject);
		Instantiate(mParticleExplosion, transform.position, Quaternion.identity);
		
		if (Application.platform != RuntimePlatform.Android)
		{
			Transform sound = Instantiate(mSoundEffectPrefab);
			sound.GetComponent<Script_SoundEffect>().PlaySound(Random.Range(10, 15));
		}
		else
		{
			mSoundManager.PlaySound("FireWorks" + (Random.Range(0, 6) + 1));
		}
	}
}
