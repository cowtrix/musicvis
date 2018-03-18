using System;
using System.Collections.Generic;
using UnityEngine;

public class ColorTemplateManager : MonoBehaviour
{
    [Serializable]
    public struct Template
    {
        public Color Color0;
        public Color Color1;
        public Color Color2;
    }

    public Template OutTemplate;

    public List<Template> Templates = new List<Template>();
    public float Speed = 60;

    private int _baseIndex = 0;
    private float _time;
    
    private void Update()
    {
        if (_baseIndex > Templates.Count - 1)
        {
            _baseIndex = 0;
        }

        _time += Time.deltaTime;
        while (_time > Speed)
        {
            _time -= Speed;
            _baseIndex++;
        }

        var tF = _time / Speed;
        var baseTemplate = Templates[_baseIndex];

        var nextIndex = _baseIndex + 1;
        if (nextIndex > Templates.Count - 1)
        {
            nextIndex = 0;
        }
        var nextTemplate = Templates[nextIndex];

        OutTemplate = new Template()
        {
            Color0 = Color.Lerp(baseTemplate.Color0, nextTemplate.Color0, tF),
            Color1 = Color.Lerp(baseTemplate.Color1, nextTemplate.Color1, tF),
            Color2 = Color.Lerp(baseTemplate.Color2, nextTemplate.Color2, tF),
        };
    }
    
    public Color GetColor(int index)
    {
        switch (index)
        {
            case 0:
                return OutTemplate.Color0;
            case 1:
                return OutTemplate.Color1;
            case 2:
                return OutTemplate.Color2;
        }
        throw new Exception();
    }
}