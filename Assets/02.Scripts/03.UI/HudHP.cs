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
	[SerializeField] private HudHealthTextEffect _textEffectOnDamage;
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
	private Vector3 _targetScale;
	private RectTransform _rect;
	private CoroutineHandle _coHandle;

	public void Init()
	{
		_rect = GetComponent<RectTransform>();
		_beforeHp = _currentHp = _player.Hp;
		_targetScale = _rect.localScale;
		SetText();

		if (_player.TeamId == User.TeamId)
		{
			_fillImage.sprite = _SpriteFillSelf;
			_followImage.sprite = _SpriteFollowSelf;
			_isSelf = true;
			_textEffectOnDamage.IsSelf = true;
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
		_textEffectOnDamage.IsSelf = false;
	}

	public void HandleOneFrame()
	{
		//_rect.anchoredPosition = Camera.main.WorldtoCanvasRectPos(_canvasRect.sizeDelta, _targetTransform.position + _offset);
		_currentHp = _player.Hp;
		if (_beforeHp == _currentHp)
		{
			return;
		}

		Timing.KillCoroutines(_coHandle);
		_coHandle = _isSelf ? Timing.RunCoroutine(Co_OnHpChangeSelf()) : Timing.RunCoroutine(Co_OnHpChange());
		SetText();
		_textEffectOnDamage.Damage = _beforeHp - _currentHp;
		_textEffectOnDamage.PlayEffect();
		_beforeHp = _currentHp;
	}

	public void Reset()
	{
		Timing.KillCoroutines(_coHandle);
		_beforeHp = _currentHp = _player.Hp;
		_fillRect.anchorMax = new Vector2(_currentHp / (float)_player.MaxHp, 1);
		_followRect.anchorMax = new Vector2(_currentHp / (float)_player.MaxHp, 1);
		SetText();
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
		var currentScale = _targetScale * _expandRate;
		_rect.localScale = currentScale;
		Vector2 anchorMax = _fillRect.anchorMax;
		while (elapsed < _holdTime)
		{
			var ratio = elapsed / _holdTime;
			anchorMax.x = Mathf.Lerp(anchorMax.x, targetValue, ratio);
			_rect.localScale = Vector3.Lerp(currentScale, _targetScale, ratio);
			_fillRect.anchorMax = anchorMax;
			elapsed += Timing.DeltaTime;
			yield return Timing.WaitForOneFrame;
		}

		anchorMax.x = targetValue;
		_rect.localScale = _targetScale;
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
