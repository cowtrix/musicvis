using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystem_SetStartingFrameWrapper : MonoBehaviour 
{
	public ParticleSystem System;
    public int Frame = 0;
	public int TotalFrames = 100;

	private void Update()
	{
		var tsa = System.textureSheetAnimation;
        tsa.startFrame = Frame / (float)TotalFrames;
	}
}
