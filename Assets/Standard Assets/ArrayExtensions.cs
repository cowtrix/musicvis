using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

public static class ArrayExtensions
{
    public static bool RemoveByEquals<T>(this LinkedList<T> list, T item)
    {
        for (var node = list.First; node != null; node = node.Next)
        {
            if (item.Equals(node.Value))
            {
                list.Remove(node);
                return true;
            }
        }
        return false;
    }

    public static bool ContainsEquals<T>(this LinkedList<T> list, T item)
    {
        for (var node = list.First; node != null; node = node.Next)
        {
            if (item.Equals(node.Value))
            {
                return true;
            }
        }
        return false;
    }

    public static int IndexOf<T>(this LinkedList<T> list, T item)
    {
        var count = 0;
        for (var node = list.First; node != null; node = node.Next, count++)
        {
            if (item.Equals(node.Value))
                return count;
        }
        return -1;
    }

    public static void Fill<T>(this IList<T> array, T obj, int count = -1)
    {
        array.Clear();
        if (count < 0)
        {
            count = array.Count;
        }
        for (var i = 0; i < count; ++i)
        {
            array.Add(obj);
        }
    }

    public static void Fill<T>(this T[] array, T obj, int count = -1)
    {
        if (count < 0)
        {
            count = array.Length;
        }
        for (var i = 0; i < count; ++i)
        {
            array[i] = obj;
        }
    }

    public static int GetOrderIndependentHashCode<T>(this IList<T> source)
    {
        int hash = 0;
        for (int i = 0; i < source.Count; i++)
        {
            if (source[i] != null)
            {
                hash = hash ^ source[i].GetHashCode();
            }
        }
        return hash;
    }

    public static int GetContentBasedHashCode<TK, TV>(this IDictionary<TK, TV> source)
    {
        int hash = 0;
        var enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
        {
            int key = enumerator.Current.Key.GetHashCode(); // key cannot be null
            if (enumerator.Current.Value == null)
            {
                throw new Exception();
            }
            int value = enumerator.Current.Value != null ? enumerator.Current.Value.GetHashCode() : 0;
            hash ^= ShiftAndWrap(key, 2) ^ value;
        }
        return hash;
    }

    private static int ShiftAndWrap(int value, int positions)
    {
        positions = positions & 0x1F;

        // Save the existing bit pattern, but interpret it as an unsigned integer. 
        uint number = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
        // Preserve the bits to be discarded. 
        uint wrapped = number >> (32 - positions);
        // Shift and wrap the discarded bits. 
        return BitConverter.ToInt32(BitConverter.GetBytes((number << positions) | wrapped), 0);
    }

    public static void Swap<T>(ref T lhs, ref T rhs) { T temp; temp = lhs; lhs = rhs; rhs = temp; }

    public static bool IsNullOrEmpty(this IList list)
    {
        if (list == null || list.Count == 0)
        {
            return true;
        }
        for (int i = 0; i < list.Count; i++)
        {
            var variable = list[i];
            if (variable != null)
            {
                return false;
            }
        }
        return true;
    }

    public static bool Overlaps<T>(this List<T> list, IEnumerable<T> other)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (other.Contains(list[i]))
            {
                return true;
            }
        }
        return false;
    }

    public static void Move(this IList list, int oldIndex, int newIndex)
    {
        var aux = list[newIndex];
        list[newIndex] = list[oldIndex];
        list[oldIndex] = aux;
    }

    public static KeyValuePair<T, T2> Random<T, T2>(this IDictionary<T, T2> array)
    {
        if (array.Count == 0)
        {
            return default(KeyValuePair<T, T2>);
        }

        var randIndex = UnityEngine.Random.Range(0, array.Count());
        var enumerator = array.GetEnumerator();
        var counter = 0;
        while (counter < randIndex)
        {
            enumerator.MoveNext();
            counter++;
        }

        return enumerator.Current;
    }

    public static T Random<T>(this IList<T> array)
    {
        if (array.Count == 0)
        {
            return default(T);
        }
        if (array.Count == 1)
        {
            return array[0];
        }
        return array[UnityEngine.Random.Range(0, array.Count())];
    }

    public static IOrderedEnumerable<T> Randomize<T>(this IList<T> source, int seed = 1324)
    {
        Random rnd = new Random(seed);
        return source.OrderBy((item) => rnd.Next());
    }
}