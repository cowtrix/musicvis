using System.Collections;
using System.Collections.Generic;
using MadMaps.Common;
using UnityEngine;
using MidiJack;

#if UNITY_EDITOR
using UnityEditor;
using MadMaps.Common.GenericEditor;
using System.Reflection;
using System;

public class ExplicitListenerDrawer : GenericDrawer<ExplicitListener>
{
	protected override ExplicitListener DrawGUIInternal(ExplicitListener target, string label = "", Type targetType = null, FieldInfo fieldInfo = null,
            object context = null)
	{
		target.Value = EditorGUILayout.Slider(target.Value, target.Min, target.Max);
		var minMax = new Vector2(target.Min, target.Max);
		minMax = EditorGUILayout.Vector2Field(GUIContent.none, minMax);
		target.Min = minMax.x;
		target.Max = minMax.y;

		target.UseMidi = EditorGUILayout.Toggle("Midi", target.UseMidi);
		if(target.UseMidi)
		{
			target.MidiChannel = (MidiChannel)EditorGUILayout.EnumPopup("Channel", target.MidiChannel);
			target.MidiIndex = EditorGUILayout.IntField("Index", target.MidiIndex);
		}

		return target;
	}
}
#endif

public class InstantWavelengthListener : IListener 
{
	public AnimationCurve Levels;
	public float Strength { get { return Value.Value; } }
	public SmartValue Value = new SmartValue(1);

	public void Listen(MusicState state)
	{
		Value.AddValue(Levels.GetSumUnderWavelength(state.Wavelength));
	}    
}

public class InstantRMSListener : IListener 
{
	public float Strength { get { return Value.Value; } }
	public SmartValue Value = new SmartValue(1);

	public void Listen(MusicState state)
	{
		Value.AddValue(state.RMS);
	}      
}

public class InstantPeakListener : IListener 
{
	public float Strength { get { return Value.Value; } }
	public SmartValue Value = new SmartValue(1);

	public void Listen(MusicState state)
	{
		Value.AddValue(state.Peak);
	}      
}

public class ExplicitListener : IListener 
{
	public float Strength { get { return Value; } }

	public float Value;
	public float Min = 0;
	public float Max = 1;
	public bool UseMidi;
	public MidiChannel MidiChannel;
	public int MidiIndex;

	public void Listen(MusicState state)
	{
		#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
		if(UseMidi)
		{
			Value = MidiMaster.GetKnob(MidiChannel, MidiIndex);
		}
		#endif
	}      
}

public class PulseListener : IListener 
{
	public float Strength { get; private set; }

	public float BPM;
	public float Min = 0;
	public float Max = 180;

	public bool UseMidi;
	public MidiChannel MidiChannel;
	public int MidiIndex;

	float _timer;

	public void Listen(MusicState state)
	{
		#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
		if(UseMidi)
		{
			BPM = Min + MidiMaster.GetKnob(MidiChannel, MidiIndex) * Max;
		}
		#endif
		Strength = 0;
		_timer += Time.deltaTime;
		var oneOverBPM = 1f / BPM;
		while (_timer > oneOverBPM)
		{
			_timer -= oneOverBPM;
			Strength = 1;
		}
	}      
}