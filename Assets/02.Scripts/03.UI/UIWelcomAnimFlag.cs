using System;
using System.Collections;
using System.Collections.Generic;

using MEC;

using UnityEngine;
using UnityEngine.UI;

public class UIWelcomAnimFlag : MonoBehaviour, IUIAnim
{
	[SerializeField] private Image _profileImage0;
	[SerializeField] private Image _profileImage2;
	[SerializeField] private Image _profileImage3;
	[SerializeField] private float _startPos;
	[SerializeField] private float _firstMoveDuration;
	[SerializeField] private float _firstTargetPos;
	[SerializeField] private float _secondMoveDiration;
	[SerializeField] private float _secondTargetPos;
	[SerializeField] private float _thirdMoveDuration;
	[SerializeField] private float _thirdTargetPos;

	private RectTransform _rect;

	public void Reset()
	{
		_rect = GetComponent<RectTransform>();
		_rect.SetAnchorX(_startPos);
	}

	public void PlayAnim(Action callback = null)
	{
		Timing.RunCoroutine(CoPlay(callback));
	}

	private IEnumerator<float> CoPlay(Action callback = null)
	{
		for (float delta = 0f; delta < _firstMoveDuration; delta += Time.deltaTime)
		{
			_rect.SetAnchorX(Mathf.Lerp(_startPos, _firstTargetPos, delta / _firstMoveDuration));
			yield return 0f;
		}

		for (float delta = 0f; delta < _secondMoveDiration; delta += Time.deltaTime)
		{
			_rect.SetAnchorX(Mathf.Lerp(_firstTargetPos, _secondTargetPos, delta / _secondMoveDiration));
			yield return 0f;
		}

		for (float delta = 0f; delta < _thirdMoveDuration; delta += Time.deltaTime)
		{
			_rect.SetAnchorX(Mathf.Lerp(_secondTargetPos, _thirdTargetPos, delta / _thirdMoveDuration));
			yield return 0f;
		}

		callback?.Invoke();
		yield break;
	}
}
