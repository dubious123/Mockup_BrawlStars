using System;
using System.Collections;
using System.Collections.Generic;

using MEC;

using UnityEngine;
using UnityEngine.UI;

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
				Timing.RunCoroutine(CoInternalFade());
				_thirdAnim.gameObject.SetActive(true);
				_thirdAnim.PlayAnim(callback);
			});
		});
	}

	private IEnumerator<float> CoInternalFade()
	{
		var image = GetComponent<RawImage>();
		for (float delta = 0; delta < 1; delta += Time.deltaTime)
		{
			var alpha = Mathf.Lerp(0.5f, 0, delta);
			image.ChangeAlpha(alpha);
			yield return 0f;
		}
	}
}
