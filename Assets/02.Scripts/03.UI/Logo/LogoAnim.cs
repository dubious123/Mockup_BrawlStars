using System;
using System.Collections.Generic;

using MEC;

using Unity.VectorGraphics;

using UnityEngine;

public class LogoAnim : MonoBehaviour, IUIAnim
{
	[SerializeField] private SVGImage _logoImage;
	[SerializeField] private SVGImage _logoGlow;
	[SerializeField] private float _fadeOutTime;
	[SerializeField] private float _glowStartTime;
	[SerializeField] private float _glowFadeInTime;
	[SerializeField] private float _logoFadeInTime;
	[SerializeField] private float _logoImageStartScale;
	[SerializeField] private float _logoStartBirghtness;
	[SerializeField] private AudioClip _supercellJingle;
	public float TotalTime => _fadeOutTime + _glowFadeInTime + _logoFadeInTime;
	public float LogoBrightness { get => _logoMat.GetFloat("_Brightness"); set => _logoMat.SetFloat("_Brightness", value); }
	private Material _logoMat;

	private void Awake()
	{
		_logoMat = _logoGlow.material;
		Reset();
	}

	public void Reset()
	{
		_logoImage.rectTransform.localScale = Vector3.one * _logoImageStartScale;
		_logoImage.ChangeAlpha(0);
		_logoGlow.ChangeAlpha(0);
		LogoBrightness = _logoStartBirghtness;
	}

	public void PlayAnim(Action callback = null)
	{
		Timing.RunCoroutine(CoPlayLogo());
		Timing.CallDelayed(_glowStartTime, () => Audio.PlayOnce(_supercellJingle));
		Timing.CallDelayed(_glowStartTime, () => Timing.RunCoroutine(CoPlayGlow()));

		IEnumerator<float> CoPlayLogo()
		{
			var startSize = _logoImageStartScale * Vector3.one;
			for (float delta = 0f; delta < _fadeOutTime; delta += Time.deltaTime)
			{
				var t = Mathf.Pow(delta / _fadeOutTime, 2);
				_logoImage.transform.localScale = Vector3.Lerp(startSize, Vector3.one, t);
				_logoImage.ChangeAlpha(t);
				yield return 0f;
			}

			yield return Timing.WaitForSeconds(_glowFadeInTime);

			for (float delta = 0f; delta < _logoFadeInTime; delta += Time.deltaTime)
			{
				_logoImage.ChangeAlpha(Mathf.Lerp(1, 0, delta / _logoFadeInTime));
				yield return 0f;
			}

			callback?.Invoke();
			yield break;
		}

		IEnumerator<float> CoPlayGlow()
		{
			yield return Timing.WaitForSeconds(_glowStartTime);
			for (float delta = _glowStartTime; delta < _fadeOutTime; delta += Time.deltaTime)
			{
				var t = Mathf.Pow(delta / _fadeOutTime, 2);
				_logoGlow.ChangeAlpha(t);
				yield return 0f;
			}

			for (float delta = 0f; delta < _glowFadeInTime; delta += Time.deltaTime)
			{
				var t = delta / _glowFadeInTime;
				LogoBrightness = Mathf.Lerp(_logoStartBirghtness, 0, t);
				_logoGlow.ChangeAlpha(1 - t);
				yield return 0f;
			}

			yield break;
		}
	}
}
