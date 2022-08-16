using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;
using TMPro;

public class Dog_Bash : BaseAbility
{
	#region SerializeFields
	[SerializeField] float _maxBashlength;
	[SerializeField] float _chargingTimeLimit;
	[SerializeField] float _smoothSpeed;
	[SerializeField] float _bashSpeed;
	[SerializeField] Animator _animator;
	[SerializeField] RawImage _indicator;
	[SerializeField] float _coolTime;
	#endregion
	protected AnimationClip _clip;
	private Collider _collider;

	private float _currentBashLengh;
	private float _currentBashTime;
	private float _currentChargingTime;
	private float _expectedBashTime;

	private float _smoothVelocity;
	private bool _released;
	private bool _canceled;
	private bool _isRunning;

	private TextMeshProUGUI _coolTimeUI;
	private float _currentCoolTime;

	public override void Init(BaseCharacter character)
	{
		_character = character;
		_isRunning = false;
		_collider = GetComponent<Collider>();
		_collider.enabled = false;
		_clip = _animator.runtimeAnimatorController.GetAnimationClipOrNull(AnimatorMeta.Dog_Bash);
		_coolTimeUI = GameObject.FindGameObjectsWithTag("CoolTime")[0].GetComponent<TextMeshProUGUI>();
		_coolTimeUI.enabled = false;
		_currentCoolTime = 0f;
		_hitInfo = new HitInfo
		{
			Damage = 9,
			IsStun = true,
			StunDuration = 1,
			KnockbackDist = 1,
			KnockbackDuration = 0.3f,
			KnockbackSpeed = 10f,
		};
	}

	public void ChargeBash()
	{
		if (_isRunning || _currentCoolTime > 0) return;
		_released = false;
		_canceled = false;
		_currentBashLengh = 0;
		_currentChargingTime = 0;
		_isRunning = true;
		_currentCoolTime = _coolTime;
		Timing.RunCoroutine(Co_Perform());
	}
	public void ReleaseBash()
	{
		_released = true;
	}
	public void CancelBash()
	{
		_released = true;
		_canceled = true;
	}
	private void OnTriggerEnter(Collider other)
	{
		var target = other.transform.GetComponent<BaseCharacter>();
		if (target == null || target == _character) return;
		target.OnHit(_hitInfo with { Pos = transform.position });
	}
	protected override IEnumerator<float> Co_Perform()
	{
		_isRunning = true;
		_character.DisableBasicAttack();
		_character.IsCharging = true;
		_indicator.enabled = true;
		while (_currentChargingTime <= _chargingTimeLimit && _released == false)
		{
			_currentChargingTime += Time.deltaTime;
			_currentBashLengh = Mathf.SmoothDamp(_currentBashLengh, _maxBashlength, ref _smoothVelocity, _smoothSpeed);
			_currentBashLengh = Mathf.Min(_currentBashLengh, _maxBashlength);
			_indicator.rectTransform.sizeDelta = new Vector2(100, _currentBashLengh * 100);
			yield return Timing.WaitForOneFrame;
		}
		_indicator.enabled = false;
		if (_canceled == true)
		{
			_character.IsCharging = false;
			_isRunning = false;
			yield break;
		}
		_character.IsCharging = false;
		_expectedBashTime = _currentBashLengh / _bashSpeed;
		_currentBashTime = 0;
		_collider.enabled = true;
		_coolTimeUI.enabled = true;
		_coolTimeUI.text = _coolTime.ToString();
		_animator.SetBool(AnimatorMeta.Dog_Bash, true);
		Timing.CallPeriodically(_coolTime, 0.1f, () =>
		{
			_currentCoolTime -= 0.1f;
			_coolTimeUI.text = _currentCoolTime.ToString("0.0");
			Debug.Log("hi");
		},
		() =>
		{
			_coolTimeUI.enabled = false;
			_currentCoolTime = 0;
		}
		);
		while (_currentBashTime <= _expectedBashTime)
		{
			_character.IsControllable = false;
			_currentBashTime += Time.deltaTime;
			_character.transform.Translate(_bashSpeed * Time.deltaTime * _character.LookDir, Space.World);
			yield return Timing.WaitForOneFrame;
		}
		_collider.enabled = false;
		_character.IsControllable = true;
		_character.EnableBasicAttack();
		_animator.SetBool(AnimatorMeta.Dog_Bash, false);
		_isRunning = false;

		yield break;
	}
}
