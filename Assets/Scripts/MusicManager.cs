using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MusicState
{
    public float Peak;
    public float RMS;
    public float[] Wavelength = new float[512];
}

public class MusicManager : Singleton<MusicManager>
{
    public float Silence = -40;
    public Lasp.FilterType FilterType;
    public static MusicState MusicState = new MusicState();
    public Texture2D WavelengthTexture;
    public Texture2D DeltaTexture;

    SmartValue _min, _max;

    private void Awake()
    {
        _min = new SmartValue(1000);
        _max = new SmartValue(1000);
    } 

    private void Update()
    {
        var peak = Lasp.AudioInput.GetPeakLevelDecibel(FilterType);
        var rms = Lasp.AudioInput.CalculateRMSDecibel(FilterType);
        Lasp.AudioInput.RetrieveWaveform(FilterType, MusicState.Wavelength);
        MusicState.Peak = Mathf.Clamp01(1 - peak / Silence);
        MusicState.RMS = Mathf.Clamp01(1 - rms / Silence);

        if(WavelengthTexture == null || WavelengthTexture.width != MusicState.Wavelength.Length)
        {
            WavelengthTexture = new Texture2D(2, MusicState.Wavelength.Length);            
        }
        if(DeltaTexture == null || DeltaTexture.width != MusicState.Wavelength.Length)
        {
            DeltaTexture = new Texture2D(2, MusicState.Wavelength.Length);            
        }

        var min = MusicState.Wavelength.Min();
        _min.AddValue(min);
        
        var max = MusicState.Wavelength.Max();
        _max.AddValue(max);

        var step = (max - min) / 1000f;

        for(var i = 0; i < MusicState.Wavelength.Length; ++i)
        {
            var f = (MusicState.Wavelength[i] - min) / (max - min);
            var e = WavelengthTexture.GetPixel(0, i).grayscale;
            
            var v = Mathf.MoveTowards(f, e, step);

            DeltaTexture.SetPixel(0, i, new Color(e, e, e, 1));
            DeltaTexture.SetPixel(1, i, new Color(e, e, e, 1));

            WavelengthTexture.SetPixel(0, i, new Color(f, f, f, 1));
            WavelengthTexture.SetPixel(1, i, new Color(f, f, f, 1));
        }

        WavelengthTexture.Apply();
        DeltaTexture.Apply();
        Shader.SetGlobalTexture("_Wavelength", WavelengthTexture);
        Shader.SetGlobalTexture("_Delta", WavelengthTexture);
    }
}