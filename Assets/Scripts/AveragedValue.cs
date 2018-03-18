using System.Collections.Generic;
using System.Linq;

public class AveragedValue
{
    private Queue<float> _values;
    public int Size;

    public AveragedValue(int steps)
    {
        Size = steps;
        _values = new Queue<float>(steps);
    }

    public float Multiplier = 1;

    public float GetValue()
    {
        if (_values.Count == 0 || _values.Count < Size)
        {
            return 0;
        }
        return _values.Average() * Multiplier;
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