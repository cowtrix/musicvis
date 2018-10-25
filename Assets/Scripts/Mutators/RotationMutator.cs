﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationMutator : Mutator 
{
	public Vector3 Axis;
	public bool RandomiseOnStart;
	public float Wander;


	[ContextMenu("Randomise")]
	void Randomise()
	{
		Axis = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Axis.magnitude;
	}

	private void Start()
	{
		if(RandomiseOnStart)
		{
			Randomise();
		}
	}
	
	protected override void TickInternal(float strength)
	{
		transform.localRotation *= Quaternion.Euler(strength * Axis);
		if(Wander != 0)
		{
			Axis = (Axis + (Random.onUnitSphere * Wander)).normalized * Axis.magnitude;
		}
	}
}
