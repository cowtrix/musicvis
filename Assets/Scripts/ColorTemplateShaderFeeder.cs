using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MadMaps.Common;
using System;

[Serializable]
public class ColorMutator
{
	public float Hue, Sat, Val;

	public Color Mutate(Color c)
	{
		float h, s, v;
		Color.RGBToHSV(c, out h, out s, out v);
		h = Mathfx.Frac(h + Hue);
		s = Mathf.Clamp01(s + Sat);
		v = Mathf.Clamp01(v + Val);
		return Color.HSVToRGB(h, s, v).WithAlpha(c.a);
	}
}

public class ColorTemplateShaderFeeder : MonoBehaviour 
{
	public string Name = "_Color";
	public Renderer Renderer;
	public float Offset = 0;
	[Range(0, 2)]
	public int Index = 0;
	[Range(0, 1)]
	public float Alpha = .2f;
	public ColorTemplateManager Manager;
	public Color LastColor;
	private static MaterialPropertyBlock _block;

	public ColorMutator Mutator;
	
	private void Update()
	{
		if(_block == null)
		{
			_block = new MaterialPropertyBlock();
		}
		Renderer.GetPropertyBlock(_block);
		LastColor = Mutator.Mutate(Manager.GetTemplateAtTime(Offset).GetByIndex(Index).WithAlpha(Alpha));
		
		_block.SetColor(Name, LastColor);
		Renderer.SetPropertyBlock(_block); 
	}
}
