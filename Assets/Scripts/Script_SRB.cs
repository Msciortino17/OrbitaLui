using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_SRB : MonoBehaviour
{

	public float mTimer;
	public float mTimer2;
	public Transform mParticleExplosion;

	Script_GameManager mGameManager;
	public Transform mSoundEffectPrefab;
	Script_SoundManager mSoundManager;
    
    public Transform mLeftSprite;
    public Transform mRightSprite;
	public Transform mLeftColorSprite;
	public Transform mRightColorSprite;

	public Color mColor;

	// Use this for initialization
	void Start()
	{
		GetComponent<Script_SpaceObject>().mFreeze = false;
		//GetComponentInChildren<ParticleSystem>().Play();
		mGameManager = GetComponent<Script_SpaceObject>().mGameManager;
		mSoundManager = mGameManager.GetComponent<Script_SoundManager>();
		mTimer = Random.Range(6.0f, 10.0f);
		mTimer2 = 0.03f;
	}

	// Update is called once per frame
	void Update()
	{
		if (!mGameManager.mPaused)
		{
			mTimer -= Time.deltaTime;
			mTimer2 -= Time.deltaTime;
			if (mTimer <= 0.0f)
			{
				DestroySRB();
			}
			else if (mTimer2 <= 0.0f)
			{
				GetComponent<Script_SpaceObject>().mControllingVelocity = Vector2.zero;
			}
			CheckDistance();
		}
		else
		{

		}
	}

	private void CheckDistance()
	{
		Vector2 toPlanet = mGameManager.mPlanetCenter - GetPosition();
		float distanceToPlanet = toPlanet.magnitude;

		if (distanceToPlanet > mGameManager.mMaxDistance || distanceToPlanet < mGameManager.mPlanetRadius)
		{
			DestroySRB();
		}
	}

	private Vector2 GetPosition()
	{
		Vector2 adjustedPosition = Vector2.zero;
		adjustedPosition.x = transform.position.x;
		adjustedPosition.y = transform.position.y;
		return adjustedPosition;
	}

	private void DestroySRB()
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

    public void SetSprite(bool _isLeft)
    {
        if (_isLeft)
        {
            mRightSprite.gameObject.SetActive(false);
			mRightColorSprite.gameObject.SetActive(false);
		}
        else
        {
            mLeftSprite.gameObject.SetActive(false);
			mLeftColorSprite.gameObject.SetActive(false);
		}
    }

	public void SetColor(Color _color)
	{
		mLeftColorSprite.GetComponent<SpriteRenderer>().color = _color;
		mRightColorSprite.GetComponent<SpriteRenderer>().color = _color;
	}
}
