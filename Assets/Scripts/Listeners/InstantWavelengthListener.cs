using System.Collections;
using System.Collections.Generic;
using MadMaps.Common;
using UnityEngine;
using MidiJack;

public class InstantWavelengthListener : IListener 
{
	public AnimationCurve Levels;

	public void Listen(SmartValue value)
	{
		value.AddValue(Levels.GetSumUnderWavelength(MusicManager.MusicState.Wavelength));
	}    
}