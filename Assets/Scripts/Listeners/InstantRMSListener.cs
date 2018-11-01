using System.Collections;
using System.Collections.Generic;
using MadMaps.Common;
using UnityEngine;
using MidiJack;

public class InstantRMSListener : IListener 
{
	public void Listen(SmartValue value)
	{
		value.AddValue(MusicManager.MusicState.RMS);
	}      
}