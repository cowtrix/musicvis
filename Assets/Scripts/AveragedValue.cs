using System.Collections.Generic;
using System.Linq;

public class AveragedValue
{
    private Queue<float> _values;
    public readonly int Size;

    public AveragedValue(int steps)
    {
        Size = steps;
        _values = new Queue<float>(steps);
    }

    public float GetValue()
    {
        if (_values.Count == 0)
        {
            return 0;
        }
        return _values.Average();
    }

    public void Add(float val)
    {
        _values.Enqueue(val);
        while (_values.Count > Size)
        {
            _values.Dequeue();
        }
    }
}