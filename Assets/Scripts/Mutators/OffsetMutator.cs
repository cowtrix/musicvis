using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetMutator : MonoBehaviour {
	public Vector3 Vector;
	public bool Local;
	public bool Aligned;
	Vector3 _initialOffset;

	void Awake()
	{
		_initialOffset = Local ? transform.localPosition : transform.position;
		//Debug.Log("_initialOffset is " + _initialOffset, this);
	}

	public void Tick(float val)
	{
		var vec = _initialOffset + Vector * val;
		if(Aligned)
		{
			transform.localToWorldMatrix.MultiplyVector(vec);
		}
		if(Local)
		{
			transform.position = vec;
		}
		else
		{
			transform.localPosition = vec;
		}
	}
}
