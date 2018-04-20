using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatRendererShaderVarMutator : Mutator 
{
	public Renderer Renderer;
	public float Multiplier = 1;
	public string Name;
	private static MaterialPropertyBlock _block;

	public override void Tick(float strength)
	{
		if(_block == null)
		{
			_block = new MaterialPropertyBlock();
		}
		Renderer.GetPropertyBlock(_block);
		_block.SetFloat(Name, strength * Multiplier);
		Renderer.SetPropertyBlock(_block); 	
	}
}
