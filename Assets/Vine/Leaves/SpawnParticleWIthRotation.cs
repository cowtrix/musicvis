using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnParticleWithRotation : MonoBehaviour 
{
	public Vector2 SpawnTime = new Vector2(1,1);
	public ParticleSystem ParticleSystem;

	private float _timeTilNextSpawn = 0;

	// Update is called once per frame
	void Update () 
	{
		if(!ParticleSystem)
		{
			return;
		}
		_timeTilNextSpawn -= Time.deltaTime;
		if(_timeTilNextSpawn > 0)
		{
			return;
		}
		_timeTilNextSpawn = Random.Range(SpawnTime.x, SpawnTime.y);
		
		var emitParams = new ParticleSystem.EmitParams();
		
	}
}
