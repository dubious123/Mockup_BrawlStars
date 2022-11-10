using System.Collections;
using System.Collections.Generic;

using MEC;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class HudHP : MonoBehaviour
{
	#region SerializeFields
	[SerializeField] private Slider _fillSlider;
	[SerializeField] private Slider _followSlider;
	[SerializeField] private CPlayer _player;
	[SerializeField] private float _holdingTime;
	[SerializeField] private TextMeshProUGUI _healthText;
	[SerializeField] private RectTransform _canvasRect;
	[SerializeField] private Vector3 _offset;
	[SerializeField] private Transform _targetTransform;
	#endregion
	private RectTransform _rect;
	private float _targetFollowValue;
	private CoroutineHandle _coHandle;

	private void Start()
	{
		_rect = GetComponent<RectTransform>();
		_fillSlider.onValueChanged.AddListener(_ =>
		{
			Timing.KillCoroutines(_coHandle);
			_coHandle = Timing.RunCoroutine(Co_OnHpChange(_));
		});

		if (_player.TeamId == User.TeamId)
		{
			transform.parent.GetComponent<Canvas>().sortingOrder = 1;
		}
	}

	private void LateUpdate()
	{
		_fillSlider.value = _player.Hp;
		_healthText.text = $"{_fillSlider.value}";
		_rect.anchoredPosition = Camera.main.WorldtoCanvasRectPos(_canvasRect.sizeDelta, _targetTransform.position) + new Vector2(0, 100);
	}

	private IEnumerator<float> Co_OnHpChange(float value)
	{
		yield return Timing.WaitForSeconds(0.5f);
		_targetFollowValue = value;
		float elapsed = 0f;
		float startValue = _followSlider.value;
		while (elapsed < _holdingTime)
		{
			_followSlider.value = Mathf.Lerp(startValue, _targetFollowValue, elapsed / _holdingTime);
			elapsed += Time.deltaTime;
			yield return Timing.WaitForOneFrame;
		}
		yield break;
	}
}
