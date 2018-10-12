using System.Collections;
using System.Collections.Generic;
using MadMaps.Common;
using UnityEngine;
using MidiJack;

// Delivers an explicit value
// Use this for things like Midi knob inputs
public class ExplicitListener : IListener 
{
	public float Strength { get { return Value; } }

	public float Value;
	public float Min = 0;
	public float Max = 1;
	public bool UseMidi;
	public MidiChannel MidiChannel;
	public int MidiIndex;

	public void Listen()
	{
		#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN    // MIDI don't work with Linux :(
		if(UseMidi)
		{
			Value = Min + MidiMaster.GetKnob(MidiChannel, MidiIndex) * (Max - Min);
		}
		#endif
	}      
}

