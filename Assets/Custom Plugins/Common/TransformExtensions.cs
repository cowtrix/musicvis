using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MadMaps.Common
{
    public static class TransformExtensions
    {
        
        public static int GetHierarchyIndex(this Transform transform)
        {
            int index = 0;
            MarchUpHierarchyForIndex(transform, ref index);
            return index;
        }

        private static void MarchUpHierarchyForIndex(Transform transform, ref int index)
        {
            var siblingIndex = transform.GetSiblingIndex();
            index += siblingIndex + 1;
            if(transform.parent)
            {
                for(var i = 0; i < siblingIndex; ++i)
                {
                    var child = transform.parent.GetChild(i);
                    index += child.GetDeepChildCount();                    
                }
                MarchUpHierarchyForIndex(transform.parent, ref index);
            }
            else
            {
                var rootObjs = SceneManager.GetActiveScene().GetRootGameObjects();
                for(var i = 0; i < siblingIndex; ++i)
                {
                    var child = rootObjs[i].transform;
                    index += child.GetDeepChildCount();
                }
            }
        }

        public static int GetDeepChildCount(this Transform transform)
        {
            int count = 0;
            GetDeepChildCountRecursive(transform, ref count);
            return count;
        }

        private static void GetDeepChildCountRecursive(Transform transform, ref int count)
        {
            foreach(Transform child in transform)
            {
                count++;
                GetDeepChildCountRecursive(child, ref count);
            }
        }

        public static int GetHierarchyDepth(this Transform transform)
        {
            int depth = 0;
            MarchUpHierarchy(transform, ref depth);
            return depth;
        }

        private static void MarchUpHierarchy(Transform t, ref int count)
        {
            if (t.parent == null)
            {
                return;
            }
            count++;
            MarchUpHierarchy(t.parent, ref count);
        }

        //Breadth-first search
        public static Transform FindDeepChild(this Transform aParent, string aName)
        {
            if (aName == aParent.name)
            {
                return aParent;
            }
            var result = aParent.Find(aName);
            if (result != null)
                return result;
            foreach (Transform child in aParent)
            {
                result = child.FindDeepChild(aName);
                if (result != null)
                    return result;
            }
            return null;
        }

        public static void ApplyTRSMatrix(this Transform transform, Matrix4x4 matrix)
        {
            transform.localScale = matrix.GetScale();
            transform.rotation = matrix.GetRotation();
            transform.position = matrix.GetPosition();
        }

        public static void ApplyLocalTRSMatrix(this Transform transform, Matrix4x4 matrix)
        {
            transform.localScale = matrix.GetScale();
            transform.localRotation = matrix.GetRotation();
            transform.localPosition = matrix.GetPosition();
        }

        public static Matrix4x4 GetGlobalTRS(this Transform transform)
        {
            return Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        }

        public static Matrix4x4 GetLocalTRS(this Transform transform)
        {
            return Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
        }

        public static Quaternion GetRotation(this Matrix4x4 m)
        {
            return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
        }

        public static Vector3 GetPosition(this Matrix4x4 matrix)
        {
            var x = matrix.m03;
            var y = matrix.m13;
            var z = matrix.m23;

            return new Vector3(x, y, z);
        }

        public static Vector3 GetScale(this Matrix4x4 m)
        {
            return new Vector3(m.GetColumn(0).magnitude,
                                m.GetColumn(1).magnitude,
                                m.GetColumn(2).magnitude);
        }

        public static void SetLayerRecursive(this Transform t, int layer)
        {
            t.gameObject.layer = layer;
            foreach (Transform child in t)
            {
                child.SetLayerRecursive(layer);
            }
        }
    }
}