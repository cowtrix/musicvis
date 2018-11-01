using System.Collections;
using System.Collections.Generic;
using MadMaps.Common;
using UnityEngine;
using MidiJack;

public class InstantPeakListener : IListener 
{
	public void Listen(SmartValue value)
	{
		value.AddValue(MusicManager.MusicState.Peak);
	}      
}