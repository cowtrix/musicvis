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
	public float SizeMultiplier = 1;
	public int Rows = 10;
	public int Columns = 10;
	public int BufferSize = 1000;
	public float ColorFrequency = 1;
	public bool Exponential;
	public float AlphaBoost = 1;

	List<float> _buffer = new List<float>();
	public SmartValue AlphaValue = new SmartValue(1);

	public ColorTemplateManager TemplateManager;

	protected override void TickInternal(float strength)
	{
		Debug.Log(System.textureSheetAnimation);

		System.maxParticles = Rows * Columns;
		Vector2 realSize = SizeMultiplier * Size;
		float maxDist = Mathf.Sqrt(Rows * realSize.x * Columns * realSize.y);
		while (_buffer.Count > BufferSize)
		{
			_buffer.RemoveAt(0);
		}
		float alpha = Mathf.Clamp01(strength) * AlphaBoost;
		AlphaValue.AddValue(alpha);
		alpha = AlphaValue.GetValue();
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

		
		Vector2 min = transform.position.xz() - realSize / 2;
		Vector2 step = new Vector3(realSize.x / (Rows-1), realSize.y / (Columns-1));
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
			_particles[i].startColor = TemplateManager.GetTemplateAtTime(distance * ColorFrequency).Color0.WithAlpha(alpha);
            
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
