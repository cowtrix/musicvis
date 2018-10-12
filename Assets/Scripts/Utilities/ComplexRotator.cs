﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexRotator : MonoBehaviour 
{	
	public float TimeForRotations = 1;
	public AnimationCurve SpeedOverTime;
	public Vector3 StartRotation, EndRotation;

	float _timer;
	float _rotationTimer;
	float _currentRotationStrength = 1;

	private void Update()
	{
		_timer += .1f;
		var t = Mathfx.Frac(_timer / TimeForRotations);
		var target = Vector3.Lerp(StartRotation, EndRotation, t);

		transform.localRotation = Quaternion.Euler(target);
	}
}