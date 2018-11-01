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
			_block.SetColor("_Color", Random.ColorHSV(0, 1, 0.8f, 1f, 0.8f, 1f));
			Graphics.DrawMesh(Mesh, trs, Material, 0, null, 0, _block);
		}
	}
}
