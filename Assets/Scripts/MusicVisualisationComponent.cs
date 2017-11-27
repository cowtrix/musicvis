using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MusicVisualisationComponent), true)]
public class MusicVisualisationComponentGUI : Editor
{
    private float _max = float.MinValue, _min = float.MaxValue;
    private SerializedProperty _waveLength, _mode;

    public override void OnInspectorGUI()
    {
        var msc = target as MusicVisualisationComponent;

        EditorGUILayout.LabelField("");
        if (msc.Value > _max)
        {
            _max = msc.Value;
        }
        if (msc.Value < _min)
        {
            _min = msc.Value;
        }
        var nv = (msc.Value - _min) / Mathf.Max(_max - _min, float.Epsilon);
        var r = GUILayoutUtility.GetLastRect();
        EditorGUI.ProgressBar(r, nv, "Value");

        DrawDefaultInspector();
    }
}

#endif

public abstract class MusicVisualisationComponent : MonoBehaviour
{
    public float Value { get; protected set; }

    public abstract void Think(float strenghth, float time, MusicState currentState);

    protected static float GetValFromSampleMode(ESampleMode mode, MusicState currentState, float wavelength = 0)
    {
        float val = 0;
        switch (mode)
        {
            case ESampleMode.RMS:
                val = currentState.RMS;
                break;
            case ESampleMode.Peak:
                val = currentState.Peak;
                break;
            case ESampleMode.Wavelength:
                var index = Mathf.FloorToInt(wavelength * (currentState.Wavelength.Length - 1));
                val = currentState.Wavelength[index];
                break;
        }
        return val;
    }
}