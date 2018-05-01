using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MadMaps.Common.GenericEditor;
using System;
using System.Reflection;

public abstract class BaseMutatorEffect : IMutatorEffector 
{
	public class BaseReflectionEntry
	{
		public string FieldName;
		[NonSerialized]
		public FieldInfo FieldInfo;

		public BaseReflectionEntry Child;
	}

	public class ReflectionEntries : BaseReflectionEntry
	{		
		public Component Component;		
	}

	public List<ReflectionEntries> Reflections = new List<ReflectionEntries>();

	public abstract void Tick(float strength);

	protected void ApplyReflection(float val, bool roundToInt = false)
	{
		for(var i = Reflections.Count - 1; i >= 0; i--)
		{	
			
			var reflectionEntry = Reflections[i];
			var target = reflectionEntry.Component;
			
			ApplyReflectionRecursive(val, reflectionEntry, target,roundToInt);
			
		}
	}

	private static void ApplyReflectionRecursive(float val, BaseReflectionEntry reflectionEntry, object target, bool roundToInt)
	{
		if(reflectionEntry.FieldInfo != null && reflectionEntry.FieldName != reflectionEntry.FieldInfo.Name)
		{
			reflectionEntry.FieldInfo = null;
		}
		if(reflectionEntry.FieldInfo == null && target != null)
		{
			reflectionEntry.FieldInfo = target.GetType().GetField(reflectionEntry.FieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		}
		if(reflectionEntry.FieldInfo != null && target != null) 
		{
			if(reflectionEntry.Child == null)
			{
				if(roundToInt)
				{
					reflectionEntry.FieldInfo.SetValue(target, Mathf.RoundToInt(val));
				}
				else
				{
					reflectionEntry.FieldInfo.SetValue(target, val);
				}				
			}
			else
			{
				var obj = reflectionEntry.FieldInfo.GetValue(target);
				ApplyReflectionRecursive(val, reflectionEntry.Child, obj, roundToInt);
			}
		}
		else
		{
			Debug.LogWarning(string.Format("Failed to find field {0} in {1}", reflectionEntry.FieldName, target));
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

[Name("Linear/Remapped Pipe")]
[Serializable]
public class RemappedPipeMutatorEffect : BaseMutatorEffect
{
	public AnimationCurve Mapping = AnimationCurve.Linear(0, 1, 1, 1);
	public List<Mutator> Mutators = new List<Mutator>();

	float _lastStrength;

	public override void Tick(float strength)
	{
		_lastStrength = Mapping.Evaluate(strength);
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
		return "Map Pipe " + _lastStrength;
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

[Name("Event/Max Threshold Set Random")]
[Serializable]
public class MaxThresholdRandomMutatorEffect : BaseMutatorEffect
{
	public float TriggerValue = .5f;
	public float Min = 0;
	public float Max = 0;
	public List<Mutator> Mutators = new List<Mutator>();
	public bool RoundToInt;
	private float _lastVal;

	public override void Tick(float strength)
	{
		if(strength < TriggerValue)
		{
			return;
		}
		
		_lastVal = UnityEngine.Random.Range(Min, Max);
		if(RoundToInt)
		{
			_lastVal = Mathf.RoundToInt(_lastVal);
		}
		foreach (var mutator in Mutators)
		{
			if(mutator == null)
			{
				continue;
			}
			mutator.Tick(_lastVal);
		}

		ApplyReflection(_lastVal, RoundToInt);
	}

	public override string ToString()
	{
		return string.Format("Activated: {0}", _lastVal);
	}
}