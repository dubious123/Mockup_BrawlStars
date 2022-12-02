using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using MEC;

using OpenCover.Framework.Model;

using UnityEditor;
using UnityEditor.Networking.PlayerConnection;

using UnityEngine;

public class HudHealthTextEffect : MonoBehaviour
{
	[SerializeField] private GameObject _textPrefab;
	[SerializeField] private float _startAlpha;
	[SerializeField] private float _startSize;
	[SerializeField] private float _fadeInSize;
	[SerializeField] private float _fadeOutSize;
	[SerializeField] private float _fadeInTime;
	[SerializeField] private Vector2 _fadeInTravelDelta;
	[SerializeField] private float _holdTime;
	[SerializeField] private float _fadeOutTime;
	[SerializeField] private Vector2 _fadeOutTravelDelta1;

	[field: SerializeField] public int Damage { private get; set; }

	public void PlayEffect()
	{
		Timing.RunCoroutine(CoPlay());
	}

	public IEnumerator<float> CoPlay()
	{
		var instance = Instantiate(_textPrefab, transform);
		var rect = instance.GetComponent<RectTransform>();
		var text = _textPrefab.GetComponent<TextWithShadow>();
		rect.anchorMax = Vector2.one;
		rect.anchorMin = Vector2.zero;
		text.Text = Damage.ToString();
		var startPos = rect.anchorMin.y;
		var targetPos = startPos + _fadeInTravelDelta.y;
		for (var delta = 0f; delta < _fadeInTime; delta += Time.deltaTime)
		{
			var t = delta / _fadeInTime;
			rect.localScale = Vector3.one * Mathf.Lerp(_startSize, _fadeInSize, t);
			text.Alpha = Mathf.Lerp(_startAlpha, 1, t);
			rect.SetAnchorY(Mathf.Lerp(startPos, targetPos, t));
			Debug.Log(rect.anchorMin.y);

			Debug.Log(text.Alpha);

			yield return 0f;
		}

		for (var delta = 0f; delta < _holdTime; delta += Time.deltaTime)
		{
			yield return 0f;
		}

		startPos = rect.anchorMin.y;
		targetPos = startPos + _fadeOutTravelDelta1.y;

		for (var delta = 0f; delta < _fadeOutTime; delta += Time.deltaTime)
		{

			rect.localScale = Vector3.one * Mathf.Lerp(_fadeInSize, _fadeOutSize, delta / _fadeOutTime);
			rect.SetAnchorY(Mathf.Lerp(startPos, targetPos, delta / _fadeInTime));
			text.Alpha = Mathf.Lerp(1, 0, delta / _fadeOutTime);
			yield return 0f;
		}

		Destroy(instance);
		Debug.Log("destroy");
		yield break;
	}
}
