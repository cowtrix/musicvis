using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mutator : MonoBehaviour 
{
	public abstract void Tick(float strength, float weight);
}
