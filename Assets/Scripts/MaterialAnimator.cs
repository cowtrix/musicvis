using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialAnimator : MonoBehaviour 
{
    public SpriteClip CurrentClip;
    public SpriteClip NextClip;
    public Material Material;
    public string VariableName = "_MainTex";
    public float FPS = 24;

    int _currentFrame;
    float _accum;

    private void Update()
    {
        _accum += Time.deltaTime;
        var step = 1f/(float)FPS;
        while(_accum > step)
        {
            _accum -= step;
            _currentFrame++;
        }

        if(_currentFrame >= CurrentClip.Frames.Count)
        {
            if(!NextClip)
            {
                NextClip = CurrentClip;
            }
            CurrentClip = NextClip;
            _currentFrame = 0;
        }

        Material.SetTexture(VariableName, CurrentClip.Frames[_currentFrame]);
    }
}
