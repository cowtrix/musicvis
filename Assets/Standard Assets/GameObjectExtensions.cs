using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;


public static class GameObjectExtensions
{
    public static void SetChildrenActive(this GameObject gameObject, bool value)
    {
        foreach(Transform child in gameObject.transform)
        {
            child.gameObject.SetActive(value);
        }
    }

    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component != null)
        {
            return component;
        }
        return gameObject.AddComponent<T>();
    }

    public static T GetComponentInChildrenDisabled<T>(this Transform transform) where T : Component
    {
        var ret = new List<T>();
        Traverse(transform, ret);
        return ret.Count > 0 ? ret[0] : null;
    }

    public static T[] GetComponentsInSelfAndChildrenDisabled<T>(this Transform transform) where T : Component
    {
        var ret = new List<T>();

        Traverse(transform, ret);

        return ret.ToArray();
    }

    static void Traverse<T>(Transform t, List<T> list) where T : Component
    {
        var test = t.GetComponent<T>();
        if (test)
        {
            list.Add(test);
        }
        foreach (Transform child in t)
        {
            Traverse(child, list);
        }
    }

    public static T GetComponentByInterface<T>(this GameObject gameObject) where T : class
    {
        if (gameObject == null)
        {
            return null;
        }
        return gameObject.GetComponent(typeof(T)) as T;
    }

    public static T[] GetComponentsByInterface<T>(this GameObject gameObject) where T : class
    {
        var components = gameObject.GetComponents(typeof(T));

        var ret = new T[components.Length];

        for (var i = 0; i < ret.Length; i++)
        {
            ret[i] = components[i] as T;
        }

        return ret;
    }

    public static T[] GetComponentsByInterfaceInChildren<T>(this GameObject gameObject) where T : class
    {
        var components = gameObject.GetComponentsInChildren(typeof(T));

        var ret = new T[components.Length];

        for (var i = 0; i < ret.Length; i++)
        {
            ret[i] = components[i] as T;
        }

        return ret;
    }

    public static T GetComponentByInterfaceInChildren<T>(this GameObject gameObject) where T : class
    {
        return gameObject.GetComponentInChildren(typeof(T)) as T;
    }

    public static bool IsNullOrDestroyed(this object target)
    {
        var item = (MonoBehaviour)target;

        return item == null || item.gameObject == null || !item.gameObject.activeInHierarchy;
    }

    public static T GetComponentByInterface<T>(this MonoBehaviour component) where T : class
    {
        return component.GetComponent(typeof(T)) as T;
    }

    public static Component[] GetComponentsByInterface<T>(this MonoBehaviour component) where T : class
    {
        return component.GetComponents(typeof(T));
    }

    public static T GetComponentByInterfaceInAncestors<T>(this Transform component) where T : class
    {
        var result = component.GetComponent(typeof(T)) as T;

        if (result != null)
        {
            return result;
        }

        if (component.transform.parent == null)
        {
            return null;
        }

        return GetComponentByInterfaceInAncestors<T>(component.transform.parent);
    }

    public static List<T> GetComponentsInAncestors<T>(this Transform component, List<T> list = null) where T : Component
    {
        if(list == null)
        {
            list = new List<T>();
        }
        if(component != null)
        {
            var result = component.GetComponent<T>();
            if (result != null)
            {
                list.Add(result);
            }

            if (component.parent == null)
            {
                return list;
            }  
            return GetComponentsInAncestors<T>(component.parent, list);
        }
        return list;
    }

    public static T GetComponentInAncestors<T>(this Transform component) where T : Component
    {
        var result = component.GetComponent<T>();

        if (result != null)
        {
            return result;
        }

        if (component.transform.parent == null)
        {
            return null;
        }
        //TODO: Should this be parent?
        return GetComponentInAncestors<T>(component.transform.parent);
    }

    public static void DestroyChildren(this GameObject gameObject)
    {
        foreach (Transform child in gameObject.transform)
        {
            UnityEngine.Object.Destroy(child.gameObject);
        }

    }
}