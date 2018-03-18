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
    [Range(0, 1)]
    public float Weight = 1;
    public float TimeOffset = 0;
    public ESampleMode SampleMode;
    public int AveragingSteps = 10;
    public bool Exponential;

    public AveragedValue Value { get; protected set; }
    public AnimationCurve WavelengthTargets = AnimationCurve.Linear(0, 1, 1, 1);

    protected virtual void Awake()
    {
        Value = new AveragedValue(AveragingSteps);
    }

    protected static float GetSumUnderWavelength(AnimationCurve curve, MusicState state)
    {
        float sum = 0;
        for (var i = 0; i < state.Wavelength.Length; i++)
        {
            var val = state.Wavelength[i];
            var t = i / (float) (state.Wavelength.Length - 1);
            val *= curve.Evaluate(t);
            sum += val;
        }
        return sum;
    }

    public void Think(MusicVisualisation musicVisualisation)
    {
        Value.Size = AveragingSteps;
        float val = 0;
        switch (SampleMode)
        {
            case ESampleMode.RMS:
                val = musicVisualisation.CurrentState.RMS;
                break;
            case ESampleMode.Peak:
                val = musicVisualisation.CurrentState.Peak;
                break;
            case ESampleMode.Wavelength:
                val = GetSumUnderWavelength(WavelengthTargets, musicVisualisation.CurrentState);
                break;
            case ESampleMode.Time:
                val = musicVisualisation.Time;
                break;
            case ESampleMode.DeltaTime:
                val = Time.deltaTime;
                break;
        }
        if (Exponential)
        {
            val *= val;
        }
        Value.Add(val);
        Value.Multiplier = musicVisualisation.Strength * Weight;

        var time = musicVisualisation.Time + TimeOffset;

        ThinkInternal(time, musicVisualisation);
    }

    protected abstract void ThinkInternal(float time, MusicVisualisation musicVisualisation);
}