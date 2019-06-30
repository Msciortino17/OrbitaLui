using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_SpaceShip : MonoBehaviour
{
	
	public Script_GameManager mGameManager;
	public Script_SpaceObject mMySpaceObject;

	Vector2 mThrusterVelocity;
	public float mRotationalSpeedMax;
	public float mRotationalSpeedRate;
	public float mSRBTimer;
	public float mSRBForce;
	public float mThrusterForce;

	public bool mPlayerControlled;
	public float mInputTimer;
	bool mTookOff;
	public int mLapCounter;

	public Transform mParticleThruster;
	public Transform mParticleExplosion;
	private ParticleSystem mPSThruster;

	public Transform mRocketChunkPrefab;
	public Transform mSRBPrefab;
	private Transform mSRB_Left;
	private Transform mSRB_Right;
	public Transform mFaceSprite;

	private AudioSource mAudioSource;
	public bool mAudioPlaying;

	public Transform mSoundEffectPrefab;
	private Script_SoundManager mSoundManager;
	public float mRotationTimer;
	public float mRotationMidTime;
	public float mRotationLongTime;
	public bool mRotateLeftPrev;
	public bool mRotateRightPrev;
	public bool mRotateSoundFlag;

	private Script_HintManager mHintManager;

	public Transform mShipcolorsReference;
	public Transform mSRBLeftColorsReference;
	public Transform mSRBRightColorsReference;
	public Color mShipColor;
	public List<Color> mShipColors;

	public Transform mSRBSmokePoofLeft;
	public Transform mSRBSmokePoofRight;
	public bool mSRBSmokePoofFlag;

	public string mPilotName;
	public string mPilotRank;

	// Use this for initialization
	void Start()
	{
		mThrusterVelocity = Vector2.zero;

		mPlayerControlled = true;
		mTookOff = false;
		mLapCounter = 0;

		mPSThruster = mParticleThruster.GetComponent<ParticleSystem>();
		mPSThruster.Stop();

		mMySpaceObject = GetComponent<Script_SpaceObject>();
		mMySpaceObject.mGameManager = mGameManager;

		mSRB_Left = transform.Find("SRB_Left");
		mSRB_Right = transform.Find("SRB_Right");

		mAudioSource = GetComponent<AudioSource>();
		mAudioPlaying = false;
		mRotateSoundFlag = true;

		mSoundManager = mGameManager.GetComponent<Script_SoundManager>();
		mHintManager = GameObject.Find("Hint").GetComponent<Script_HintManager>();

		mShipColor = mShipColors[Random.Range(0, mShipColors.Count)];
		mShipcolorsReference.GetComponent<SpriteRenderer>().color = mShipColor;
		mSRBLeftColorsReference.GetComponent<SpriteRenderer>().color = mShipColor;
		mSRBRightColorsReference.GetComponent<SpriteRenderer>().color = mShipColor;

		mSRBSmokePoofFlag = true;
	}

	// Update is called once per frame
	void Update()
	{
		mAudioSource.volume = mGameManager.mSoundVolume;

		if (Input.GetKeyDown(KeyCode.I))
		{
			DestroyShip();
		}

		if (!mGameManager.mPaused)
		{
			if (mTookOff)
			{
				UpdateVelocity();
				CheckDistance();
				CheckLap();
				UpdateRotationSound();

				if (mPlayerControlled)
				{
					if (mSRB_Left.GetComponentInChildren<ParticleSystem>().isPaused)
					{
						mSRB_Left.GetComponentInChildren<ParticleSystem>().Play();
					}

					if (mSRB_Right.GetComponentInChildren<ParticleSystem>().isPaused)
					{
						mSRB_Right.GetComponentInChildren<ParticleSystem>().Play();
					}
				}
			}
			else
			{
				if (mInputTimer <= 0.0f)
				{
					if (CheckInputThruster())
					{
						if (!mTookOff)
						{
							mSRB_Left.GetComponentInChildren<ParticleSystem>().Play();
							mSRB_Right.GetComponentInChildren<ParticleSystem>().Play();
							gameObject.layer = 8;
						}

						mMySpaceObject.mFreeze = false;
						mTookOff = true;
					}
				}
				else
				{
					mInputTimer -= Time.deltaTime;
				}
			}
		}
		else
		{
			if (mPlayerControlled)
			{
				if (mAudioSource.isPlaying)
				{
					mAudioSource.Pause();
				}
				
				if (Application.platform != RuntimePlatform.Android)
				{
					if (mAudioSource.isPlaying)
					{
						mAudioSource.Pause();
					}
				}
				else
				{
					if (mSoundManager.IsSoundPlaying("Rocket_Thruster"))
					{
						mSoundManager.StopSound("Rocket_Thruster");
					}
				}

				if (mSRBTimer > 0.0f)
				{
					mSRB_Left.GetComponentInChildren<ParticleSystem>().Pause();
					mSRB_Right.GetComponentInChildren<ParticleSystem>().Pause();
				}
			}
		}

		Debug.DrawRay(transform.position, transform.up * 0.5f, Color.red);

		mRotateLeftPrev = CheckInputRotateLeft();
		mRotateRightPrev = CheckInputRotateRight();

		if (mGameManager.mLives <= 0)
		{
			DestroyShip();
		}
	}

	public void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Ship" && mTookOff)
		{
			DestroyShip();
			if (mGameManager.mScore >= 5 && !mGameManager.mGameOver)
			{
				mGameManager.mScore -= 5;
			}
			if (mHintManager.mDisplayHints && mHintManager.mFlagShipCrash)
			{
				mHintManager.mFlagShipCrash = false;
				mHintManager.AddHint("When 2 ships collide, both of them suffer horribly...");
			}
			if (mHintManager.mDisplayHints && mHintManager.mFlagSpareParts)
			{
				mHintManager.mFlagSpareParts = false;
				mHintManager.AddHint("Those flaming ship parts can be recycled. Collect enough of them for an extra life!");
			}
		}
	}

	public void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "RocketChunk" && !mGameManager.mGameOver)
		{
			collision.gameObject.GetComponent<Script_RocketChunk>().DestroyChunk();
			mGameManager.mOneUpCounter++;
			GameObject.Destroy(collision.gameObject);
			if (Application.platform != RuntimePlatform.Android)
			{
				PlaySound(5);
			}
			else
			{
				mSoundManager.PlaySound("Feedback_Positive");
			}
			// TODO - Spawn a particle effect
		}
	}

	private void UpdateVelocity()
	{
		mThrusterVelocity = Vector2.zero;
		if (mPlayerControlled)
		{
			Vector2 vUp = new Vector2(transform.up.x, transform.up.y);

			if (CheckInputThruster())
			{
				mThrusterVelocity = vUp * mThrusterForce;
				Debug.DrawRay(transform.position, transform.up * 0.5f, Color.red);

				if (!mPSThruster.isPaused)
				{
					mPSThruster.Emit(1);
				}

				if (Application.platform != RuntimePlatform.Android)
				{
					if (!mAudioSource.isPlaying)
					{
						mAudioSource.Play();
					}
				}
				else
				{
					if (!mSoundManager.IsSoundPlaying("Rocket_Thruster"))
					{
						mSoundManager.PlaySound("Rocket_Thruster", true);
					}
				}
			}
			else
			{
				if (Application.platform != RuntimePlatform.Android)
				{
					if (mAudioSource.isPlaying && mSRBTimer <= 0.0f)
					{
						mAudioSource.Pause();
					}
				}
				else
				{
					if (mSoundManager.IsSoundPlaying("Rocket_Thruster") && mSRBTimer <= 0.0f)
					{
						mSoundManager.StopSound("Rocket_Thruster");
					}
				}
			}

			if (CheckInputRotateRight() && mMySpaceObject.mRotationalSpeed > -1.0f * mRotationalSpeedMax)
			{
				mMySpaceObject.mRotationalSpeed -= mRotationalSpeedRate * Time.deltaTime;
			}
			if (CheckInputRotateLeft() && mMySpaceObject.mRotationalSpeed < mRotationalSpeedMax)
			{
				mMySpaceObject.mRotationalSpeed += mRotationalSpeedRate * Time.deltaTime;
			}

			if (mSRBTimer > 0.0f)
			{
				mThrusterVelocity += vUp * mSRBForce;
				mSRBTimer -= Time.deltaTime;
				Debug.DrawRay(transform.position, transform.up * 1.0f, Color.yellow);

				if (mSRBSmokePoofFlag && mSRBTimer <= 0.0f)
				{
					mSRBSmokePoofFlag = false;
					mSRBSmokePoofLeft.GetComponent<ParticleSystem>().Play();
					mSRBSmokePoofRight.GetComponent<ParticleSystem>().Play();
				}

				if (mSRBTimer <= 0.0f)
				{
					Transform newSRB_Left = Instantiate(mSRBPrefab, mSRB_Left.position, mSRB_Left.rotation);
					Transform newSRB_Right = Instantiate(mSRBPrefab, mSRB_Right.position, mSRB_Right.rotation);

					newSRB_Left.parent = transform.parent;
					newSRB_Right.parent = transform.parent;

					newSRB_Left.GetComponent<Script_SpaceObject>().DuplicateSettings(transform.GetComponent<Script_SpaceObject>());
					newSRB_Right.GetComponent<Script_SpaceObject>().DuplicateSettings(transform.GetComponent<Script_SpaceObject>());

					newSRB_Left.GetComponent<Script_SpaceObject>().mRotationalSpeed = 200.0f;// Random.Range(-100.0f, 100.0f);
					newSRB_Right.GetComponent<Script_SpaceObject>().mRotationalSpeed = -200.0f;// Random.Range(-100.0f, 100.0f);

					newSRB_Left.GetComponent<Script_SpaceObject>().mControllingVelocity = newSRB_Left.GetComponent<Script_SpaceObject>().GetPosition() - GetPosition();
					newSRB_Right.GetComponent<Script_SpaceObject>().mControllingVelocity = newSRB_Right.GetComponent<Script_SpaceObject>().GetPosition() - GetPosition();

					newSRB_Left.GetComponent<Script_SRB>().SetSprite(true);
					newSRB_Right.GetComponent<Script_SRB>().SetSprite(false);

					newSRB_Left.GetComponent<Script_SRB>().SetColor(mShipColor);
					newSRB_Right.GetComponent<Script_SRB>().SetColor(mShipColor);

					mSRB_Left.gameObject.SetActive(false);
					mSRB_Right.gameObject.SetActive(false);

					if (mHintManager.mFlagSRB)
					{
						mHintManager.mFlagSRB = false;
						mHintManager.AddHint("Those were your solid rocket boosters. They give you a boost during take off.");
					}

					if (Application.platform != RuntimePlatform.Android)
					{
						Transform sound = Instantiate(mSoundEffectPrefab);
						sound.GetComponent<Script_SoundEffect>().PlaySound(16);
					}
					else
					{
						mSoundManager.PlaySound("Opening_Soda");
					}
				}
			}

			mMySpaceObject.mControllingVelocity = mThrusterVelocity;
		}

	}

	private void UpdateRotationSound()
	{
		mRotationTimer += Time.deltaTime;
		
		if ((CheckInputRotateLeft() && !mRotateLeftPrev) ||
			(CheckInputRotateRight() && !mRotateRightPrev))
		{
			mRotateSoundFlag = true;
			mRotationTimer = 0.0f;
		}
		
		if (mRotateSoundFlag && (CheckInputRotateLeft() || CheckInputRotateRight()))
		{
			if (mRotationTimer > mRotationLongTime)
			{
				mRotateSoundFlag = false;
				
				if (Application.platform != RuntimePlatform.Android)
				{
					Transform sound = Instantiate(mSoundEffectPrefab);
					sound.GetComponent<Script_SoundEffect>().PlaySound(6);
				}
				else
				{
					mSoundManager.PlaySound("Metal_Groan_Long");
				}
			}
		}

		if ((!CheckInputRotateLeft() && mRotateLeftPrev) ||
			(!CheckInputRotateRight() && mRotateRightPrev))
		{
			if (mRotationTimer > mRotationMidTime)
			{
				mRotateSoundFlag = false;
				
				if (Application.platform != RuntimePlatform.Android)
				{
					Transform sound = Instantiate(mSoundEffectPrefab);
					sound.GetComponent<Script_SoundEffect>().PlaySound(7);
				}
				else
				{
					mSoundManager.PlaySound("Metal_Groan_Medium");
				}
			}
			else
			{
				mRotateSoundFlag = false;
				
				if (Application.platform != RuntimePlatform.Android)
				{
					Transform sound = Instantiate(mSoundEffectPrefab);
					sound.GetComponent<Script_SoundEffect>().PlaySound(8);
				}
				else
				{
					mSoundManager.PlaySound("Metal_Groan_Short");
				}
			}
		}
	}

	private void CheckDistance()
	{
		Vector2 toPlanet = mGameManager.mPlanetCenter - GetPosition();
		float distanceToPlanet = toPlanet.magnitude;

		if (distanceToPlanet > mGameManager.mMaxDistance || distanceToPlanet < mGameManager.mPlanetRadius)
		{
			DestroyShip();
			if (distanceToPlanet > mGameManager.mMaxDistance)
			{
				if (mHintManager.mDisplayHints && mHintManager.mFlagDriftTooFar)
				{
					mHintManager.AddHint("Don't fly too far from Earth or else ground control will lose track of you!");
					mHintManager.mFlagDriftTooFar = false;
				}
				if (mHintManager.mDisplayHints && mHintManager.mFlagSpareParts)
				{
					mHintManager.mFlagSpareParts = false;
					mHintManager.AddHint("Those flaming ship parts can be recycled. Collect enough of them for an extra life!");
				}
			}
		}
	}

	private void DestroyShip()
	{
		if (mSoundManager.IsSoundPlaying("Rocket_Thruster"))
		{
			mSoundManager.StopSound("Rocket_Thruster");
		}

		Instantiate(mParticleExplosion, transform.position, Quaternion.identity);
		mGameManager.ShipDestroyed(transform);

		CreateSpaceShipPart(0);
		CreateSpaceShipPart(1);
		CreateSpaceShipPart(2);
		CreateSpaceShipPart(3);
		CreateSpaceShipPart(4);

		//mHintManager.AddHint("RIP " + mPilotRank + " " + mPilotName + ".", true);

		if (mGameManager.mLives > 0 && mPlayerControlled)
		{
			mGameManager.CreateShip();
		}

		Destroy(gameObject);

		if (Application.platform != RuntimePlatform.Android)
		{
			Transform sound = Instantiate(mSoundEffectPrefab);
			sound.GetComponent<Script_SoundEffect>().PlaySound(Random.Range(0, 4));
		}
		else
		{
			mSoundManager.PlaySound("Explosion" + (Random.Range(0, 4) + 1));
		}
	}

	private void CreateSpaceShipPart(int index)
	{
		Transform rocketChunk = Instantiate(mRocketChunkPrefab, transform.position, transform.rotation);
		rocketChunk.parent = transform.parent;
		rocketChunk.GetComponent<Script_SpaceObject>().DuplicateSettings(transform.GetComponent<Script_SpaceObject>());
		rocketChunk.GetComponent<Script_SpaceObject>().mRotationalSpeed = Random.Range(-100.0f, 100.0f);
		rocketChunk.GetComponent<Script_SpaceObject>().mControllingVelocity = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
		rocketChunk.GetComponent<Script_RocketChunk>().SetRocketChunk(index);
		rocketChunk.GetComponent<Script_RocketChunk>().mTimerMax = Random.Range(5.0f, 10.0f);
	}

	private void CheckLap()
	{
		// First checkpoint is crossing the mid horizon of the planet
		if (mLapCounter == 0)
		{
			if (transform.position.y < mGameManager.mPlanetCenter.y)
			{
				mLapCounter = 1;
			}
		}
		// Next check is when we cross the pole while above the horizon
		if (mLapCounter == 1)
		{
			if (Mathf.Abs(transform.position.x - mGameManager.mPlanetCenter.x) < 0.1 &&
				transform.position.y > mGameManager.mPlanetCenter.y)
			{
				mLapCounter = 2;
			}
		}
		// A lap is completed, lose control and spawn a new ship
		if (mLapCounter == 2)
		{
			if (mGameManager.mNumberOfOrbitingShips > mGameManager.mShipsInOrbitRecord)
			{
				mGameManager.mShipsInOrbitRecord = mGameManager.mNumberOfOrbitingShips;
				PlayerPrefs.SetInt("ShipsInOrbitRecord", mGameManager.mShipsInOrbitRecord);
			}

			mPlayerControlled = false;
			mGameManager.CreateShip();
			mLapCounter = 3;

			mMySpaceObject.mControllingVelocity = Vector2.zero;

			
			if (Application.platform != RuntimePlatform.Android)
			{
				if (mAudioSource.isPlaying && mSRBTimer <= 0.0f)
				{
					mAudioSource.Pause();
				}
			}
			else
			{
				if (mSoundManager.IsSoundPlaying("Rocket_Thruster") && mSRBTimer <= 0.0f)
				{
					mSoundManager.StopSound("Rocket_Thruster");
				}
			}

			if (!mGameManager.mTutorialMode)
			{
				mGameManager.mScore += 5 * (mGameManager.mOrbitingShips.Count - 1);
			}

			if (mHintManager.mDisplayHints && mHintManager.mFlagFinishLap)
			{
				mHintManager.mFlagFinishLap = false;
				mHintManager.AddHint("Great work! Now try to get as many into orbit as you can.");
			}
		}
	}

	private Vector2 GetPosition()
	{
		Vector2 adjustedPosition = Vector2.zero;
		adjustedPosition.x = transform.position.x;
		adjustedPosition.y = transform.position.y;
		return adjustedPosition;
	}

	private string GetPilotRank()
	{
		// TODO - Make more options
		return "Major";
	}

	public bool CheckInputThruster()
	{

		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
		{
			if (Input.GetKey(KeyCode.UpArrow))
			{
				return true;
			}
		}
		else
		{
			if (mGameManager.mControlFireBooster)
			{
				return true;
			}
		}

		return false;
	}

	public bool CheckInputRotateLeft()
	{
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
		{
			if (Input.GetKey(KeyCode.LeftArrow))
			{
				return true;
			}
		}
		else
		{
			if (mGameManager.mControlRotateCounterClockwise)
			{
				return true;
			}
		}

		return false;
	}

	public bool CheckInputRotateRight()
	{

		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
		{
			if (Input.GetKey(KeyCode.RightArrow))
			{
				return true;
			}
		}
		else
		{
			if (mGameManager.mControlRotateClockwise)
			{
				return true;
			}
		}

		return false;
	}

	private void PlaySound(int _index)
	{
		Transform sound = Instantiate(mSoundEffectPrefab);
		sound.GetComponent<Script_SoundEffect>().PlaySound(_index);
	}

}
