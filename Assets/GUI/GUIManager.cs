using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using MadMaps.Common;

public class GUIManager : Singleton<GUIManager> 
{
	public ListenerDisplay DisplayPrefab;
	public RectTransform DisplayContainer;
	public ListenerToggle TogglePrefab;
	public RectTransform ToggleContainer;

	List<Listener> _listeners = new List<Listener>();
	List<ListenerToggle> _toggles = new List<ListenerToggle>();
	List<ListenerDisplay> _activeDisplays = new List<ListenerDisplay>();
	Queue<ListenerDisplay> _inactiveDisplays = new Queue<ListenerDisplay>();
	bool _toggleListDirty;

	public void RegisterListener(Listener listener)
	{
		if(_listeners.Contains(listener))
		{
			return;
		}
		_listeners.Add(listener);
		_toggleListDirty = true;
	}

	void Update()
	{
		if(_toggleListDirty)
		{
			RefreshToggles();
		}
	}

	void RefreshToggles()
	{
		_toggleListDirty = false;
		_listeners = _listeners.OrderByDescending((x) => x.transform.GetHierarchyIndex()).ToList();
		for(var i = 0; i < _listeners.Count; ++i)
		{
			var listener = _listeners[i];
			if(i <= _toggles.Count)
			{
				var newToggle = Instantiate(TogglePrefab.gameObject).GetComponent<ListenerToggle>();
				newToggle.transform.SetParent(ToggleContainer);
				_toggles.Add(newToggle);
			}
			var t = _toggles[i];
			t.gameObject.SetActive(true);
			t.SetTarget(listener);			
		}
		for(var i = _listeners.Count; i < _toggles.Count; ++i)
		{
			_toggles[i].gameObject.SetActive(false);
		}
	}

	public void SetActive(Listener l, bool val)
	{
		ListenerDisplay current = null;
		foreach(var disp in _activeDisplays)
		{
			if(disp.Target == l)
			{
				current = disp;
				break;
			}
		}
		if((current && val) || (!current && !val))
		{
			// Nothing to do here
			return;
		}
		
		if(!val)
		{
			current.gameObject.SetActive(false);
			_activeDisplays.Remove(current);
			_inactiveDisplays.Enqueue(current);
			var tog = _toggles.First((x) => x.Target == l);
			if(tog)
			{
				tog.Toggle.isOn = false;
			}
			return;
		}
		else
		{
			ListenerDisplay display = null;
			if(_inactiveDisplays.Count == 0)
			{
				display = Instantiate(DisplayPrefab.gameObject).GetComponent<ListenerDisplay>();
				display.transform.SetParent(DisplayContainer);
			}
			else
			{
				display = _inactiveDisplays.Dequeue();
			}
			_activeDisplays.Add(display);
			display.gameObject.SetActive(true);
			display.SetTarget(l);
		}
		
	}
}
