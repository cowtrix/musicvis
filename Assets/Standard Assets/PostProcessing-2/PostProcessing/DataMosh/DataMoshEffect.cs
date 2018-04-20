using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace UnityEngine.Rendering.PostProcessing
{
	[Serializable]
	[PostProcess(typeof(DataMoshRenderer), "Data Mosh", false)]
	public sealed class DataMosh : PostProcessEffectSettings
	{
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return enabled.value
				&& SystemInfo.graphicsShaderLevel >= 35;
		}
	}

	public class DataMoshRenderer : PostProcessEffectRenderer<DataMosh> 
	{
		public override DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.MotionVectors;
		}

		public override void Render(PostProcessRenderContext context)
		{

		}
	}
}