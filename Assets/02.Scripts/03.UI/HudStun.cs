using System;

using UnityEngine;
using UnityEngine.UI;

public class HudStun : MonoBehaviour
{
	#region SerializeFields
	[SerializeField] private CPlayer _player;
	[SerializeField] private RectTransform _canvasRect;
	[SerializeField] private Vector2 _offset;
	[SerializeField] private Image _centerIcon;
	[SerializeField] private Image _durationCircle;
	#endregion
	private RectTransform _rect;
	private int _stunDuration;

	private void Start()
	{
		_rect = GetComponent<RectTransform>();
		_centerIcon.enabled = false;
		_durationCircle.enabled = false;
		if (_player.TeamId == User.TeamId)
		{
			transform.parent.GetComponent<Canvas>().sortingOrder = 1;
		}
	}

	private void Update()
	{
		var currentDuration = _player.NPlayer.StunDuration;
		if (currentDuration > 0)
		{
			_stunDuration = Math.Max(currentDuration, _stunDuration);
			_centerIcon.enabled = true;
			_durationCircle.enabled = true;
			_durationCircle.fillAmount = currentDuration / (float)_stunDuration;
		}
		else
		{
			_centerIcon.enabled = false;
			_durationCircle.enabled = false;
		}

		_rect.anchoredPosition = Camera.main.WorldtoCanvasRectPos(_canvasRect.sizeDelta, _player.transform.position) + _offset;
	}
}
