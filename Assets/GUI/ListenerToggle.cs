using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListenerToggle : MonoBehaviour {
	public Listener Target;
	public Image Value;
	public Toggle Toggle;
	public Text Label;
	RectTransform _toggleTransform;

	void Awake()
	{
		_toggleTransform = Toggle.transform as RectTransform;
	}

	void Update()
	{
		if(!Target)
		{
			gameObject.SetActive(false);
			return;
		}
		Value.fillAmount = Target.Strength;
		Label.text = Target.name;

		const float tabWidth = 4;
		var ancestorCount = Mathf.Max(0, Target.transform.GetComponentsInAncestors<Listener>().Count - 1);
		_toggleTransform.anchoredPosition = new Vector2(ancestorCount * tabWidth, 0);
		_toggleTransform.sizeDelta = new Vector2(-ancestorCount * tabWidth * 2, 0);
	}

    public void SetTarget(Listener listener)
    {
		Target = listener;
        Toggle.onValueChanged.RemoveAllListeners();
		Toggle.onValueChanged.AddListener((x) => GUIManager.Instance.SetActive(listener, x));
		Label.text = listener.name;

		const float tabWidth = 4;
		var ancestorCount = listener.transform.GetComponentsInAncestors<Listener>().Count;
		_toggleTransform.anchoredPosition = new Vector2(ancestorCount * tabWidth, 0);
		_toggleTransform.sizeDelta = new Vector2(-ancestorCount * tabWidth * 2, 0);
	}
}
