using System.Collections.Generic;
using UnityEngine;

public class RefTrackedBehavior<T> : MonoBehaviour where T : RefTrackedBehavior<T>
{
    private static readonly Dictionary<Transform, T> _transformLookup = new Dictionary<Transform,T>();
    private bool _isRegistered = false;

    protected virtual void OnEnable()
    {
        TryEnable();
    }
     
    private void TryEnable()
    {
        if (!_isRegistered)
        {
            //_registeredItems.Add((T)this);

#if UNITY_EDITOR
            if (_transformLookup.ContainsKey(transform))
            {
                Debug.LogError("Multiple ref tracked behaviors of the same type on the same object, this is a no no: " + gameObject.name + " - " + typeof(T).FullName, this);
            }
#endif
            _transformLookup[transform] = (T)this;

            _isRegistered = true;
        }
    }

    protected virtual void OnDisable()
    {
        TryDisable();
    }

    protected virtual void OnDestroy()
    {
        TryDisable();
    }

    private void TryDisable()
    {
        if (_isRegistered)
        {
            /*var index = _registeredItems.IndexOf((T) this);
            if (index != -1)
            {
                _registeredItems.RemoveAt(index);
            }*/
            _transformLookup.Remove(transform);
            _isRegistered = false;
        }
    }
    
    public static Dictionary<Transform, T>.Enumerator GetEnumerator()
    {
        return _transformLookup.GetEnumerator();
    }

    public static int GetObjectCount()
    {
        return _transformLookup.Count;
    }

    public static T GetByTransform(Transform trans)
    {
        T result;
        _transformLookup.TryGetValue(trans, out result);
        return result;
    }

    public static T GetRandomInstance()
    {
        var enumerator = _transformLookup.GetEnumerator();
        var chosenIndex = Random.Range(0, _transformLookup.Count);
        int counter = 0;
        while (enumerator.MoveNext())
        {
            if (counter == chosenIndex)
            {
                return enumerator.Current.Value;
            }
            ++counter;
        }
        Debug.LogError(string.Format("Failed to get random instance of type {0}! Were there any?", typeof(T).ToString()));
        return null;
    }
}