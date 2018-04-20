using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationMutator : Mutator 
{
	public Vector3 Axis;
	
	public override void Tick(float strength)
	{
		transform.localRotation *= Quaternion.Euler(Axis);
	}
}
