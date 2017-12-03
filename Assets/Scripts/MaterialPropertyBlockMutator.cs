using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialPropertyBlockMutator : MusicVisualisationComponent
{
    static MaterialPropertyBlock _materialPropertyBlock;

    public float Multiplier = .1f;
    public Gradient Gradient;
    
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

    protected override void ThinkInternal(float strenghth, float time, MusicState currentState)
    {
        _currentTime = (_currentTime + Time.deltaTime) * Value.GetValue() * Multiplier;
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