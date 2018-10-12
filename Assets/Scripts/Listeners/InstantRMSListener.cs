using System.Collections;
using System.Collections.Generic;
using MadMaps.Common;
using UnityEngine;
using MidiJack;

public class InstantRMSListener : IListener 
{
	public float Strength { get { return Value.Value; } }
	public SmartValue Value = new SmartValue(1);

	public void Listen()
	{
		Value.AddValue(MusicManager.MusicState.RMS);
	}      
}