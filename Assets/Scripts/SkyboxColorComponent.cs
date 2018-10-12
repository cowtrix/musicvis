using System;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxColorComponent : MonoBehaviour
{
    public ColorTemplateManager TemplateManager;
    [Range(0, 2)]
    public int ColorIndex1, ColorIndex2, ColorIndex3;
    public ColorMutator ColorMutator1, ColorMutator2, ColorMutator3;

    void Update()
    {
        var template = TemplateManager.GetTemplateAtTime();
        RenderSettings.ambientGroundColor = ColorMutator1.Mutate(template.GetByIndex(ColorIndex1));
        RenderSettings.ambientEquatorColor = ColorMutator2.Mutate(template.GetByIndex(ColorIndex2));
        RenderSettings.ambientSkyColor = ColorMutator3.Mutate(template.GetByIndex(ColorIndex3));
    }
}