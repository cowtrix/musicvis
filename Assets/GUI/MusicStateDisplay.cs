using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class MusicStateDisplay : MonoBehaviour 
{
	public Slider RMS, Peak;
	public UILineRenderer LineRenderer;
	const int STEP = 8;
	SmartValue _rms;
	SmartValue _peak;
	float _wMin = float.MaxValue;
	float _wMax = float.MinValue;

	void Awake()
	{
		_rms = new SmartValue(1000);
		_peak = new SmartValue(1000);
		LineRenderer.drivenExternally = true;
	}

	void Update()
	{
		var ms = MusicManager.MusicState;
		_rms.AddValue(ms.RMS);
		_peak.AddValue(ms.Peak);
		RMS.value = _rms.GetValue();
		Peak.value = _peak.GetValue();

		var p = LineRenderer.Points;
		var pLength = ms.Wavelength.Length / STEP;
		if(p == null || p.Length != pLength)
		{
			p = new Vector2[pLength];
		}
		int pCounter = 0;
		for(var i = 0; i < ms.Wavelength.Length; i += STEP)
		{
			if(pCounter >= p.Length)
			{
				break;
			}
			var w = ms.Wavelength[i];
			if(w < _wMin)
			{
				_wMin = w;
			}
			if(w > _wMax)
			{
				_wMax = w;
			}
			var x = i / (float)ms.Wavelength.Length;
			var y = (w - _wMin) / (_wMax - _wMin);
			p[pCounter] = new Vector2(x, y);
			pCounter++;
		}
		LineRenderer.Points = p;
		LineRenderer.SetAllDirty();
	}
}
