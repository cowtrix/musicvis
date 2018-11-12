using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlanetRenderer : MonoBehaviour {

	public Mesh Mesh;
	public Material Material;
	public const int Count = 4;
	public float StartDistance = 1f;
	public float RotationSpeed = 0.1f;

	class Instance
	{
		public Vector3 Position;
		public float Scale = 1;
		public Vector3 Rotation;
		public Vector3 RotationDelta;
		public Instance Next;

		public Instance(float distance, int generation)
		{
			Position = Random.onUnitSphere * distance;
			Rotation = Random.rotation.eulerAngles;
			Scale = (generation / (float)PlanetRenderer.Count) * .25f;
			RotationDelta = Random.onUnitSphere * Scale * Scale;

			generation--;
			if(generation > 0)
			{
				Next = new Instance(distance * .05f, generation);
			}
		}

        public void Tick(float f)
        {
            Rotation += RotationDelta * f;
			if(Next != null)
			{
				Next.Tick(f);
			}
        }

        public void Render(Matrix4x4 localToWorldMatrix, Mesh mesh, Material material)
        {
            var pos = localToWorldMatrix.MultiplyPoint3x4(Position);
			var rot = Quaternion.Euler(localToWorldMatrix.rotation * Rotation);
			//pos += rot * Position;

			var mat = Matrix4x4.TRS(pos, rot, Vector3.one * Scale);
			Graphics.DrawMesh(mesh, mat, material, 0, null);

			if(Next != null)
			{
				Next.Render(mat * localToWorldMatrix, mesh, material);
			}
        }
    }

	Instance _rootInstance;

	void Awake()
	{
		_rootInstance = new Instance(StartDistance, Count);
	}

	public void Tick(float f)
	{
		_rootInstance.Tick(f * RotationSpeed);
	}

	void Update()
	{
		if(!Mesh || !Material)
		{
			return;
		}
		
		_rootInstance.Render(transform.localToWorldMatrix, Mesh, Material);
	}
}
