using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SmartValue
{
    private Queue<float> _values;
    public int BufferSize = 1;
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
        var currentVal = SmoothValue ? _values.Average() : _values.Last();
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