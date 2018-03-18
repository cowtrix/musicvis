using UnityEngine;

public class AutoComponent<T> where T : Component
{
    public T Value
    {
        get
        {
            if (__value == null)
            {
                __value = _context.GetOrAddComponent<T>();
            }
            return __value;
        }
    }
    private T __value;
    private readonly GameObject _context;

    public AutoComponent(GameObject context)
    {
        _context = context;
    }

    public static implicit operator T(AutoComponent<T> auto)
    {
        return auto.Value;
    }
}