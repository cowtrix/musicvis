using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabRingArray : MonoBehaviour {

	public GameObject Prefab;
	public int Count = 32;
	[Range(0, 1)]
	public float Rotation;
	public float Radius = 100;
	public Vector3 AdditionalRotation;
	List<GameObject> _pool = new List<GameObject>();

	void Update()
	{
		for(var i = 0; i < Count; ++i)
		{
			if(_pool.Count <= i)
			{
				var newObj = Instantiate(Prefab);
				newObj.transform.SetParent(transform);
				_pool.Add(newObj);
			}
			var prefab = _pool[i];
			var f = i / (float)Count;
			prefab.transform.localPosition = Quaternion.Euler(new Vector3(Rotation * 90, f * 360, 0)) * Vector3.right * Radius;
			prefab.transform.LookAt(prefab.transform.localPosition + prefab.transform.localPosition);
			prefab.transform.localRotation *= Quaternion.Euler(AdditionalRotation);
		}
	}
}
