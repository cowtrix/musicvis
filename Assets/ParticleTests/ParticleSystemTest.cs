using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MadMaps.Common;

public class ParticleSystemTest : Mutator 
{
	public ParticleSystem System;
	public Transform FocalPoint;
	ParticleSystem.Particle[] _particles;

	public float ValueMultiplier = 1;
	public Vector2 Size = new Vector2 (10, 10);
	public int Rows = 10;
	public int Columns = 10;
	public int BufferSize = 1000;
	public float ColorFrequency = 1;
	public bool Exponential;

	List<float> _buffer = new List<float>();

	public ColorTemplateManager TemplateManager;

	public override void Tick(float strength)
	{
		System.maxParticles = Rows * Columns;
		float maxDist = Mathf.Sqrt(Rows * Size.x * Columns * Size.y);
		while (_buffer.Count > BufferSize)
		{
			_buffer.RemoveAt(0);
		}

		if(Exponential)
		{
			strength *= strength;
		}
		strength *= ValueMultiplier;
		
		_buffer.Add(strength);

		if(_particles == null || _particles.Length != System.main.maxParticles)
		{
			_particles = new ParticleSystem.Particle[System.main.maxParticles];
		}
		int numParticlesAlive = System.GetParticles(_particles);

		Vector2 min = transform.position.xz() - Size / 2;
		Vector2 step = new Vector3(Size.x / (Rows-1), Size.y / (Columns-1));
		int rowCounter = 0;
		int columnCounter = 0;
		for (int i = 0; i < numParticlesAlive; i++)
        {
			var newParticlePos = new Vector2(min.x, min.y) + new Vector2(step.x * rowCounter, step.y * columnCounter);
			float distance = Vector2.Distance(FocalPoint.transform.position.xz(), newParticlePos);
			distance = Mathf.Clamp01(distance / maxDist);
			int index = _buffer.Count - 1 - Mathf.FloorToInt(distance * _buffer.Count);
			index = Mathf.Clamp(index, 0, _buffer.Count - 1);
			if(index > _buffer.Count)
			{
				_particles[i].position = new Vector3(newParticlePos.x, 0, newParticlePos.y);
			}
			else
			{
				_particles[i].position = new Vector3(newParticlePos.x, _buffer[index], newParticlePos.y);
			}
			_particles[i].startColor = TemplateManager.GetTemplateAtTime(distance * ColorFrequency).Color0;
            
			rowCounter++;
			if(rowCounter == Rows)
			{
				rowCounter = 0;
				columnCounter++;
			}
        }

		System.SetParticles(_particles, numParticlesAlive);
	}
}
