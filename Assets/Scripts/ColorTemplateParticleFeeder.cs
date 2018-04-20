using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MadMaps.Common;

public class ColorTemplateParticleFeeder : MonoBehaviour 
{
	public ParticleSystem System;
	public float Offset = 0;
	[Range(0, 2)]
	public int Index = 0;
	[Range(0, 1)]
	public float Alpha = .2f;
	public ColorTemplateManager Manager;
	public Color LastColor;

	private void Update()
	{
		LastColor = Manager.GetTemplateAtTime(Offset).GetByIndex(Index).WithAlpha(Alpha);
		var main = System.main;
		main.startColor = LastColor;
		//System.main = main;
	}
}
