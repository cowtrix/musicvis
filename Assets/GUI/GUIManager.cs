using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour 
{
	public Text Header;
	public Slider Slider;

	public RectTransform Container;

	List<MusicVisualisation> _visualisations = new List<MusicVisualisation>();
	List<VisUI> _uis = new List<VisUI>();
	bool _uiToggled = true;
	CanvasGroup _canvasGroup;

	private class VisUI
	{
		public Text Header;
		//public Slider Weight;
		public Dictionary<Slider, ExplicitListener> Listeners = new Dictionary<Slider, ExplicitListener>();
		public MusicVisualisation Visualisation;
	}

	private void Awake()
	{
		_canvasGroup = GetComponent<CanvasGroup>();

		_visualisations.Clear();
		_visualisations.AddRange(FindObjectsOfType<MusicVisualisation>());

		foreach (var visualisation in _visualisations)
		{
			var newUI = new VisUI();
			newUI.Visualisation = visualisation;

			newUI.Header = Instantiate(Header);
			newUI.Header.text = visualisation.name;
			newUI.Header.transform.SetParent(Container);

			/*newUI.Weight = Instantiate(Slider);
			newUI.Weight.transform.SetParent(Container);
			newUI.Weight.GetComponentInChildren<Text>().text = "Weight";
			newUI.Weight.value = visualisation.Strength;
			newUI.Weight.onValueChanged.AddListener((f) => visualisation.Strength = f);*/
			
			for(var i = 0; i < visualisation.Components.Count; ++i)
			{
				var component = visualisation.Components[i];
				var explicitListener = component.Listener as ExplicitListener;
				if(explicitListener == null)
				{
					continue;
				}
				var newSlider = Instantiate(Slider);
				newSlider.transform.SetParent(Container);				
				newSlider.GetComponentInChildren<Text>().text = component.Name;
				newSlider.value = explicitListener.Value;
				newSlider.onValueChanged.AddListener((f) => 
					{
						if(explicitListener != null)
						{
							explicitListener.Value = f;
						}
					}
				);
				newUI.Listeners.Add(newSlider, explicitListener);
			}
			_uis.Add(newUI);
		}
	}

	private void Update()
	{
		if(Input.GetKeyUp(KeyCode.Tab))
		{
			_uiToggled = !_uiToggled;
		}
		_canvasGroup.alpha = _uiToggled ? 1 : 0;
	}

	private void LateUpdate()
	{
		foreach(var ui in _uis)
		{
			//ui.Visualisation.Strength = ui.Weight.value;
			foreach(var slider in ui.Listeners)
			{
				slider.Value.Value = slider.Key.value;
			}
		}
	}
}
