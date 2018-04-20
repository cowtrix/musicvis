using System;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxColorComponent : MonoBehaviour
{
    public ColorTemplateManager TemplateManager;

    void Update()
    {
        var template = TemplateManager.GetTemplateAtTime();
        RenderSettings.ambientGroundColor = template.Color0;
        RenderSettings.ambientEquatorColor = template.Color1;
        RenderSettings.ambientSkyColor = template.Color2;
    }
}