using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Animator_Tool
{
	public static AnimationClip GetAnimationClipOrNull(RuntimeAnimatorController anim, string name)
	{
		foreach (var clip in anim.animationClips)
		{
			if (clip.name == name)
				return clip;
		}
		return null;
	}
}
