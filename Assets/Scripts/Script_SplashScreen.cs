using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_SplashScreen : MonoBehaviour
{
	public float mFadeRate;
	public Image mImage;

	// Use this for initialization
	void Start()
	{
		mImage = GetComponent<Image>();

		Color color = mImage.color;
		color.a = 1.0f;
		mImage.color = color;
	}

	// Update is called once per frame
	void Update()
	{
		Color color = mImage.color;
		color.a -= mFadeRate * Time.deltaTime;
		mImage.color = color;

		if (color.a <= 0.0f)
		{
			gameObject.SetActive(false);
		}
	}

	public void SkipSplashScreen()
	{
		Color color = mImage.color;
		color.a -= 0.0f;
		mImage.color = color;
	}
}
