using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Mutator Effectors/Threshold Event")]
public class ThresholdEventMutatorEffector : MutatorEffector
{
	public float TriggerValue = .5f;
	public float EventDuration = 1;
	public AnimationCurve ValueOverTime;

	float _playingTimer = 0;

	protected override void Tick(float strength)
	{
		Value = 0;
		if(_playingTimer < 0)
		{
			if(strength > TriggerValue)
			{
				_playingTimer = EventDuration;
			}
			else
			{				
				return;
			}			
		}
		_playingTimer -= Time.deltaTime;
		var normalisedT = 1 - Mathf.Clamp01(_playingTimer / EventDuration);
		Value = ValueOverTime.Evaluate(normalisedT);
	}
}