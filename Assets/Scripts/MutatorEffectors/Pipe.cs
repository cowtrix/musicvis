using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MadMaps.Common.GenericEditor;
using System;

[Name("Linear/Pipe")]
[Serializable]
public class PipeMutatorEffect : IMutatorEffector 
{
	public AnimationCurve Distortion = AnimationCurve.Linear(0, 1, 1, 1);
	public List<Mutator> Mutators = new List<Mutator>();

	float _lastStrength;

	public void Tick(float strength, float weight)
	{
		_lastStrength = strength * Distortion.Evaluate(strength);
		foreach (var mutator in Mutators)
		{
			if(mutator == null)
			{
				continue;
			}
			mutator.Tick(_lastStrength, weight);
		}
	}

	public override string ToString()
	{
		return "Pipe " + _lastStrength;
	}
}

[Name("Event/Max Threshold")]
[Serializable]
public class MaxThresholdMutatorEffect : IMutatorEffector 
{
	public float TriggerValue = .5f;
	public float EventDuration = 1;
	public AnimationCurve Distortion;
	public AnimationCurve ValueOverTime;
	public List<Mutator> Mutators = new List<Mutator>();

	float _playingTimer = 0;
	float _lastVal;

	public void Tick(float strength)
	{
		strength = strength * Distortion.Evaluate(strength);
		if(_playingTimer < 0)
		{
			if(strength > TriggerValue)
			{
				_playingTimer = EventDuration;
			}
			else
			{
				return;
			}			
		}
		_playingTimer -= Time.deltaTime;
		var normalisedT = 1 - Mathf.Clamp01(_playingTimer / EventDuration);
		_lastVal = ValueOverTime.Evaluate(normalisedT);
		foreach (var mutator in Mutators)
		{
			if(mutator == null)
			{
				continue;
			}
			mutator.Tick(_lastVal, weight);
		}
	}

	public override string ToString()
	{
		if(_playingTimer < 0)
		{
			return "Event Unactivated";
		}
		return string.Format("Activated: {0}s = {1}", _playingTimer, _lastVal);
	}
}