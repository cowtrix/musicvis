using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleMutator : Mutator 
{
	public Vector2 Scale = new Vector2(1, 2);
	public float Multiplier = 1;

	Vector3 _initialScale;

	void Awake()
	{
		_initialScale = transform.localScale;
	}
	
	protected override void TickInternal(float strength)
	{
		transform.localScale = (_initialScale * Scale.x + _initialScale * strength * (Scale.y - Scale.x)) * Multiplier;
	}
}
