using System;
using System.Collections;
using System.Collections.Generic;

using MEC;

using Unity.Mathematics;

using UnityEngine;
using UnityEngine.UI;

public class UIWelcomeAnimSpinLight : MonoBehaviour, IUIAnim
{
	[SerializeField] private float _spinSpeed;
	[SerializeField] private float _fadeInTime;
	[SerializeField] private float _holdTime;
	[SerializeField] private float _fadeOutTime;
	[SerializeField] private RawImage _image;
	public void Reset()
	{
	}

	public void PlayAnim(Action callback = null)
	{
		var deltaTime = 0f;
		Timing.CallContinuously(_fadeInTime + _holdTime + _fadeOutTime, () =>
		{
			deltaTime += Timing.DeltaTime;
			_image.material.SetFloat("_Rotation", deltaTime * _spinSpeed);
		});

		Timing.RunCoroutine(CoPlay(callback));
	}

	private IEnumerator<float> CoPlay(Action callback = null)
	{
		for (float delta = 0f; delta < _fadeInTime; delta += Time.deltaTime)
		{
			_image.ChangeAlpha(delta / _fadeInTime);
			yield return 0f;
		}

		_image.ChangeAlpha(1);
		for (float delta = 0f; delta < _holdTime; delta += Time.deltaTime)
		{
			yield return 0f;
		}

		for (float delta = 0f; delta < _fadeOutTime; delta += Time.deltaTime)
		{
			_image.ChangeAlpha(Mathf.Lerp(1, 0, delta / _fadeOutTime));
			yield return 0f;
		}

		_image.ChangeAlpha(0);
		gameObject.SetActive(false);
		callback?.Invoke();
		yield break;
	}
}
