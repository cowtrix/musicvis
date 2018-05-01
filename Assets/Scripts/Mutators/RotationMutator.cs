using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationMutator : Mutator 
{
	public Vector3 Axis;
	
	protected override void TickInternal(float strength)
	{
		transform.localRotation *= Quaternion.Euler(strength * Axis);
	}
}
