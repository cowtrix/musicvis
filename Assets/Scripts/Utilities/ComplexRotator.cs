using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexRotator : MonoBehaviour 
{	
	public float TimeForRotations = 1;
	public Vector3 StartRotation, EndRotation;

	float _timer;
	float _rotationTimer;
	float _currentRotationStrength = 1;

	public void Tick(float val)
	{
		_timer += val;
		var t = Mathf.Sin(_timer * TimeForRotations);
		var target = Vector3.Lerp(StartRotation, EndRotation, t);

		transform.localRotation = Quaternion.Euler(target);
	}
}
