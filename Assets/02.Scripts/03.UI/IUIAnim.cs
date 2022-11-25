using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public interface IUIAnim
{
	public void PlayAnim(Action callback = null);
	public void Reset();
}
