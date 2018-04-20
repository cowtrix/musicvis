using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MadMaps.Common;

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

	private void Update()
	{
		if(_block == null)
		{
			_block = new MaterialPropertyBlock();
		}
		Renderer.GetPropertyBlock(_block);
		LastColor = Manager.GetTemplateAtTime(Offset).GetByIndex(Index).WithAlpha(Alpha);
		_block.SetColor(Name, LastColor);
		Renderer.SetPropertyBlock(_block); 
	}
}
