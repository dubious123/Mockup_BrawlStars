using System;

using MEC;

using UnityEngine;

public class UIWelcomeAnimSecond : MonoBehaviour, IUIAnim
{
	[SerializeField] private UIWelcomeAnimVS _vs;
	[SerializeField] private UIWelcomAnimFlag _blue;
	[SerializeField] private UIWelcomAnimFlag _red;
	[SerializeField] private float _textAnimDelayTime;
	[SerializeField] private AudioClip _audioClip;

	public void Reset()
	{
		_vs.Reset();
		_blue.Reset();
		_red.Reset();
		_vs.gameObject.SetActive(false);
	}

	public void PlayAnim(Action callback = null)
	{
		Audio.PlayOnce(_audioClip);
		_blue.PlayAnim();
		_red.PlayAnim();
		Timing.CallDelayed(_textAnimDelayTime, () =>
		{
			_vs.gameObject.SetActive(true);
			_vs.PlayAnim(callback);
		});
	}
}
