using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ColorTemplateManager : MonoBehaviour
{
    [Serializable]
    public struct Template
    {
        public Color Color0;
        public Color Color1;
        public Color Color2;

        public Color GetByIndex(int index)
        {
            switch (index)
            {
                case 0:
                return Color0;
                case 1:
                return Color1;
                case 2:
                return Color2;
                default:
                break;
            }
            return Color.cyan;
        }
    }

    public List<Template> Templates = new List<Template>();
    public float TimePerTemplate = 60;
    public float Speed = 1;
    public bool Exponential;
    public float Time;

    [ContextMenu("AutoFill Random")]
    public void AutoFillRandom()
    {
        Templates.Clear();
        for(var i = 0; i < 60; ++i)
        {
            var rootHue = Random.value;
            Templates.Add(new Template()
            {
                Color0 = Color.HSVToRGB(rootHue, Random.Range(.7f, 1f), Random.Range(.9f, 1f)),
                Color1 = Color.HSVToRGB(Mathfx.Frac(rootHue + Random.Range(1/6f, 1/4f)), Random.Range(.7f, 1f), Random.Range(.9f, 1f)),
                Color2 = Color.HSVToRGB(Mathfx.Frac(rootHue - Random.Range(1/6f, 1/4f)), Random.Range(.7f, 1f), Random.Range(.7f, 1f)),
            });
        }
    }

    public void AddToTime(float value)
    {
        Time += value;
    }

    public Template GetTemplateAtTime(float tOffset = 0)
    {
        float totalTime = TimePerTemplate * Templates.Count;
        float tFrac = (Time + tOffset) % totalTime;

        float tIndex = (tFrac / totalTime) * (Templates.Count - 1);
        int tBase = Mathf.FloorToInt(tIndex);
        int tNext = Mathf.CeilToInt(tIndex);

        if(tNext >= Templates.Count)
            tNext = 0;

        float tF = tIndex - tBase;

        return new Template()
        {
            Color0 = Color.Lerp(Templates[tBase].Color0, Templates[tNext].Color0, tF),
            Color1 = Color.Lerp(Templates[tBase].Color1, Templates[tNext].Color1, tF),
            Color2 = Color.Lerp(Templates[tBase].Color2, Templates[tNext].Color2, tF),
        };
    }
}