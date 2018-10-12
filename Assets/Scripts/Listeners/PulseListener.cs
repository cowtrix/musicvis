using System.Collections;
using System.Collections.Generic;
using MadMaps.Common;
using UnityEngine;
using MidiJack;

public class PulseListener : IListener 
{
	public float Strength { get; private set; }

	public float BPM;
	public AnimationCurve PulseShape = AnimationCurve.Linear(0, 1, 1, 0);
		
	float _timer;

	public void Listen()
	{
		Strength = 0;
		_timer += Time.deltaTime;

		var frac = Mathf.Clamp01(Mathfx.Frac((_timer / 60) * BPM));
		Strength = PulseShape.Evaluate(frac);
	}      
}