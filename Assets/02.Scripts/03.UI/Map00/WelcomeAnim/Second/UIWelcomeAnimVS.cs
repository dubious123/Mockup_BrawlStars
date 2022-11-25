using System;
using System.Collections;
using System.Collections.Generic;

using MEC;

using UnityEngine;
using UnityEngine.UI;

public class UIWelcomeAnimVS : MonoBehaviour, IUIAnim
{
	[SerializeField] private float _expandTime;
	[SerializeField] private float _holdTime;
	[SerializeField] private float _shrinkTime;

	public void Reset()
	{
		GetComponent<RectTransform>().localScale = Vector3.one;
	}

	public void PlayAnim(Action callback = null)
	{
		Timing.RunCoroutine(CoPlay(callback));
	}

	public IEnumerator<float> CoPlay(Action callback = null)
	{
		var rect = GetComponent<RectTransform>();
		rect.localScale = Vector3.zero;
		for (float delta = 0f; delta < _expandTime; delta += Time.deltaTime)
		{
			rect.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, Mathf.Pow(delta / _expandTime, 4));
			yield return 0f;
		}

		for (float delta = 0f; delta < _holdTime; delta += Time.deltaTime)
		{
			yield return 0f;
		}

		for (float delta = 0f; delta < _shrinkTime; delta += Time.deltaTime)
		{
			rect.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, Mathf.Pow(delta / _shrinkTime, 4));
			yield return 0f;
		}

		callback?.Invoke();
		gameObject.SetActive(false);
		yield break;
	}
}
