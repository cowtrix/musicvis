using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEmit : MonoBehaviour {
	ParticleSystem _system;
	public Vector2 EmissionRange;

	void Awake()
	{
		_system = GetComponent<ParticleSystem>();
	}

	public void Tick(float f)
	{
		var e = _system.emission;
		e.rate = Mathf.Lerp(EmissionRange.x, EmissionRange.y, f);
	}
}
