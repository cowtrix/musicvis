using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpiralMover : MonoBehaviour 
{
	public float SpawnTime = .25f;

	public float Radius = 10;
	public AnimationCurve RadiusOverTime = AnimationCurve.Linear(0, 1, 1, 1);

	public bool Polygonal;
	[Range(2, 8)]
	public int Sides = 3;
	public float CurveCount = 1;
	
	public float TotalTime = 60;
	public float UpdateTime = 1;
	[Range(0, 1)]
	public float TransitionSpeed = 1f;

	public Vector2 PlaneTarget = new Vector2(0, 0);
	public AnimationCurve PlaneTargetInterpolation = AnimationCurve.Linear(0, 0, 1, 1);

	public float Distance = 100;
	public AnimationCurve DistanceTargetInterpolation = AnimationCurve.Linear(0, 0, 1, 1);

	public Vector3 Scale = new Vector3(1,1,1);
	public AnimationCurve ScaleOverTime = AnimationCurve.Linear(0, 1, 1, 1);

	public List<Mesh> Meshes = new List<Mesh>();
	public List<Material> Materials = new List<Material>();
	public float Time;

	public ColorTemplateManager ColorManager;
	[Range(0, 2)]
	public int ColorIndex;
	public float ColorOffset;

	[Serializable]
	public struct TransformState
	{
		public Vector3 Position;
		public Vector3 Scale;
		public Vector3 Rotation;

		public TransformState (Vector3 pos, Vector3 rot, Vector3 scale)
		{
			Position = pos;
			Rotation = rot;
			Scale = scale;				
		}

		public static TransformState Lerp(TransformState a, TransformState b, float w)
		{
			return new TransformState(Vector3.Lerp(a.Position, b.Position, w), 
				Vector3.Lerp(a.Rotation, b.Rotation, w), 
				Vector3.Lerp(a.Scale, b.Scale, w));
		}

		public Matrix4x4 ToMatrix()
		{
			return Matrix4x4.TRS(Position, Quaternion.Euler(Rotation), Scale);
		}
	}

	[Serializable]
	public class Instance
	{
		public TransformState CurrentState;
		public TransformState TargetState;
		public float SpawnTime;
		public Mesh Mesh;
		public Material Material;
		public Color Color;
	}

	List<Instance> _instances = new List<Instance>();
	List<Vector2> _polygonPoints = new List<Vector2>();		
	float _spawnTimer;
	float _lastSpawnTime = 0;
	float _updateTimer = 0;
	static MaterialPropertyBlock _materialPropertyBlock;

	public void AddToTime(float t)
	{
		Time += t;
	}

	public void RandomPolySettings(float f)
	{
		if(f < 0.1f)
		{
			return;
		}
		var r = UnityEngine.Random.value;
		if(r < 0.5f)
		{
			Polygonal = false;
			return;
		}
		Polygonal = true;
		Sides = UnityEngine.Random.Range(2, 8);
	}

	private void Update()
	{
		float time = Time;
		float dt = UnityEngine.Time.deltaTime;
		_spawnTimer -= dt;
		_updateTimer -= dt;

		int spawnCount = 1;
		while(_spawnTimer < 0)
		{
			var newInstance = new Instance()
			{
				Mesh = Meshes.Random(),
				Material = Materials.Random(),
				SpawnTime = _lastSpawnTime + SpawnTime * spawnCount,
				CurrentState = new TransformState(Vector3.back * 100, Vector3.zero, Vector3.one),
				TargetState = new TransformState(Vector3.back * 100, Vector3.zero, Vector3.one),
				Color = ColorManager == null ? Color.white : ColorManager.GetTemplateAtTime(ColorOffset).GetByIndex(ColorIndex),
			};
			_instances.Add(newInstance);
			_spawnTimer += SpawnTime;
			_lastSpawnTime = time;
			spawnCount++;
		}
		
		// Rebuild poly points if needed
		if(_polygonPoints.Count != Sides)
		{
			_polygonPoints.Clear();
			for(var i = 0; i < Sides; ++i)
			{
				var tPi = (i / (float)(Sides - 1)) * Mathf.PI * 2;
				_polygonPoints.Add(new Vector2(Mathf.Sin(tPi), Mathf.Cos(tPi)));
			}
		}

		for(int i = _instances.Count - 1; i >= 0; --i)
		{
			var instance = _instances[i];
			if(time - instance.SpawnTime > TotalTime)
			{
				_instances.RemoveAt(i);
				continue;
			}
			
			if(_updateTimer < 0)
			{
				instance.TargetState = GetTransformState(time - instance.SpawnTime);
			}
			
			instance.CurrentState = TransformState.Lerp(instance.CurrentState, instance.TargetState, TransitionSpeed);
			if(_materialPropertyBlock == null)
			{
				_materialPropertyBlock = new MaterialPropertyBlock();
			}
			_materialPropertyBlock.Clear();
			_materialPropertyBlock.SetColor("_Color", instance.Color);
			Graphics.DrawMesh(instance.Mesh, transform.localToWorldMatrix * instance.CurrentState.ToMatrix(), instance.Material, 0, null, 0, _materialPropertyBlock);		
		}
		if(_updateTimer < 0)
		{
			_updateTimer = UpdateTime;
		}
	}

	TransformState GetTransformState(float t)
	{
		float totalTime = TotalTime;
		float nt = t / totalTime;		

		float curveTime = nt * CurveCount;

		//Circular
		float degRad = curveTime * Mathf.PI * 2;
		Vector3 circleAngle = new Vector3(Mathf.Sin(degRad), Mathf.Cos(degRad), 0);

		var rotation = -Quaternion.LookRotation(circleAngle.normalized, Vector3.forward).eulerAngles;

		// Poly
		if(Polygonal)
		{
			float fIndex = Mathfx.Frac(curveTime) * (_polygonPoints.Count - 1);
			int lowIndex = Mathf.FloorToInt(fIndex);
			int highIndex = Mathf.CeilToInt(fIndex);
			if(highIndex >= _polygonPoints.Count)
			{
				highIndex = 0;
			}
			if(lowIndex < 0 || lowIndex >= _polygonPoints.Count)
			{
				lowIndex = 0;
			}

			float frac = Mathfx.Frac(fIndex);
			
			Vector2 point = Vector2.Lerp(_polygonPoints[lowIndex], _polygonPoints[highIndex], frac);
			circleAngle = point;

			//rotation = Vector3.Cross(_polygonPoints[lowIndex] - _polygonPoints[highIndex], _polygonPoints[lowIndex]);
		}
		
		var flatTarget = Vector2.Lerp(Vector2.zero, PlaneTarget, PlaneTargetInterpolation.Evaluate(nt));
		var z = Mathf.Lerp(0, Distance, DistanceTargetInterpolation.Evaluate(nt));
		var targetPos = new Vector3(flatTarget.x, flatTarget.y, z);

		var circlePos = circleAngle * RadiusOverTime.Evaluate(nt) * Radius;
		var finalPosition = targetPos + circlePos;
		
		var scale = Scale * ScaleOverTime.Evaluate(nt);

		return new TransformState(finalPosition, rotation, scale);
	}
}
