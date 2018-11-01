using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildDisabler : Mutator {
	public float Threshold = 0.01f;

    protected override void TickInternal(float strength)
    {
        gameObject.SetChildrenActive(strength > Threshold);
    }
}
