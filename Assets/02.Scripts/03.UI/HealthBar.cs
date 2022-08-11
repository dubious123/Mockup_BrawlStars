using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
	#region SerializeFields
	[SerializeField] Slider _fillSlider;
	[SerializeField] Slider _followSlider;
	[SerializeField] BaseCharacter _character;
	[SerializeField] float _holdingTime;
	[SerializeField] TextMeshProUGUI _healthText;

	#endregion
	private Transform _camPos;
	private float _targetFollowValue;
	private Coroutine _currentCo;
	void Start()
	{
		_camPos = Camera.main.transform;
		_fillSlider.onValueChanged.AddListener(_ =>
		{
			if (_currentCo != null) StopCoroutine(_currentCo);
			_currentCo = StartCoroutine(OnHpChange(_));
		});
	}
	private void LateUpdate()
	{
		_fillSlider.value = _character.GetHp;
		_healthText.text = $"{_fillSlider.value}";
		transform.LookAt(transform.position + _camPos.forward);
	}
	private IEnumerator OnHpChange(float value)
	{
		yield return new WaitForSeconds(0.5f);
		_targetFollowValue = value;
		float elapsed = 0f;
		float startValue = _followSlider.value;
		while (elapsed < _holdingTime)
		{
			_followSlider.value = Mathf.Lerp(startValue, _targetFollowValue, elapsed / _holdingTime);
			elapsed += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		_currentCo = null;
		yield break;
	}
}
