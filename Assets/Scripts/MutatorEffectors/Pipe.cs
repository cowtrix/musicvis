using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MadMaps.Common.GenericEditor;
using System;
using System.Reflection;

public abstract class BaseMutatorEffect : IMutatorEffector 
{
	public class ReflectionEntries
	{
		public Component Component;
		public string FieldName;
		public FieldInfo FieldInfo;
	}

	public List<ReflectionEntries> Reflections = new List<ReflectionEntries>();

	public abstract void Tick(float strength);

	protected void ApplyReflection(float val)
	{
		for(var i = Reflections.Count - 1; i >= 0; i--)
		{
			var reflectionEntry = Reflections[i];

			if(reflectionEntry.FieldInfo != null && reflectionEntry.FieldName != reflectionEntry.FieldInfo.Name)
			{
				reflectionEntry.FieldInfo = null;
			}

			if(reflectionEntry.FieldInfo == null && reflectionEntry.Component != null)
			{
				reflectionEntry.FieldInfo = reflectionEntry.Component.GetType().GetField(reflectionEntry.FieldName);
			}
			if(reflectionEntry.FieldInfo != null && reflectionEntry.Component != null) 
			{
				reflectionEntry.FieldInfo.SetValue(reflectionEntry.Component, val);
			}
		}
	}
}

[Name("Linear/Pipe")]
[Serializable]
public class PipeMutatorEffect : BaseMutatorEffect
{
	public AnimationCurve Distortion = AnimationCurve.Linear(0, 1, 1, 1);
	public List<Mutator> Mutators = new List<Mutator>();

	float _lastStrength;

	public override void Tick(float strength)
	{
		_lastStrength = strength * Distortion.Evaluate(strength);
		foreach (var mutator in Mutators)
		{
			if(mutator == null)
			{
				continue;
			}
			mutator.Tick(_lastStrength);
		}
		ApplyReflection(_lastStrength);
	}

	public override string ToString()
	{
		return "Pipe " + _lastStrength;
	}
}

[Name("Event/Max Threshold")]
[Serializable]
public class MaxThresholdMutatorEffect : BaseMutatorEffect
{
	public float TriggerValue = .5f;
	public float EventDuration = 1;
	public AnimationCurve Distortion;
	public AnimationCurve ValueOverTime;
	public List<Mutator> Mutators = new List<Mutator>();

	float _playingTimer = 0;
	float _lastVal;

	public override void Tick(float strength)
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
			mutator.Tick(_lastVal);
		}
		ApplyReflection(_lastVal);
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