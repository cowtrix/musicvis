using System.Collections;
using System.Collections.Generic;
using MadMaps.Common;
using UnityEngine;

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