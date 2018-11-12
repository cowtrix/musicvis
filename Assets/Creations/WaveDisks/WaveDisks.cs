using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveDisks : MonoBehaviour {
	public Mesh Mesh;
	public Material Material;
	public Vector3 MeshRotation;
	public int Step = 4;
	List<Quaternion> _rotationHistory = new List<Quaternion>();
	MaterialPropertyBlock _block;
	public Vector3 Scale = Vector3.one;
	public Gradient Color;
	public float ColorTiling = 1;
	public Vector2 Range = new Vector2(0, 1);
	void Update()
	{
		if(_block == null){
			_block = new MaterialPropertyBlock();
		}
		var ms = MusicManager.MusicState;
		var w = ms.Wavelength;

		_rotationHistory.Add(transform.rotation);
		while(_rotationHistory.Count > w.Length)
		{
			_rotationHistory.RemoveAt(0);
		}

		for(var i = 0; i < w.Length; i += Step)
		{
			var percent = i / (float)w.Length;
			if(percent < Range.x)
			{
				continue;
			}
			var f = w[i];
			var r = transform.rotation * Quaternion.Euler(MeshRotation);
			if(i < _rotationHistory.Count)
			{
				r = _rotationHistory[i];
			}
			var s = transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3((i + f) * Scale.x, (i + f) * Scale.y, (i + f) * Scale.z));
			var trs = Matrix4x4.TRS(transform.position, r, s);
			_block.SetFloat("_U", i / (float)w.Length);
			_block.SetFloat("_Strength", f);
			Random.InitState(i);
			_block.SetColor("_Color", Color.Evaluate(Mathfx.Frac((i / ColorTiling))));
			Graphics.DrawMesh(Mesh, trs, Material, 0, null, 0, _block);
		}
	}
}
