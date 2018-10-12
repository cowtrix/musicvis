using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour 
{
	public List<GameObject> Prefabs = new List<GameObject>();
	public float SpawnSpeed = 1;

	float _timeTilNextSpawn;

	private void Update()
	{
		_timeTilNextSpawn -= Time.deltaTime;
		while(_timeTilNextSpawn < 0)
		{
			_timeTilNextSpawn += SpawnSpeed;
			var newGo = Instantiate(Prefabs.Random()) as GameObject;
			newGo.transform.SetParent(transform);
			newGo.transform.localPosition = Vector3.zero;
			newGo.transform.localRotation = Quaternion.identity;
			newGo.transform.localScale = Vector3.one;
		}
	}
}
