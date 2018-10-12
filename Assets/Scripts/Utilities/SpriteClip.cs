using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SpriteClip : ScriptableObject 
{
	public List<Texture2D> Frames = new List<Texture2D>();
}
