using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct HighScore
{
	public string mName;
	public int mScore;
}

public class Script_GameManager : MonoBehaviour
{

	public Transform mSpaceShipPrefab;
	public Transform mExtraLifePrefab;
	public Vector3 mShipSpawnPoint;
	public List<Transform> mOrbitingShips;
	public int mNumberOfOrbitingShips;

	public Vector2 mPlanetCenter;
	public float mPlanetRadius;
	public float mMaxDistance;
	public float mGravitationalForce;

	public int mScore;
	public HighScore[] mHighScores;

	public int mLives;
	public float mExtraLifeTimer;
	public Vector2 mExtraLifeSpawnPoint;

	public enum GameState { Gameplay, Menu, Options, Shop, HighScore };
	public GameState mGameState;
	public bool mPaused;

	public Transform mMainMenu;
	public Transform mOptionsMenu;
	public Transform mShopMenu;
	public Transform mGameplayMenu;
	public Transform mHighScoreMenu;
	public Transform mGameOverUI;
	public Transform mTutorialUI;
	public Transform mPauseMenu;
	public Transform mOutOfPlaysMenu;

	public Transform mGUILives;
	public Transform mGUIScore;
    public Transform mGUIScrap;
	public Transform mGUIPlaysLeftMainMenu;
	public Transform mGUIPlaysLeftGameOver;
	public Transform mGUIPlaysLeftOutOfPlays;
	public Transform mGUIHighScore;

	public Transform mFaceBookStuffUI;
	public Transform mProfilePicture;
	public Transform mCurrentPilotText;
	public Transform mTutorialText;
	public Transform mTutorialTextFrame;
	public Transform mTutorialNextButton;
	public Transform mButtonShipThruster;
	public Transform mButtonShipRotateCW;
	public Transform mButtonShipRotateCCW;
	public Transform mHUD;

	public string mName;
	public Transform mInputFieldName;
	public Transform mInputFieldName2;

	public float mMusicVolume;
	public float mSoundVolume;

	public Transform mSoundEffectPrefab;
	private Script_SoundManager mSoundManager;

	public Transform mParticleBeacon;
	public Transform mParticleBeaconParent;
	public Transform mGamePlayObjectParent;

	public bool mControlFireBooster;
	public bool mControlRotateClockwise;
	public bool mControlRotateCounterClockwise;

	public bool mFirstTimePlaying;
	public bool mTutorialMode;
	public bool mTutorialFlag;
	public int mTutorialStep;
	public bool mTutorialShipCrashed;
	public int mTutorialCrashedShipCount;
	public Transform mControllingShip;

	public Transform mShipRotationText;

	//public Script_AdManager mAdManager;
	public float mGameOverTimer;
	public float mGameOverOpacityRate;

    public bool mBackButton;

	public Transform mHintManagerReference;
	Script_HintManager mHintManager;

	public int mShipsInOrbitRecord;
	public Transform mEarthReference;

	public int mOneUpCounter;

	public bool mGameOver;

	private bool mStartupFlag;

	// Use this for initialization
	void Start()
	{
		mStartupFlag = true;
		Debug.Log("Current platform: " + Application.platform);

		//Screen.orientation = ScreenOrientation.LandscapeRight;

		mOrbitingShips = new List<Transform>();

		mGameState = GameState.Menu;

		for (int i = 0; i < 36; i += 2)
		{
			Transform particle = Instantiate(mParticleBeacon, new Vector3(Mathf.Cos(i * 10), Mathf.Sin(i * 10)) * mMaxDistance, Quaternion.identity);
			particle.GetComponent<Script_Beacon>().mGameManager = this;
			particle.GetComponent<Script_Beacon>().mTimer = (Mathf.PI * 2.0f) * (i / 36.0f);
			particle.Rotate(Vector3.forward, i * 10.0f);
			particle.parent = mParticleBeaconParent;
		}

		mPaused = false;
		mHighScores = new HighScore[10];
		mShipsInOrbitRecord = PlayerPrefs.GetInt("ShipsInOrbitRecord", 0);
		LoadHighScores();

		mName = PlayerPrefs.GetString("name", "Player");
		mInputFieldName.GetComponent<InputField>().text = mName;
		mInputFieldName2.GetComponent<InputField>().text = mName;

		mControlFireBooster = false;
		mControlRotateClockwise = false;
		mControlRotateCounterClockwise = false;

		mExtraLifeTimer = 30.0f;

		mMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
		mSoundVolume = PlayerPrefs.GetFloat("SoundVolume", 1.0f);

		GetComponent<AudioSource>().volume = mMusicVolume;

		GetComponent<AudioSource>().Play();

		mFirstTimePlaying = false;
		mTutorialMode = false;
		mTutorialFlag = true;
		mTutorialStep = 0;
		mTutorialCrashedShipCount = 0;
		mTutorialShipCrashed = false;

		mSoundManager = GetComponent<Script_SoundManager>();

		mGameOverTimer = 0.0f;
		mGameOverOpacityRate = -1.5f;

        mBackButton = false;

		mHintManager = mHintManagerReference.GetComponent<Script_HintManager>();

		mEarthReference = GameObject.Find("Earth").transform;

		mOneUpCounter = 0;

		mGameOver = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.S))
		{
			mScore += 10;
		}

		//mParticleBeaconParent.gameObject.SetActive(mGameState == GameState.Gameplay);

		if (Input.GetKeyDown(KeyCode.Escape) || mBackButton)
		{
            mBackButton = false;
			if (mGameState == GameState.Gameplay && mTutorialMode && mPaused)
			{
				if (Application.platform != RuntimePlatform.Android)
				{
					PlaySound(4);
				}
				else
				{
					mSoundManager.PlaySound("Feedback_Negative");
				}

				mPauseMenu.gameObject.SetActive(true);
				mGameplayMenu.gameObject.SetActive(false);
			}
			else if (mGameState == GameState.Options && mPaused)
			{
				mPauseMenu.gameObject.SetActive(true);
				mGameState = GameState.Gameplay;
				UpdateMenus();
				mGameplayMenu.gameObject.SetActive(false);
				if (Application.platform != RuntimePlatform.Android)
				{
					PlaySound(5);
				}
				else
				{
					mSoundManager.PlaySound("Feedback_Positive");
				}
			}
			else if (mGameState == GameState.Menu)
			{
				ExitGame();
				if (Application.platform != RuntimePlatform.Android)
				{
					PlaySound(4);
				}
				else
				{
					mSoundManager.PlaySound("Feedback_Negative");
				}
			}
			else if (mGameState == GameState.Gameplay && !mPaused)
			{
				mPaused = true;
				if (Application.platform != RuntimePlatform.Android)
				{
					PlaySound(4);
				}
				else
				{
					mSoundManager.PlaySound("Feedback_Negative");
				}
				mPauseMenu.gameObject.SetActive(true);
				mGameplayMenu.gameObject.SetActive(false);
			}
			else if (mGameState == GameState.Gameplay && mPaused)
			{
				Resume();
			}
			else
			{
				mGameplayMenu.gameObject.SetActive(true);
				mPauseMenu.gameObject.SetActive(false);
				mPaused = false;
				MainMenu();

				mScore = 0;
				mLives = 5;
				mOrbitingShips.Clear();
				ClearGameObjects();
				if (Application.platform != RuntimePlatform.Android)
				{
					PlaySound(4);
				}
				else
				{
					mSoundManager.PlaySound("Feedback_Negative");
				}
			}
		}

		if (mGameState == GameState.Gameplay)
		{
			if (mOneUpCounter >= 5)
			{
				mLives++;
				mOneUpCounter = 0;
			}

			if (mGameOver)// && (!mFreeVersion || mGameOverTimer > 0.0f))
			{
				UpdateGameOver();
			}

			if (!mPaused && !mTutorialMode && mLives < 5 && !mGameOver)
			{
				mExtraLifeTimer -= Time.deltaTime;
				if (mExtraLifeTimer <= 0.0f)
				{
					Transform life = Instantiate(mExtraLifePrefab, mExtraLifeSpawnPoint, Quaternion.identity);
					life.parent = mGamePlayObjectParent;
					life.GetComponent<Script_ExtraLife>().mGameManager = this;
					mExtraLifeTimer = 8.0f * mLives + Random.Range(3.0f, 8.0f);

					if (mHintManager.mDisplayHints && mHintManager.mFlagExtraLife)
					{
						mHintManager.mFlagExtraLife = false;
						mHintManager.AddHint("An extra life just spawned. Touch it with any ship to increase your lives by one.");
					}
				}
			}

			if (!mButtonShipThruster.gameObject.activeInHierarchy)
			{
				mControlFireBooster = false;
			}
			if (!mButtonShipRotateCW.gameObject.activeInHierarchy)
			{
				mControlRotateClockwise = false;
			}
			if (!mButtonShipRotateCCW.gameObject.activeInHierarchy)
			{
				mControlRotateCounterClockwise = false;
			}
		}

		mGUILives.GetComponent<Text>().text = "Lives: " + mLives;
		mGUIScore.GetComponent<Text>().text = "Score: " + mScore;
		//mGUIScore.GetComponent<Text>().text = Script_Facebook.debug;
		mGUIScrap.GetComponent<Text>().text = "Scrap: " + mOneUpCounter + "/5";
		//mGUIScrap.GetComponent<Text>().text = "";
	}

	void OnApplicationFocus(bool pauseStatus)
	{
		if (!mStartupFlag)
		{
			mStartupFlag = true;
			return;
		}

		if (pauseStatus)
		{
			//your app is NO LONGER in the background
			if (mGameState == GameState.Gameplay && !mGameOver && !mPaused)
			{
				mPaused = true;
				if (Application.platform != RuntimePlatform.Android)
				{
					PlaySound(4);
				}
				else
				{
					mSoundManager.PlaySound("Feedback_Negative");
				}
				mPauseMenu.gameObject.SetActive(true);
				mGameplayMenu.gameObject.SetActive(false);
			}
		}
		else
		{
			//your app is now in the background
		}
	}

	private System.DateTime GetSavedPlaysLeftTimeStamp()
	{
		System.DateTime defaultTime = System.DateTime.Now;
		int year = PlayerPrefs.GetInt("PlaysLeft_Year", defaultTime.Year);
		int month = PlayerPrefs.GetInt("PlaysLeft_Month", defaultTime.Month);
		int day = PlayerPrefs.GetInt("PlaysLeft_Day", defaultTime.Day);
		int hour = PlayerPrefs.GetInt("PlaysLeft_Hour", defaultTime.Hour);
		int minute = PlayerPrefs.GetInt("PlaysLeft_Minute", defaultTime.Minute);
		int second = PlayerPrefs.GetInt("PlaysLeft_Second", defaultTime.Second);
		return new System.DateTime(year, month, day, hour, minute, second);
	}

	private void SetSavedPlaysLeftTimeStamp(System.DateTime time)
	{
		PlayerPrefs.SetInt("PlaysLeft_Year", time.Year);
		PlayerPrefs.SetInt("PlaysLeft_Month", time.Month);
		PlayerPrefs.SetInt("PlaysLeft_Day", time.Day);
		PlayerPrefs.SetInt("PlaysLeft_Hour", time.Hour);
		PlayerPrefs.SetInt("PlaysLeft_Minute", time.Minute);
		PlayerPrefs.SetInt("PlaysLeft_Second", time.Second);
	}

	private void UpdateScore()
	{

	}

	private void UpdateHighScore()
	{
		// Compare old high scores to new one
		HighScore newScore;
		newScore.mName = mName;
		newScore.mScore = mScore;
		for (int i = 0; i < 10; ++i)
		{
			HighScore oldScore = mHighScores[i];
			if (newScore.mScore != 0 && mScore > oldScore.mScore)
			{
				// Switcheroo
				mHighScores[i] = newScore;
				newScore = oldScore;
			}

			PlayerPrefs.SetString("high_score_name_" + i, mHighScores[i].mName);
			PlayerPrefs.SetInt("high_score_score_" + i, mHighScores[i].mScore);
		}

		// Update GUI to display the high scores
		mGUIHighScore.GetComponent<Text>().text = "Ships in Orbit:\n" + mShipsInOrbitRecord + "\n\n";
		for (int i = 0; i < 10; ++i)
		{
			mGUIHighScore.GetComponent<Text>().text += mHighScores[i].mName + " : " + mHighScores[i].mScore + "\n"; ;
		}
	}

	private void LoadHighScores()
	{
		// Read in high scores
		for (int i = 0; i < 10; ++i)
		{
			HighScore highScore = new HighScore();
			highScore.mName = PlayerPrefs.GetString("high_score_name_" + i, "--");
			highScore.mScore = PlayerPrefs.GetInt("high_score_score_" + i, 0);
			mHighScores[i] = highScore;
		}

		// Update GUI to display the high scores
		mGUIHighScore.GetComponent<Text>().text = "Ships in Orbit:\n" + mShipsInOrbitRecord + "\n\n";
		for (int i = 0; i < 10; ++i)
		{
			mGUIHighScore.GetComponent<Text>().text += mHighScores[i].mName + " : " + mHighScores[i].mScore + "\n"; ;
		}
	}

	private void SetTutorialTextBox(string _message, float _textPosX, float _textPosY, float _textWidth, float _textHeight, float _framePosX, float _framePosY, float _frameWidth, float _frameHeight)
	{
		mTutorialText.GetComponent<Text>().text = _message;
		mTutorialText.GetComponent<RectTransform>().localPosition = new Vector3(_textPosX, _textPosY, 0.0f);
		mTutorialText.GetComponent<RectTransform>().sizeDelta = new Vector2(_textWidth, _textHeight);
		mTutorialTextFrame.GetComponent<RectTransform>().position = new Vector3(_framePosX, _framePosY, 0.0f);
		mTutorialTextFrame.GetComponent<RectTransform>().sizeDelta = new Vector2(_frameWidth, _frameHeight);
	}

	private void SetTutorialButtonPosition(float _x, float _y)
	{
		mTutorialNextButton.GetComponent<RectTransform>().position = new Vector3(_x, _y, 0.0f);
	}

	private void UpdateGameOver()
	{
		if (mGameOver)// && (!mFreeVersion || mGameOverTimer > 0.0f))
		{
			//if (mFreeVersion)
			//{
			//	if (mGameOverTimer > 2.5f)
			//	{
			//		mGameOverUI.GetChild(0).GetComponent<Text>().text = "Game Over";
			//	}
			//	else
			//	{
			//		mGameOverUI.GetChild(0).GetComponent<Text>().text = "Incoming Transmission...";
			//		Color newColor = mGameOverUI.GetChild(0).GetComponent<Text>().color;
			//		newColor.a += mGameOverOpacityRate * Time.deltaTime;
			//		mGameOverUI.GetChild(0).GetComponent<Text>().color = newColor;

			//		if ((mGameOverOpacityRate < 0.0f && newColor.a < 0.01f) ||
			//			(mGameOverOpacityRate > 0.0f && newColor.a > 0.99f))
			//		{
			//			mGameOverOpacityRate *= -1.0f;
			//		}
			//	}
			//}

			mGameOverTimer -= Time.deltaTime;
			if (mGameOverTimer <= 0.0f)
			{
				//Color newColor = mGameOverUI.GetChild(0).GetComponent<Text>().color;
				//newColor.a = 1.0f;
				//mGameOverUI.GetChild(0).GetComponent<Text>().color = newColor;
				//mGameOverUI.GetChild(0).GetComponent<Text>().text = "Game Over";

				mGameOverUI.GetChild(1).gameObject.SetActive(true); // turn button back on
				mGameOverUI.GetChild(2).gameObject.SetActive(true); // turn name text back on
				mGameOverUI.GetChild(3).gameObject.SetActive(true); // turn name input field back on
			}
		}
	}

	public void ShipDestroyed(Transform ship)
	{
		mOrbitingShips.Remove(ship);
		mNumberOfOrbitingShips--;

		if (mTutorialMode)
		{
			mTutorialShipCrashed = true;
		}

		else if (mLives > 0)
		{
			mLives--;
		}

		if (mLives <= 0 && !mGameOver)
		{
			mGameOver = true;
			mGameOverUI.gameObject.SetActive(true);
			//mGameOverUI.GetChild(0).GetComponent<Text>().text = "Out of lives, finish your lap.";
			mGameOverUI.GetChild(0).GetComponent<Text>().text = "Game Over";
			mGameOverUI.GetChild(1).gameObject.SetActive(false); // turn off the play again button for now
			mGameOverUI.GetChild(2).gameObject.SetActive(false); // turn off the name text for now
			mGameOverUI.GetChild(3).gameObject.SetActive(false); // turn off the name input for now
		}
	}

	public void CreateShip()
	{
		if (mLives > 0)
		{
			Transform ship = Instantiate(mSpaceShipPrefab, mShipSpawnPoint, Quaternion.identity);
			ship.parent = mGamePlayObjectParent;
			Script_SpaceShip shipScript = ship.GetComponent<Script_SpaceShip>();
			shipScript.mGameManager = this;
			mOrbitingShips.Add(ship);
			if (mTutorialMode)
			{
				mControllingShip = ship;
			}
			mNumberOfOrbitingShips++;
		}
	}

	private void UpdateMenus()
	{
		mMainMenu.gameObject.SetActive(mGameState == GameState.Menu);
		mOptionsMenu.gameObject.SetActive(mGameState == GameState.Options);
		mGameplayMenu.gameObject.SetActive(mGameState == GameState.Gameplay);
		mHighScoreMenu.gameObject.SetActive(mGameState == GameState.HighScore);
	}

	private void ClearGameObjects()
	{
		for (int i = 0; i < mGamePlayObjectParent.childCount; ++i)
		{
			Destroy(mGamePlayObjectParent.GetChild(i).gameObject);
		}
	}

	private void PlaySound(int _index)
	{
		Transform sound = Instantiate(mSoundEffectPrefab);
		sound.GetComponent<Script_SoundEffect>().PlaySound(_index);
	}

	public void TutorialNext()
	{
		mTutorialFlag = true;
		mTutorialStep++;
	}

	public void PlayAgain()
	{
		UpdateHighScore();

		mGameOverUI.gameObject.SetActive(false);
		mLives = 5;
		mScore = 0;
		mOneUpCounter = 0;
		mGameOver = false;

		ClearGameObjects();
		mOrbitingShips.Clear();

		CreateShip();
		
		if (Application.platform != RuntimePlatform.Android)
		{
			PlaySound(5);
		}
		else
		{
			mSoundManager.PlaySound("Feedback_Positive");
		}
	}

	public void PlayGame()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			PlaySound(5);
		}
		else
		{
			mSoundManager.PlaySound("Feedback_Positive");
		}

		mGameState = GameState.Gameplay;
		UpdateMenus();

		mLives = 5;
		mScore = 0;
		mOneUpCounter = 0;
		mNumberOfOrbitingShips = 0;
		mGameOver = false;
		mGameOverUI.gameObject.SetActive(false);
		mPaused = false;

		CreateShip();

		mHintManager.ResetHints();
	}

	public void PlayTutorial()
	{
		mTutorialMode = true;
		mTutorialFlag = true;
		mTutorialShipCrashed = false;
		mTutorialStep = 0;
		mTutorialCrashedShipCount = 0;
		mTutorialUI.gameObject.SetActive(true);
		PlayGame();
	}

	public void MainMenu()
	{
		if (mScore != 0)
		{
			UpdateHighScore();
		}

		mPauseMenu.gameObject.SetActive(false);
		ClearGameObjects();

		mGameState = GameState.Menu;
		UpdateMenus();
		mPaused = false;
		
		if (mTutorialMode)
		{
			mTutorialMode = false;
			mButtonShipThruster.gameObject.SetActive(true);
			mButtonShipRotateCW.gameObject.SetActive(true);
			mButtonShipRotateCCW.gameObject.SetActive(true);
			mHUD.gameObject.SetActive(true);
		}
		
		//if (Application.platform != RuntimePlatform.Android)
		//{
		//	PlaySound(4);
		//}
		//else
		//{
		//	mSoundManager.PlaySound("Feedback_Negative");
		//}
	}

	public void PlayNegativeSound()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			PlaySound(4);
		}
		else
		{
			mSoundManager.PlaySound("Feedback_Negative");
		}
	}

	public void OptionsMenu()
	{
		mPauseMenu.gameObject.SetActive(false);
		mGameState = GameState.Options;
		UpdateMenus();
		
		if (Application.platform != RuntimePlatform.Android)
		{
			PlaySound(5);
		}
		else
		{
			mSoundManager.PlaySound("Feedback_Positive");
		}
	}

	public void ShopMenu()
	{
		mGameState = GameState.Shop;
		UpdateMenus();
		
		if (Application.platform != RuntimePlatform.Android)
		{
			PlaySound(5);
		}
		else
		{
			mSoundManager.PlaySound("Feedback_Positive");
		}
	}

	public void HighScoreMenu()
	{
		mGameState = GameState.HighScore;
		mEarthReference.gameObject.SetActive(false);
		UpdateMenus();
		
		if (Application.platform != RuntimePlatform.Android)
		{
			PlaySound(5);
		}
		else
		{
			mSoundManager.PlaySound("Feedback_Positive");
		}
	}

	public void ExitGame()
	{
		//if (Application.platform != RuntimePlatform.Android)
		//{
		//	PlaySound(4);
		//}
		//else
		//{
		//	mSoundManager.PlaySound("Feedback_Negative");
		//}
		Application.Quit();
	}

	public void Resume()
	{
		mPaused = false;
		
		if (Application.platform != RuntimePlatform.Android)
		{
			PlaySound(5);
		}
		else
		{
			mSoundManager.PlaySound("Feedback_Positive");
		}

		mPauseMenu.gameObject.SetActive(false);
		mGameplayMenu.gameObject.SetActive(true);
	}

	public void AdjustMusicVolume(float newVolume)
	{
		mMusicVolume = newVolume;
		PlayerPrefs.SetFloat("MusicVolume", newVolume);
		GetComponent<AudioSource>().volume = newVolume;
	}

	public void AdjustSoundVolume(float newVolume)
	{
		mSoundVolume = newVolume;
		PlayerPrefs.SetFloat("SoundVolume", newVolume);
		if (Application.platform == RuntimePlatform.Android)
		{
			mSoundManager.SetVolume(newVolume);
		}
	}

	public void UpdateName(string _name)
	{
		mName = _name;
		PlayerPrefs.SetString("name", mName);
	}

    public void PressBackButton()
    {
		if (mGameState == GameState.HighScore)
		{
			mEarthReference.gameObject.SetActive(true);
		}

        mBackButton = true;
    }

	public void BuyPremium()
	{
		Application.OpenURL("https://play.google.com/store/apps/details?id=com.MichaelSciortino.OrbitaLuiPremium");
	}

	#region UI Controls

	public void ControlFireBoosterTrue()
	{
		mControlFireBooster = true;
	}

	public void ControlFireBoosterFalse()
	{
		mControlFireBooster = false;
	}

	public void ControlRotateClockwiseTrue()
	{
		mControlRotateClockwise = true;
	}

	public void ControlRotateClockwiseFalse()
	{
		mControlRotateClockwise = false;
	}

	public void ControlRotateCounterClockwiseTrue()
	{
		mControlRotateCounterClockwise = true;
	}

	public void ControlRotateCounterClockwiseFalse()
	{
		mControlRotateCounterClockwise = false;
	}

	#endregion

}
