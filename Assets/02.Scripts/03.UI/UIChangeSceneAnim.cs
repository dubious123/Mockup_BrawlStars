using System.Collections;
using System.Collections.Generic;

using MEC;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class UIChangeSceneAnim : MonoBehaviour
{
	[SerializeField] private RawImage _image;
	[SerializeField] private float _duration;

	public void FadeIn()
	{
		Timing.KillCoroutines(gameObject.name);
		Timing.RunCoroutine(CoInternalFadeIn(), gameObject.name);

		IEnumerator<float> CoInternalFadeIn()
		{
			for (float delta = 0f; delta < _duration; delta += Time.deltaTime)
			{
				_image.ChangeAlpha(Mathf.Lerp(0, 1, delta / _duration));
				yield return 0f;
			}

			yield break;
		}
	}

	public void FadeOut()
	{
		Timing.KillCoroutines(gameObject.name);
		Timing.RunCoroutine(CoInternalFadeOut(), gameObject.name);

		IEnumerator<float> CoInternalFadeOut()
		{
			for (float delta = 0f; delta < _duration; delta += Time.deltaTime)
			{
				_image.ChangeAlpha(Mathf.Lerp(1, 0, delta / _duration));
				yield return 0f;
			}

			yield break;
		}
	}

}
