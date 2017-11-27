using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialPropertyBlockMutator : MusicVisualisationComponent
{
    static MaterialPropertyBlock _materialPropertyBlock;

    public float Multiplier = .1f;
    public Gradient Gradient;
    public ESampleMode Mode = ESampleMode.RMS;
    [Range(0, 1)]
    public float Wavelength = 0;
    
    private Renderer _renderer;
    private Color _currentColor;
    private float _currentTime;

    private void Awake()
    {
        if (_materialPropertyBlock == null)
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
        }
        _renderer = GetComponent<Renderer>();
    }

    public override void Think(float strenghth, float time, MusicState currentState)
    {
        float val = 0;
        switch (Mode)
        {
            case ESampleMode.RMS:
                val = currentState.RMS;
                break;
            case ESampleMode.Peak:
                val = currentState.Peak;
                break;
            case ESampleMode.Wavelength:
                var index = Mathf.FloorToInt(Wavelength * currentState.Wavelength.Length - 1);
                val = currentState.Wavelength[index];
                break;

        }
        Value = val;
        _currentTime = (_currentTime + Time.deltaTime) * Value * Multiplier;
        _currentTime = _currentTime.Frac();
        _currentColor = Gradient.Evaluate(_currentTime);
    }

    private void OnWillRenderObject()
    {
        _materialPropertyBlock.Clear();
        _materialPropertyBlock.SetColor("_Color", _currentColor);
        _renderer.SetPropertyBlock(_materialPropertyBlock);
    }
}