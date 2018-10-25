using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavelengthSpiral : MonoBehaviour {
	LineRenderer _lineRenderer;
	Vector3[] _points;
	public float Radius = 100;
	public float Spin = 4;
	public float Amplitude = 1;

	void Awake()
	{
		_lineRenderer = GetComponent<LineRenderer>();
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
			var f = i / (float)ms.Wavelength.Length;
			_points[i] = transform.localToWorldMatrix.MultiplyPoint3x4(Quaternion.Euler(0, f * Spin * 360, 0) * (Vector3.right * f * Radius) + Vector3.up * ms.Wavelength[i] * Amplitude);
		}
		_lineRenderer.SetPositions(_points);
	}
}
