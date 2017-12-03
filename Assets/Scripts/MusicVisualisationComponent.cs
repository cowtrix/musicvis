using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MusicVisualisationComponent), true)]
[CanEditMultipleObjects]
public class MusicVisualisationComponentGUI : Editor
{
    private float _max = float.MinValue, _min = float.MaxValue;
    public override void OnInspectorGUI()
    {
        if (Application.isPlaying)
        {
            var msc = target as MusicVisualisationComponent;
            EditorGUILayout.LabelField("");
            var val = msc.Value.GetValue();
            if (val > _max)
            {
                _max = val;
            }
            if (val < _min)
            {
                _min = val;
            }
            var nv = (val - _min) / Mathf.Max(_max - _min, float.Epsilon);
            var r = GUILayoutUtility.GetLastRect();
            EditorGUI.ProgressBar(r, nv, "Value");
        }
        DrawDefaultInspector();
    }
}

#endif

public abstract class MusicVisualisationComponent : MonoBehaviour
{
    public float TimeOffset = 0;
    public ESampleMode SampleMode;
    public int AveragingSteps = 10;
    public AveragedValue Value { get; protected set; }
    [Range(0, 1)]
    public float TargetWavelength;

    private void Awake()
    {
        Value = new AveragedValue(AveragingSteps);
    }

    public void Think(float strength, float time, MusicState currentState)
    {
        float val = 0;
        switch (SampleMode)
        {
            case ESampleMode.RMS:
                val = currentState.RMS;
                break;
            case ESampleMode.Peak:
                val = currentState.Peak;
                break;
            case ESampleMode.Wavelength:
                var index = Mathf.FloorToInt(TargetWavelength * (currentState.Wavelength.Length - 1));
                val = currentState.Wavelength[index];
                break;
            case ESampleMode.Time:
                val = time;
                break;
            case ESampleMode.DeltaTime:
                val = Time.deltaTime;
                break;
        }
        Value.Add(val);
        time += TimeOffset;
        ThinkInternal(strength, time, currentState);
    }

    protected abstract void ThinkInternal(float strenghth, float time, MusicState currentState);
}