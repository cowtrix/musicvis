using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class SpiralMoverConfiguration : ScriptableObject 
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
}
