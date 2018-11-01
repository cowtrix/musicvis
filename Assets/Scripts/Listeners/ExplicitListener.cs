using System.Collections;
using System.Collections.Generic;
using MadMaps.Common;
using UnityEngine;
using MidiJack;

// Delivers an explicit value
// Use this for things like Midi knob inputs
public class ExplicitListener : IListener 
{
	public float Value;
	public float Min = 0;
	public float Max = 1;
	

	public void Listen(SmartValue value)
	{
		value.AddValue(Value);
	}      
}

