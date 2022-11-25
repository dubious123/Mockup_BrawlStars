using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class UIWelcomeAnim : MonoBehaviour, IUIAnim
{
	[SerializeField] private UIWelcomeAnimFirst _firstAnim;
	[SerializeField] private UIWelcomeAnimSecond _secondAnim;
	[SerializeField] private UIWelcomeAnimThird _thirdAnim;

	public void Reset()
	{
		_firstAnim.gameObject.SetActive(true);
		_firstAnim.Reset();
		_secondAnim.Reset();
		_thirdAnim.Reset();
	}

	public void PlayAnim(Action callback = null)
	{
		_firstAnim.gameObject.SetActive(true);
		_firstAnim.PlayAnim(() =>
		{
			_secondAnim.gameObject.SetActive(true);
			_secondAnim.PlayAnim(() =>
			{
				_thirdAnim.gameObject.SetActive(true);
				_thirdAnim.PlayAnim(callback);
			});
		});
	}
}
