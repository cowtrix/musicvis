using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mutator : MonoBehaviour 
{
	public virtual void Tick(float strength)
	{
		/*if(strength <= 0)
		{
			if(gameObject.activeSelf)
			{
				gameObject.SetActive(false);
			}
			return;
		}
		if(!gameObject.activeSelf)
		{
			gameObject.SetActive(true);
		}*/
		TickInternal(strength);
	}

	protected abstract void TickInternal(float strength);
}
