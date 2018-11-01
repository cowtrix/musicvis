using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Nuerons : MonoBehaviour 
{
	[Serializable]
	public struct Phase{
		public Vector3 Phase1, Phase2;
	}
	public float GridSize = 1f;
	public Mesh GridMesh;
	public Material GridMaterial;
	public int CellRadius = 100;
	public TrailRenderer ThoughtPrefab;
	public int ThoughtCount = 4;
	public float ThoughtSpeed = 1;
	public Vector2 Scale = new Vector2(.5f, 1);
	public float PhaseSpeed = 1;
	public List<Phase> Phases = new List<Phase>();
	float _phase;

	MaterialPropertyBlock _block;
	List<Thought> _thoughts = new List<Thought>();

	class Thought
	{
		public Transform Transform;
		public Vector3 Destination;
	}

	void Start()
	{
		_block = new MaterialPropertyBlock();
	}

	public void Tick(float f)
	{
		_phase += f * PhaseSpeed;
		if(Phases.Count == 0)
		{
			return;
		}
		while(_phase > Phases.Count)
		{
			_phase -= Phases.Count;
		}
	}

	void OnDrawGizmosSelected()
	{
		if(Phases.Count < 2)
		{
			return;
		}
		for(var i = 1; i < Phases.Count; ++i)
		{
			var last = Phases[i-1];
			var thisOne = Phases[i];

			Gizmos.color = Color.blue;
			Gizmos.DrawLine(last.Phase1, thisOne.Phase1);
			Gizmos.color = Color.green;
			Gizmos.DrawLine(last.Phase2, thisOne.Phase2);
		}	
	}

	Vector3 GetPhase(int number)
	{
		var phase = _phase;
		if(number == 0)
		{
			//phase =  Phases.Count -_phase;
		}
		var lower = Mathf.FloorToInt(phase);
		var upper = Mathf.CeilToInt(phase);
		if(upper == Phases.Count)
		{
			upper = 0;
		}
		var f = Mathfx.Frac(phase);
		var lowerPhase = Phases[lower];
		var upperPhase = Phases[upper];
		return Vector3.Lerp(lowerPhase.Phase1, upperPhase.Phase1, f);
	}

	void Update()
	{
		var ms = MusicManager.MusicState;
		while(_thoughts.Count < ThoughtCount)
		{
			var newThought = Instantiate(ThoughtPrefab.gameObject);
			newThought.transform.SetParent(transform);
			var newT = new Thought()
			{
				Transform = newThought.transform,			
			};
			newT.Destination = newThought.transform.localPosition;
			_thoughts.Add(newT);
		}

		for(var i = 0; i < _thoughts.Count; ++i)
		{
			var t = _thoughts[i];
			if(Vector3.Distance(t.Destination, t.Transform.localPosition) < .1f)
			{
				Random.InitState(Time.frameCount + i);
				t.Destination = new Vector3(Random.Range(-CellRadius, CellRadius) * GridSize,
					Random.Range(-CellRadius, CellRadius) * GridSize,
					Random.Range(-CellRadius, CellRadius) * GridSize);
			}
			t.Transform.localPosition = Vector3.MoveTowards(t.Transform.localPosition, t.Destination, ThoughtSpeed * ms.Peak);
		}

		var cellDiameter = CellRadius * 2;
		var totalRadius = (CellRadius * GridSize) / 2f;
		bool lastPhase = false;
		for(var x = -CellRadius; x < CellRadius; ++x)
		{
			lastPhase = !lastPhase;
			for(var y = -CellRadius; y < CellRadius; ++y)
			{
				lastPhase = !lastPhase;
				for(var z = -CellRadius; z < CellRadius; ++z)
				{
					var phase = GetPhase(lastPhase ? 1 : 0) * GridSize;
					lastPhase = !lastPhase;
					var pos = new Vector3(x, y, z) * GridSize + phase;
					
					var opacity = pos.magnitude / totalRadius;

					int magicInt = ((x + y) * z);
					Random.InitState(magicInt);

					var wl = ms.Wavelength[Random.Range(0, ms.Wavelength.Length-1)] % 1;
					var s = Scale.x + (Scale.y - Scale.x) * wl;

					var mat = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one * s * GridSize);

					_block.SetColor("_Color", new Color(1, 1, 1, opacity));
					Graphics.DrawMesh(GridMesh, transform.localToWorldMatrix * mat, GridMaterial, 0, null, 0, _block);
				}
			}
		}
	}
}
