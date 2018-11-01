using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutatorEffectorGroup : MonoBehaviour 
{
	public MutatorEffector MutatorEffector;
	public List<Mutator> Mutators = new List<Mutator>();

	void Start()
	{
		foreach(var mutator in Mutators)
		{
			MutatorEffector.Event.AddListener((x) => mutator.Tick(x));
		}
	}
}
