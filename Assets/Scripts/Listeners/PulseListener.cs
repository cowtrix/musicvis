using System.Collections;
using System.Collections.Generic;
using MadMaps.Common;
using UnityEngine;
using MidiJack;

public class PulseListener : IListener 
{
	public float BPM;
	public AnimationCurve PulseShape = AnimationCurve.Linear(0, 1, 1, 0);		
	float _timer;

	public void Listen(SmartValue value)
	{
		var strength = 0f;
		_timer += Time.deltaTime;

		var frac = Mathf.Clamp01(Mathfx.Frac((_timer / 60) * BPM));
		strength = PulseShape.Evaluate(frac);

		value.AddValue(strength);
	}      
}