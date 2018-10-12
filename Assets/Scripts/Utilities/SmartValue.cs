using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

[Serializable]
public class SmartValue
{
    private Queue<float> _values;
    public int BufferSize = 1;
    public int SmoothSize = int.MaxValue;
    public bool NormalizeValue;
    public bool SmoothValue;

    public float Value 
    {
        get
        {
            return GetValue();
        }
    }

    public SmartValue(int bufferSize)
    {
        BufferSize = bufferSize;
        _values = new Queue<float>(BufferSize);
    }    

    public float GetValue()
    {
        if(_values == null || _values.Count == 0)
        {
            return 0;
        }
        if(_values.Count == 1)
        {
            return _values.First();
        }

        var smoothBufferSize = SmoothSize;
        if(smoothBufferSize > BufferSize)
        {
            smoothBufferSize = BufferSize;
        }

        var currentVal = 0f;
        if(SmoothValue)
        {
            float sum = 0;
            int counter = 0;
            foreach (var f in _values)
            {
                sum += f;
                counter++;
                if(counter >= smoothBufferSize)
                {
                    break;
                }
            }            
            currentVal = sum / (float)counter;
        }
        else
        {
            currentVal = _values.Last();
        }
        if(NormalizeValue)
        {
            var min = _values.Min();        
            return (currentVal - min) / Mathf.Max(float.Epsilon, (_values.Max() - min));
        }
        return currentVal;
    }

    public void AddValue(float val)
    {
        _values.Enqueue(val);
        while (_values.Count > BufferSize)
        {
            _values.Dequeue();
        }
    }    
}