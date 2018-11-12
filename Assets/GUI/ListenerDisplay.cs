using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Gradient = UnityEngine.Gradient;
using MidiJack;

public class ListenerDisplay : MonoBehaviour 
{
	

	public Toggle MidiControlled;
	public Dropdown MidiChannelList;
	public InputField MidiIndex;

	public Listener Target;
	public Text Header;
	public Slider MultiplierSlider;
	public Image Actual;
	public Gradient ValueDisplayGradient;
	public Text BuffDisplay;
	public Toggle NormaliseToggle;
	public Slider BufferSizeSlider;
	public Slider SmoothSizeSlider;
	public Text SmoothDisplay;
	bool _freezeSliders = false;

	void Start()
	{
		var midiOptions = new List<Dropdown.OptionData>();
		for(var i = 0; i < 16; ++i)
		{
			var option = new Dropdown.OptionData("CH" + i);
			midiOptions.Add(option);
		}
		MidiChannelList.ClearOptions();
		MidiChannelList.AddOptions(midiOptions);
	}

    public void SetTarget(Listener l)
    {
		Target = l;
		MultiplierSlider.value = Target.Value.Multiplier;
		
		_freezeSliders = true;

		BufferSizeSlider.minValue = Listener.MIN_BUFFER;
		BufferSizeSlider.maxValue = Listener.MAX_BUFFER;
		BufferSizeSlider.value = Target.Value.BufferSize;

		SmoothSizeSlider.minValue = 0;
		SmoothSizeSlider.maxValue = 1;
		SmoothSizeSlider.value = Target.Value.Smooth;

		NormaliseToggle.isOn = Target.Value.NormalizeValue;

		MidiControlled.isOn = Target.UseMidi;
		//MidiIndex.value = Target.MidiIndex;
		MidiChannelList.value = ((int)Target.MidiChannel);

		MidiIndex.text = Target.MidiIndex.ToString();

		_freezeSliders = false;
    }

	void Update()
	{
		Header.text = Target.GetName();

		var actual = Target.Strength;
		Actual.fillAmount = actual;
		Actual.color = ValueDisplayGradient.Evaluate(actual);

		if(!Target.UseMidi)
		{
			Target.Value.Multiplier = MultiplierSlider.value;
		}
		else
		{
			MultiplierSlider.value = Target.Value.Multiplier;
		}

		BuffDisplay.text = Target.Value.BufferSize.ToString();

		SmoothDisplay.text = Target.Value.Smooth.ToString("0.00");

		MultiplierSlider.interactable = !Target.UseMidi;

		if(!Target.UseMidi)
		{
			MidiChannelList.transform.parent.gameObject.SetActive(false);
			MidiIndex.transform.parent.gameObject.SetActive(false);
		}
		else{
			MidiChannelList.transform.parent.gameObject.SetActive(true);
			MidiIndex.transform.parent.gameObject.SetActive(true);
		}
	}

	public void SetMidiChannel(int i)
	{
		if(_freezeSliders || Target == null)
		{
			return;
		}
		Target.MidiChannel = (MidiChannel)i;
	}

	public void SetMidiIndex(string i)
	{
		if(_freezeSliders || Target == null)
		{
			return;
		}
		Target.MidiIndex = int.Parse(i);
	}

	public void SetMidiControlled(bool b)
	{
		if(_freezeSliders || Target == null)
		{
			return;
		}
		Target.UseMidi = b;
	}

	public void SetNormalize(bool normalise)
	{
		if(_freezeSliders || Target == null)
		{
			return;
		}
		Target.Value.NormalizeValue = normalise;
	}

	public void SetBufferSize(float value)
	{
		if(_freezeSliders || Target == null)
		{
			return;
		}
		Target.Value.BufferSize = Mathf.RoundToInt(value);
	}

	public void SetSmoothSize(float value)
	{
		if(_freezeSliders || Target == null)
		{
			return;
		}
		Target.Value.Smooth = value;
	}

	public void Close()
	{
		GUIManager.Instance.SetActive(Target, false);
	}
}
