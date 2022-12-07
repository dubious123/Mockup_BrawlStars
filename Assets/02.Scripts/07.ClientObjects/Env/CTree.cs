using System.Collections;
using System.Collections.Generic;

using MEC;

using UnityEngine;

public class CTree : MonoBehaviour
{
	[SerializeField] private CEnvFade _effect;
	[SerializeField] private Transform _target;
	[SerializeField] private float _shakePeriod, _shakeAmplitude;
	[SerializeField] private int _shakeTime;

	public void FadeIn() => _effect.FadeIn();
	public void FadeOut() => _effect.FadeOut();

	public void Shake()
	{
		Timing.RunCoroutine(CoShake());
	}

	private IEnumerator<float> CoShake()
	{
		for (int i = 0; i < _shakeTime; i++)
		{
			for (float delta = 0f; delta < _shakePeriod; delta += Time.deltaTime)
			{
				var t = delta / _shakePeriod - 1;
				var rotationZ = Mathf.Sin(2 * Mathf.PI * (Mathf.Pow(t, 3) + 1)) * _shakeAmplitude;
				Debug.Log(Mathf.Sin(2 * Mathf.PI * (Mathf.Pow(t, 3) + 1)));
				_target.rotation = Quaternion.Euler(0, 0, rotationZ);
				yield return 0f;
			}
		}

		_target.rotation = Quaternion.identity;
		yield break;
	}
}
