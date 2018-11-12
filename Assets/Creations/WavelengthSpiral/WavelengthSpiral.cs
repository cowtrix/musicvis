using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavelengthSpiral : MonoBehaviour {
	LineRenderer _lineRenderer;
	Vector3[] _points;
	public float Radius = 100;
	public float Spin = 4;
	public float Amplitude = 1;

	public Vector2 Range = new Vector2(0, 1);

	void Awake()
	{
		_lineRenderer = GetComponent<LineRenderer>();
	}

	public void SetAmplitude(float val)
	{
		Amplitude = val;
	}

	public void SetLowRange(float val)
	{
		Range.x = val;
	}

	public void SetHighRange(float val)
	{
		Range.y = val;
	}

	void Update () 
	{
		var ms = MusicManager.MusicState;
		if(_points == null || _points.Length != ms.Wavelength.Length)
		{
			_points = new Vector3[ms.Wavelength.Length];
		}
		_lineRenderer.positionCount = ms.Wavelength.Length;
		
		for(var i = 0; i < ms.Wavelength.Length; ++i)
		{
			var index = i;
			var f = index / (float)ms.Wavelength.Length;
			if(f < Range.x)
			{
				index = (int)(Range.x * ms.Wavelength.Length);
			}
			else if(f > Range.y)
			{
				index = (int)(Range.y * ms.Wavelength.Length);
			}
			f = index / (float)ms.Wavelength.Length;			
			_points[i] = transform.localToWorldMatrix.MultiplyPoint3x4(Quaternion.Euler(0, f * Spin * 360, 0) * (Vector3.right * f * Radius) + Vector3.up * ms.Wavelength[index] * Amplitude);
		}
		_lineRenderer.SetPositions(_points);
	}
}
