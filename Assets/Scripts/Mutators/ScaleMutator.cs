using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleMutator : Mutator 
{
	public Vector2 Scale = new Vector2(1, 2);
	public float Multiplier = 1;
	
	protected override void TickInternal(float strength)
	{
		transform.localScale = (Vector3.one * Scale.x + Vector3.one * strength * (Scale.y - Scale.x)) * Multiplier;
	}
}
