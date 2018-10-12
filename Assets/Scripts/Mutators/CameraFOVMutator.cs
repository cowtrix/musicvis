using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFOVMutator : Mutator 
{
	public Camera Cam;
	public Vector2 FoV = new Vector2(60, 60);
	public float Multiplier = 1;
	
	protected override void TickInternal(float strength)
	{
		Cam.fieldOfView = (FoV.x + strength * (FoV.y - FoV.x)) * Multiplier;
	}
}
