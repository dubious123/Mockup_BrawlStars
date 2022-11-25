using System;
using System.Collections;
using System.Collections.Generic;

using MEC;

using Unity.VectorGraphics;
using Unity.VisualScripting;

using UnityEngine;

public class UIWelcomeAnimBrawl : MonoBehaviour, IUIAnim
{
	[SerializeField] private float _expandTime;
	[SerializeField] private float _holdTime;
	[SerializeField] private float _shrinkTime;
	[SerializeField] private SVGImage _svgImage;

	private RectTransform _imageRect;

	public void Reset()
	{
		_imageRect = _svgImage.GetComponent<RectTransform>();
		_imageRect.localScale = Vector3.zero;
		_svgImage.ChangeAlpha(1);
	}

	public void PlayAnim(Action callback = null)
	{
		Timing.RunCoroutine(CoPlay(callback));
	}

	private IEnumerator<float> CoPlay(Action callback = null)
	{
		for (float delta = 0f; delta < _expandTime; delta += Time.deltaTime)
		{
			_imageRect.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, delta / _expandTime);
			yield return 0f;
		}

		_imageRect.localScale = Vector3.one;
		for (float delta = 0f; delta < _holdTime; delta += Time.deltaTime)
		{
			yield return 0f;
		}

		for (float delta = 0f; delta < _shrinkTime; delta += Time.deltaTime)
		{
			var lerp = Mathf.Lerp(1, 0, delta / _shrinkTime);
			_imageRect.localScale = new Vector3(lerp, lerp, lerp);
			_svgImage.ChangeAlpha(lerp);
			yield return 0f;
		}

		_imageRect.localScale = Vector3.zero;
		gameObject.SetActive(false);
		callback?.Invoke();
		yield break;
	}
}
