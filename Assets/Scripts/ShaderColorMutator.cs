using System.Reflection.Emit;
using UnityEngine;

public class ShaderColorMutator : MusicVisualisationComponent
{
    public float HueShift;
    public float ValueShift;
    public float SaturationShift;

    [Range(0, 2)]
    public int Index;

    public Color Color1, Color2;

    private Renderer _renderer;
    private TrailRenderer3D _trailRenderer;
    private static MaterialPropertyBlock _block;

    protected override void Awake()
    {
        base.Awake();
        _renderer = GetComponent<Renderer>();
        _trailRenderer = GetComponent<TrailRenderer3D>();
    }

    protected override void ThinkInternal(float time, MusicVisualisation musicVisualisation)
    {
        float h, s, v;
        var templateColor = musicVisualisation.ColorTemplateManager.GetColor(0);
        Color.RGBToHSV(templateColor, out h, out s, out v);
        Color1 = Color.HSVToRGB(Mathfx.Frac(h + HueShift), Mathfx.Frac(s + SaturationShift), Mathfx.Frac(v + ValueShift));

        templateColor = musicVisualisation.ColorTemplateManager.GetColor(1);
        Color.RGBToHSV(templateColor, out h, out s, out v);
        Color2 = Color.HSVToRGB(Mathfx.Frac(h + HueShift), Mathfx.Frac(s + SaturationShift), Mathfx.Frac(v + ValueShift));
    }
    
    private void OnWillRenderObject()
    {
        if (_block == null)
        {
            _block = new MaterialPropertyBlock();
        }
        _block.Clear();

        _block.SetColor("_Color1", Color1);
        _block.SetColor("_Color2", Color2);

        if (_renderer)
        {
            _renderer.SetPropertyBlock(_block);
        }
        if (_trailRenderer)
        {
            _trailRenderer.SetPropertyBlock(_block);
        }
    }
}