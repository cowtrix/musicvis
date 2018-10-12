using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MadMaps.Common.GenericEditor;
using System;
using System.Reflection;

[AddComponentMenu("Mutator Effectors/Pipe")]
public class PipeMutatorEffector : MutatorEffector
{
	public bool Distort;
	public AnimationCurve Distortion = AnimationCurve.Linear(0, 1, 1, 1);

	public bool Remap;
	public AnimationCurve Mapping = AnimationCurve.Linear(0, 1, 1, 1);

	protected override void Tick(float strength)
	{
		if(Distort)
		{
			strength *= Distortion.Evaluate(strength);
		}
		if(Remap)
		{
			strength = Mapping.Evaluate(strength);
		}
		Value = strength;
	}
}