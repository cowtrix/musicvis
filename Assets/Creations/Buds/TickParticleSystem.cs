using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickParticleSystem : MonoBehaviour {

	public float Multiplier = 1;
	public ParticleSystem System;

	void Awake()
	{
		System.Play();
	}	

	public void Tick(float val)
	{
		System.Simulate(val * Multiplier, true, false, false);
	}
}
