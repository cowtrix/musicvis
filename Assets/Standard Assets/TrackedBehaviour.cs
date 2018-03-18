using System.Collections.Generic;
using UnityEngine;

public class TrackedBehaviour<T> : MonoBehaviour where T: TrackedBehaviour<T>
{
    private static readonly HashSet<T> AllBehaviours = new HashSet<T>();

    public static IEnumerator<T> GetEnumerator()
    {
        return AllBehaviours.GetEnumerator();
    }

    public virtual void OnEnable()
    {
        AllBehaviours.Add(this as T);
    }

    public virtual void OnDisable()
    {
        AllBehaviours.Remove(this as T);
    }
}