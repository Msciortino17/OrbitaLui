using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_ExtraLife : MonoBehaviour
{
	public Script_GameManager mGameManager;
	public float mDespawnTimer;
	public float mForceTimer;
	public Transform mParticleEffect;

	public Transform mSoundEffectPrefab;

	// Use this for initialization
	void Start()
	{
		Instantiate(mParticleEffect, transform.position, Quaternion.identity);
		mDespawnTimer = Random.Range(0.0f, 10.0f) + 20.0f;
		if (Random.Range(0, 2) == 1)
		{
			transform.GetComponent<Script_SpaceObject>().mControllingVelocity = Vector2.left * Random.Range(0.22f, 0.27f);
		}
		else
		{
			transform.GetComponent<Script_SpaceObject>().mControllingVelocity = Vector2.right * Random.Range(0.22f, 0.27f);
		}
		transform.GetComponent<Script_SpaceObject>().mRotationalSpeed = Random.Range(-50.0f, 50.0f);
		transform.GetComponent<Script_SpaceObject>().mRotationalDrag = 0.0f;
		transform.GetComponent<Script_SpaceObject>().mFreeze = false;
		transform.GetComponent<Script_SpaceObject>().mGameManager = mGameManager;
		mForceTimer = Random.Range(0.05f, 0.08f);

		Transform sound = Instantiate(mSoundEffectPrefab);
		sound.GetComponent<Script_SoundEffect>().PlaySound(5);
	}

	// Update is called once per frame
	void Update()
	{
		if (!mGameManager.mPaused)
		{
			mDespawnTimer -= Time.deltaTime;
			if (mDespawnTimer <= 0.0f || mGameManager.mGameOver)
			{
				Despawn();
			}

			mForceTimer -= Time.deltaTime;
			if (mForceTimer <= 0.0f)
			{
				transform.GetComponent<Script_SpaceObject>().mControllingVelocity = Vector2.zero;
			}
		}
	}

	public void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Ship")
		{
			mGameManager.mLives++;
			Despawn();

			Transform sound = Instantiate(mSoundEffectPrefab);
			sound.GetComponent<Script_SoundEffect>().PlaySound(5);
		}
	}

	private void Despawn()
	{
		Instantiate(mParticleEffect, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}
}
