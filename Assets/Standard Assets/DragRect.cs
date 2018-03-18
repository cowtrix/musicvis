using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DragRect : MonoBehaviour, IBeginDragHandler, IDragHandler, IScrollHandler
{
    private Vector2 _lastDragPos;
    public bool ClampToScreen = true;
    public UnityEvent OnDragCallback;
    public RectTransform Viewport;
    public Vector2 ZoomLimit = new Vector2(1, 2);
    public float ZoomSpeed = .1f;
    public float ChaseSpeed = 5f;
    public AnimationCurve ZoomCurve = AnimationCurve.Linear(0, 1, 1, 1);
    private float _zoom = float.MaxValue; // 0 is fully zoomed in, 1 is fully zoomed out
    private float _targetScale;
    private Vector2 _targetPivot;
    private Vector2 _targetPosition;

    private Rect _lastTargetRect;
    public RectTransform RectTransform
    {
        get
        {
            if (!__rectTransform)
            {
                __rectTransform = GetComponent<RectTransform>();
            }
            return __rectTransform;
        }
    }
    private RectTransform __rectTransform;
    private bool _instant = false;
    private float _lastScroll;

    public void OnEnable()
    {
        _zoom = 0;
        _targetScale = ZoomToScale(_zoom);

        _targetPivot = RectTransform.pivot;
        _targetPosition = RectTransform.position;
        _instant = true;
    }

    public void Update()
    {
        VerifyPosition();
        if (_instant)
        {
            _instant = false;
            RectTransform.position = _targetPosition;
            RectTransform.pivot = _targetPivot;
            RectTransform.localScale = Vector3.one * (1 / _targetScale);
        }
        else
        {
            var dt = Time.deltaTime;
            RectTransform.position = Vector2.Lerp(RectTransform.position, _targetPosition, ChaseSpeed * dt);
            RectTransform.pivot = _targetPivot;
            RectTransform.localScale = Vector3.one * (1 / Mathf.Lerp(1 / RectTransform.localScale.x, _targetScale, ChaseSpeed * dt));
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _lastDragPos = eventData.pressPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var delta = eventData.position - _lastDragPos;
        _lastDragPos = eventData.position;
        _targetPosition += delta;
        if (OnDragCallback != null)
        {
            OnDragCallback.Invoke();
        }
        VerifyPosition();
    }

    public void OnScroll(PointerEventData eventData)
    {
        var t = Time.time;
        var zoomDelta = Mathf.Sign(eventData.scrollDelta.y) * ZoomSpeed * ZoomCurve.Evaluate(_zoom) * -1;
        var newZoom = Mathf.Clamp01(_zoom + zoomDelta);
        if (Mathf.Approximately(newZoom, _zoom))
        {
            if (t > _lastScroll + .5f && Mathf.Approximately(_zoom, 1) && zoomDelta > 0)
            {
                SetTarget(Viewport.GetWorldRect().center, new Vector2(.5f, .5f), _targetScale, false);
            }
            return;
        }
        _lastScroll = t;

        var mapRect = RectTransform.GetWorldRect();
        var viewPos = eventData.position - mapRect.min;
        var newPivot = new Vector2(viewPos.x / mapRect.width, viewPos.y / mapRect.height);
        var mousePos = eventData.position;

        _zoom = newZoom;
        _targetScale = ZoomToScale(_zoom);
        _targetPivot = newPivot;

        var pivotDelta = newPivot - _targetPivot;
        var scaledPivotDelta = new Vector2(pivotDelta.x * _lastTargetRect.width, pivotDelta.y * _lastTargetRect.height);
        _targetPosition = mousePos + scaledPivotDelta;
        RectTransform.position = _targetPosition;

        VerifyPosition();
    }


    public void VerifyPosition()
    {
        if (!ClampToScreen)
        {
            return;
        }
        if (!Viewport)
        {
            Debug.LogWarning("DragRect Viewport was missing!", this);
            return;
        }

        var viewRect = Viewport.GetWorldRect();
        var contentRect = RectTransform.GetWorldRect();
        var rawSize = new Vector2(contentRect.width * (1 / RectTransform.localScale.x),
            contentRect.height * (1 / RectTransform.localScale.y));

        var targetSize = rawSize * (1 / _targetScale);
        var min = _targetPosition - new Vector2(targetSize.x * _targetPivot.x, targetSize.x * _targetPivot.y);
        var targetRect = new Rect(min, targetSize);

        if (viewRect.height < viewRect.width)
        {
            if (targetRect.height < viewRect.height && targetRect.height < rawSize.y)
            {
                var delta = viewRect.height - targetRect.height;
                var deltaScale = 1 + (delta / targetRect.height);

                var maxScale = ZoomLimit.y;
                var bestScale = rawSize.y / viewRect.height;
                bestScale = Mathf.Min(maxScale, bestScale);
                var newScale = _targetScale * deltaScale;

                var newScaleActual = Mathf.Min(bestScale, newScale);
                _targetScale = newScaleActual;
                _zoom = ScaleToZoom(_targetScale);
            }
        }
        else if (viewRect.width < viewRect.height)
        {
            Debug.LogWarning("DragRect not implemented for this resolution case!");
        }

        // X clamp
        {
            float xMin = viewRect.center.x - targetRect.width;
            float xMax = viewRect.center.x + targetRect.width;
            if (targetRect.xMin < xMin)
            {
                var xDelta = xMin - targetRect.xMin;
                _targetPosition.x += xDelta;
            }
            if (targetRect.xMax > xMax)
            {
                var xDelta = -(targetRect.xMax - xMax);
                _targetPosition.x += xDelta;
            }
        }

        // Y clamp
        {
            float yMin = viewRect.center.y - targetRect.height;
            float yMax = viewRect.center.y + targetRect.height;
            if (targetRect.yMin < yMin)
            {
                var yDelta = yMin - targetRect.yMin;
                _targetPosition.y += yDelta;
            }
            if (targetRect.yMax > yMax)
            {
                var yDelta = -(targetRect.yMax - yMax);
                _targetPosition.y += yDelta;
            }
        }
        _lastTargetRect = targetRect;
    }

    public void SetScale(float scale)
    {
        scale = Mathf.Clamp(scale, ZoomLimit.x, ZoomLimit.y);
        if (Mathf.Approximately(1 / RectTransform.localScale.x, scale))
        {
            return;
        }
        _zoom = ScaleToZoom(scale);
        _targetScale = scale;
    }

    float ZoomToScale(float zoom)
    {
        return ZoomLimit.x + zoom * (ZoomLimit.y - ZoomLimit.x);
    }

    float ScaleToZoom(float scale)
    {
        scale -= ZoomLimit.x;
        scale /= ZoomLimit.y - ZoomLimit.x;
        return scale;
    }
    
    public void SetTarget(Vector2 position, Vector2 pivot, float scale, bool instant)
    {
        _instant = instant;

        SetScale(scale);
        var pivotDelta = pivot - _targetPivot;
        var scaledPivotDelta = new Vector2(pivotDelta.x * _lastTargetRect.width, pivotDelta.y * _lastTargetRect.height);


        _targetPosition = position;
        _targetPivot = pivot;

        RectTransform.position += scaledPivotDelta.xy0();
        RectTransform.pivot = _targetPivot;
    }
}