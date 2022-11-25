using System;
using System.Collections;
using System.Collections.Generic;

using MEC;

using Unity.VectorGraphics;

using UnityEngine;
using UnityEngine.UI;

public class UIWelcomeAnimThird : MonoBehaviour, IUIAnim
{
	[SerializeField] private UIWelcomeAnimBrawl _brawl;
	[SerializeField] private UIWelcomeAnimSpinLight _spinLight;

	public void Reset()
	{
		_brawl.Reset();
		_spinLight.Reset();
	}

	public void PlayAnim(Action callback = null)
	{
		_spinLight.gameObject.SetActive(true);
		_spinLight.PlayAnim();
		_brawl.gameObject.SetActive(true);
		_brawl.PlayAnim(callback);
	}
}
