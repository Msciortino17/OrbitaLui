using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_ParticleExplosion : MonoBehaviour
{
	private ParticleSystem mParticleSystem;

	// Use this for initialization
	void Start()
	{
		mParticleSystem = GetComponent<ParticleSystem>();
	}

	// Update is called once per frame
	void Update()
	{
		if (!mParticleSystem.IsAlive())
		{
			Destroy(gameObject);
		}
	}
}
