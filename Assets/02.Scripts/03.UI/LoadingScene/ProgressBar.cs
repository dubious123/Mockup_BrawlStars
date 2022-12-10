using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ProgressBar : MonoBehaviour
{
	[SerializeField] private GameObject _barLeft;
	[SerializeField] private GameObject _barCenter;
	[SerializeField] private GameObject _barRight;
	[SerializeField] private GameObject _barEnd;
	[SerializeField] private Transform _barHolder;
	[SerializeField] private TextWithShadow _text;
	[field: SerializeField] public int Percent { get; set; }

	public void UpdateProgress(int progressPrecent, string message = null)
	{
		Percent = progressPrecent;
		_text.Text = message ?? $"{progressPrecent}%";
		if (Percent <= 0)
		{
			return;
		}
		else if (Percent >= 100)
		{
			_barLeft.SetActive(true);
			_barCenter.SetActive(true);
			var rectCenter = _barCenter.GetComponent<RectTransform>();
			rectCenter.anchorMax = new Vector2(0.975f, 1);
			_barRight.SetActive(false);
			_barEnd.SetActive(true);
			return;
		}

		_barLeft.SetActive(true);
		_barCenter.SetActive(true);
		var rect = _barCenter.GetComponent<RectTransform>();
		rect.anchorMax = new Vector2(Percent / 100f - 0.025f, 1);
		_barRight.SetActive(true);
		rect = _barRight.GetComponent<RectTransform>();
		rect.anchorMin = new Vector2(Percent / 100f - 0.05f, 0);
		rect.anchorMax = new Vector2(Percent / 100f, 1);
		//if (CurrentPercent > NewPercent || NewPercent < 5)
		//{
		//	return;
		//}

		//if (CurrentPercent == 0)
		//{
		//	_barLeft.SetActive(true);
		//	CurrentPercent += 5;
		//}

		//for (; CurrentPercent < NewPercent - 5; CurrentPercent += 5)
		//{
		//	Instantiate(_barCenterPrefab, _barHolder);
		//	var rect = _barCenterPrefab.GetComponent<RectTransform>();
		//	rect.anchorMin = new Vector2(CurrentPercent / 100f, 0);
		//	rect.anchorMax = new Vector2(CurrentPercent / 100f + 0.05f, 1);
		//}

		//if (CurrentPercent >= 95)
		//{
		//	_barRight.SetActive(false);
		//	_barEnd.SetActive(true);
		//}
		//else
		//{
		//	_barRight.SetActive(true);
		//	var rect = _barRight.GetComponent<RectTransform>();
		//	rect.anchorMin = new Vector2(CurrentPercent / 100f, 0);
		//	rect.anchorMax = new Vector2(CurrentPercent / 100f + 0.05f, 1);
		//}
	}
}
