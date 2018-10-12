using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTransition : Transition {

	public Vector2 Scale = new Vector2(0, 1);

	public override void Tick(float strength)
	{
		transform.localScale = Mathf.Lerp(Scale.x, Scale.y, strength) * Vector3.one;
	}
}
