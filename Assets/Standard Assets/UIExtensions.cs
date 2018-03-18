using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class UIExtensions
{
    private static Vector3[] _worldRectCache = new Vector3[4];
    public static Rect GetWorldRect(this RectTransform transform)
    {
        transform.GetWorldCorners(_worldRectCache);

        /*for (int i = 0; i < _worldRectCache.Length; i++)
        {
            var vector3 = _worldRectCache[i];
            UIDebug.Point("worldpoint"+i, vector3, Color.blue, 10);
        }*/

        var rect = new Rect(_worldRectCache[0], Vector2.zero);
        rect = RectExtensions.Encapsulate(rect, _worldRectCache[2]);
        rect = RectExtensions.Encapsulate(rect, _worldRectCache[1]);
        rect = RectExtensions.Encapsulate(rect, _worldRectCache[3]);
        return rect;
    }

    private static PointerEventData _tempPointerData;
    public static void RaycastAll(this EventSystem eventSystem, Vector2 position, List<RaycastResult> result)
    {
        if (_tempPointerData == null)
        {
            _tempPointerData = new PointerEventData(eventSystem);
        }
        _tempPointerData.position = position;
        eventSystem.RaycastAll(_tempPointerData, result);
    }

    public static void SetValue(this Slider slider, float value, float epsilon = 0.01f)
    {
        if (Math.Abs(slider.value - value) < epsilon)
        {
            return;
        }
        slider.value = value;
    }
}