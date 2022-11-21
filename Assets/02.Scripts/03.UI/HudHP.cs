using System.Collections;
using System.Collections.Generic;

using Coffee.UIExtensions;

using MEC;

using TMPro;

using Unity.VectorGraphics;

using UnityEngine;

public class HudHP : MonoBehaviour
{
	#region SerializeFields
	[SerializeField] private CPlayer _player;
	[SerializeField] private float _holdTime;
	[SerializeField] private float _followTime;
	[SerializeField] private float _expandRate = 1.1f;
	[SerializeField] private TextWithShadow _healthText;
	[SerializeField] private RectTransform _canvasRect;
	[SerializeField] private Vector3 _offset;
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
	[SerializeField] private UIParticle _particleBar;
	[SerializeField] private UIParticle _particleTextBody;
	[SerializeField] private UIParticle _particleTextShadow;
	[SerializeField] private TextMeshProUGUI _bodyMask;
	[SerializeField] private TextMeshProUGUI _shadowMask;
	#endregion
	private int _beforeHp;
	private int _currentHp;
	private bool _isSelf;
	private RectTransform _rect;
	private CoroutineHandle _coHandle;

	private void Start()
	{
		_rect = GetComponent<RectTransform>();
		_beforeHp = _currentHp = _player.Hp;
		SetText();

		if (_player.TeamId == User.TeamId)
		{
			transform.parent.GetComponent<Canvas>().sortingOrder = 1;
			_fillImage.sprite = _SpriteFillSelf;
			_followImage.sprite = _SpriteFollowSelf;
			_isSelf = true;
			return;
		}

		if (_player.NPlayer.Team == User.Team)
		{
			_fillImage.sprite = _SpriteFillBlue;
			_followImage.sprite = _SpriteFollowBlue;
		}
		else
		{
			_fillImage.sprite = _SpriteFillRed;
			_followImage.sprite = _SpriteFollowRed;
		}

		_isSelf = false;
	}

	private void LateUpdate()
	{
		_rect.anchoredPosition = Camera.main.WorldtoCanvasRectPos(_canvasRect.sizeDelta, _targetTransform.position + _offset);
		_currentHp = _player.Hp;
		if (_beforeHp == _currentHp)
		{
			return;
		}

		Timing.KillCoroutines(_coHandle);
		_coHandle = _isSelf ? Timing.RunCoroutine(Co_OnHpChangeSelf()) : Timing.RunCoroutine(Co_OnHpChange());
		SetText();
		_beforeHp = _currentHp;
	}

	private IEnumerator<float> Co_OnHpChange()
	{
		PlayParticle();
		float elapsed = 0f;
		float targetValue = _currentHp / (float)_player.MaxHp;
		Vector2 anchorMax = _fillRect.anchorMax;
		while (elapsed < _holdTime)
		{
			var ratio = elapsed / _holdTime;
			anchorMax.x = Mathf.Lerp(anchorMax.x, targetValue, ratio);
			_fillRect.anchorMax = anchorMax;
			elapsed += Timing.DeltaTime;
			yield return Timing.WaitForOneFrame;
		}

		anchorMax.x = targetValue;
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

	private IEnumerator<float> Co_OnHpChangeSelf()
	{
		PlayParticle();
		float elapsed = 0f;
		float targetValue = _currentHp / (float)_player.MaxHp;
		var targetScale = _rect.localScale;
		var currentScale = targetScale * _expandRate;
		_rect.localScale = currentScale;
		Vector2 anchorMax = _fillRect.anchorMax;
		while (elapsed < _holdTime)
		{
			var ratio = elapsed / _holdTime;
			anchorMax.x = Mathf.Lerp(anchorMax.x, targetValue, ratio);
			_rect.localScale = Vector3.Lerp(currentScale, targetScale, ratio);
			_fillRect.anchorMax = anchorMax;
			elapsed += Timing.DeltaTime;
			yield return Timing.WaitForOneFrame;
		}

		anchorMax.x = targetValue;
		_rect.localScale = targetScale;
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

	private void PlayParticle()
	{
		_particleBar.Play();
		_particleTextBody.Play();
		_particleTextShadow.Play();
	}

	private void SetText()
	{
		var str = _currentHp.ToString();
		_healthText.Text = str;
		_bodyMask.text = str;
		_shadowMask.text = str;
	}
}
