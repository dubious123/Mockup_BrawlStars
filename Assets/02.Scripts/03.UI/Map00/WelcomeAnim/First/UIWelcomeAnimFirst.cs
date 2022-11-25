using System;
using System.Collections;
using System.Collections.Generic;

using MEC;

using UnityEngine;

public class UIWelcomeAnimFirst : MonoBehaviour, IUIAnim
{
	[SerializeField] private float _holdTime;
	[SerializeField] private float _shrinkTime;

	private RectTransform _rect;

	public void Reset()
	{
		_rect = GetComponent<RectTransform>();
		_rect.localScale = Vector3.one;
	}

	public void PlayAnim(Action callback = null)
	{
		Timing.RunCoroutine(CoPlay(callback));
	}

	public IEnumerator<float> CoPlay(Action callback)
	{
		yield return Timing.WaitForSeconds(_holdTime);

		for (float delta = 0f; delta < _shrinkTime; delta += Time.deltaTime)
		{
			_rect.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, Mathf.Pow(delta / _shrinkTime, 4));
			yield return 0f;
		}

		gameObject.SetActive(false);

		callback?.Invoke();
		yield break;
	}
}
