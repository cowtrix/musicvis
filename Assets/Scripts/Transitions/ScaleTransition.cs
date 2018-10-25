using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTransition : Transition {

	public Vector2 Scale = new Vector2(0, 1);
	public bool FadeOut;
	public float FadeSpeed = 10f;
	bool _autoGrow;

	public override void Tick(float strength)
	{
		transform.localScale = Mathf.Lerp(Scale.x, Scale.y, strength) * Vector3.one;
	}

	void OnEnable()
	{
		transform.localScale = Scale.x * Vector3.one;
		_autoGrow = true;
	}

	void OnDisable()
	{
		FadeOut = false;
	}

	void Update()
	{
		if(_autoGrow)
		{
			if(transform.localScale.x < Scale.y)
			{
				transform.localScale = Vector3.MoveTowards(transform.localScale, Scale.y * Vector3.one, FadeSpeed * Time.deltaTime);
			}
			else{
				_autoGrow = false;
			}
		}
		
		if(FadeOut)
		{
			if(transform.localScale.x > Scale.x)
			{
				transform.localScale = Vector3.MoveTowards(transform.localScale, Scale.x * Vector3.one, FadeSpeed * Time.deltaTime);
			}
			else{
				gameObject.SetActive(false);
			}
		}
	}
}
