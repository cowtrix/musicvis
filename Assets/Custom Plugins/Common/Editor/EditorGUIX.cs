#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MadMaps.Common
{
    public static class EditorGUIX
    {
        [MenuItem("CONTEXT/Transform/Log Hierarchy Index")]
        public static void LogHierarchyIndex(MenuCommand command)
        {
            Debug.Log((command.context as Transform).GetHierarchyIndex());
        }

        [MenuItem("CONTEXT/Transform/Log Hierarchy Depth")]
        public static void LogDepth(MenuCommand command)
        {
            Debug.Log((command.context as Transform).GetDeepChildCount());
        }

        public static void PropertyField(Rect labelRect, Rect propertyRect, SerializedProperty property)
        {
            EditorGUI.LabelField(labelRect, property.displayName);
            EditorGUI.PropertyField(propertyRect, property, GUIContent.none);
        }
    }
}

#endif