using System.Collections;
using System.Collections.Generic;

using MEC;

using Unity.VectorGraphics;

using UnityEngine;

public class HudHP : MonoBehaviour
{
	#region SerializeFields
	[SerializeField] private CPlayer _player;
	[SerializeField] private float _holdTime;
	[SerializeField] private float _followTime;
	[SerializeField] private TextWithShadow _healthText;
	[SerializeField] private RectTransform _canvasRect;
	[SerializeField] private Vector2 _offset;
	[SerializeField] private Transform _targetTransform;


	[SerializeField] private RectTransform _fillRect;
	[SerializeField] private RectTransform _followRect;
	[SerializeField] private SVGImage _fillImage;
	[SerializeField] private SVGImage _followImage;
	[SerializeField] private Sprite _SpriteFillSelf;
	[SerializeField] private Sprite _SpriteFillBlue;
	[SerializeField] private Sprite _SpriteFillRed;
	[SerializeField] private Sprite _SpriteFollowSelf;
	[SerializeField] private Sprite _SpriteFollowBlue;
	[SerializeField] private Sprite _SpriteFollowRed;
	#endregion
	private int _beforeHp;
	private int _currentHp;
	private RectTransform _rect;
	private CoroutineHandle _coHandle;

	private void Start()
	{
		_rect = GetComponent<RectTransform>();
		_beforeHp = _currentHp = _player.Hp;
		_healthText.Text = _currentHp.ToString();

		if (_player.TeamId == User.TeamId)
		{
			transform.parent.GetComponent<Canvas>().sortingOrder = 1;
			_fillImage.sprite = _SpriteFillSelf;
			_followImage.sprite = _SpriteFillSelf;
		}
		else if (_player.NPlayer.Team == User.Team)
		{
			_fillImage.sprite = _SpriteFillBlue;
			_followImage.sprite = _SpriteFollowBlue;
		}
		else
		{
			_fillImage.sprite = _SpriteFillRed;
			_followImage.sprite = _SpriteFollowRed;
		}
	}

	private void LateUpdate()
	{
		_rect.anchoredPosition = Camera.main.WorldtoCanvasRectPos(_canvasRect.sizeDelta, _targetTransform.position) + _offset;
		_currentHp = _player.Hp;
		if (_beforeHp == _currentHp)
		{
			return;
		}

		Timing.KillCoroutines(_coHandle);
		_coHandle = Timing.RunCoroutine(Co_OnHpChange());
		_healthText.Text = _currentHp.ToString();
		_beforeHp = _currentHp;
	}

	private IEnumerator<float> Co_OnHpChange()
	{
		float elapsed = 0f;
		float targetValue = _currentHp / (float)_player.MaxHp;
		Vector2 anchorMax = _fillRect.anchorMax;
		while (elapsed < _holdTime)
		{
			anchorMax.x = Mathf.Lerp(anchorMax.x, targetValue, elapsed / _holdTime);
			_fillRect.anchorMax = anchorMax;
			elapsed += Timing.DeltaTime;
			yield return Timing.WaitForOneFrame;
		}

		elapsed = 0f;
		anchorMax = _followRect.anchorMax;
		while (elapsed < _followTime)
		{
			anchorMax.x = Mathf.Lerp(anchorMax.x, targetValue, elapsed / _followTime);
			_followRect.anchorMax = anchorMax;
			elapsed += Timing.DeltaTime;
			yield return Timing.WaitForOneFrame;
		}
		yield break;
	}
}
