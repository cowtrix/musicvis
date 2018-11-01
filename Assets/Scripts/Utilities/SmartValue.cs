using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

[Serializable]
public class SmartValue
{
    struct Frame{
        public float Value;
        public float Multiplier;

        public Frame(float val, float multiplier)
        {
            Value = val;
            Multiplier = multiplier;
        }

        public float MultipliedValue
        {
            get{
                return Value * Multiplier;
            }
        }
    }

    Frame _actual;
    Frame _target;
    private Queue<Frame> _values;
    public int BufferSize = 1000;
    public float Smooth = 0;
    public bool NormalizeValue = true;
	[Range(0, 1)]
    public float Multiplier = 1;

    public SmartValue(int bufferSize)
    {
        BufferSize = bufferSize;
        _values = new Queue<Frame>(BufferSize);
    }

    public void Tick(float dt)
    {
        var smoothFactor = (1-Smooth) * (1-Smooth);
        _actual.Multiplier = Mathf.MoveTowards(_actual.Multiplier, _target.Multiplier, smoothFactor * dt);
        _actual.Value = Mathf.MoveTowards(_actual.Value, _target.Value, smoothFactor * dt);
    }

    public float GetValue()
    {
        if(_values == null || _values.Count == 0)
        {
            return 0;
        }
        var currentVal = _values.Last();
        _target = currentVal;
        if(_values.Count == 1)
        {
            if(Smooth > 0)
            {
                return _actual.MultipliedValue;
            }
            return currentVal.MultipliedValue;
        }        
            
        if(NormalizeValue)
        {
            const float threshold = .0001f;
            var min = _values.Min((x) => x.Value);
            var max = _values.Max((x) => x.Value);
            if(max - min > threshold)
            {
                _target.Value = (currentVal.Value - min) / (max - min);
            }
        }
        
        if(Smooth > 0)
        {
            return _actual.MultipliedValue;
        }
        return _target.MultipliedValue;
    }

    public void AddValue(float val)
    {
        if(_values == null)
        {
            _values = new Queue<Frame>(BufferSize);
        }
        _values.Enqueue(new Frame(val, Multiplier));
        while (_values.Count > BufferSize)
        {
            _values.Dequeue();
        }
    }    
}