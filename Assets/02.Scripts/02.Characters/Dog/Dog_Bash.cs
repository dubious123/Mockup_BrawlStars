using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Dog_Bash : BaseAbility
{
	#region SerializeFields
	[SerializeField] float _maxBashlength;
	[SerializeField] float _chargingTimeLimit;
	[SerializeField] float _smoothSpeed;
	[SerializeField] float _bashSpeed;
	[SerializeField] Animator _animator;
	[SerializeField] RawImage _indicator;
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
	public override void Init(BaseCharacter character)
	{
		_character = character;
		_collider = GetComponent<Collider>();
		_collider.enabled = false;
		_clip = _animator.runtimeAnimatorController.GetAnimationClipOrNull(AnimatorMeta.Dog_Bash);
	}

	public void ChargeBash()
	{
		if (_currentCoroutine != null) return;
		_released = false;
		_canceled = false;
		_currentBashLengh = 0;
		_currentChargingTime = 0;
		_currentCoroutine = StartCoroutine(Perform());
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
		target.OnHit(9);
	}
	protected override IEnumerator Perform()
	{
		_character.DisableBasicAttack();
		_character.IsCharging = true;
		_indicator.enabled = true;
		while (_currentChargingTime <= _chargingTimeLimit && _released == false)
		{
			_currentChargingTime += Time.deltaTime;
			_currentBashLengh = Mathf.SmoothDamp(_currentBashLengh, _maxBashlength, ref _smoothVelocity, _smoothSpeed);
			_currentBashLengh = Mathf.Min(_currentBashLengh, _maxBashlength);
			_indicator.rectTransform.sizeDelta = new Vector2(100, _currentBashLengh * 100);
			yield return new WaitForEndOfFrame();
		}
		_indicator.enabled = false;
		if (_canceled == true)
		{
			_character.IsCharging = false;
			StopCoroutine(_currentCoroutine);
			_currentCoroutine = null;
			yield break;
		}
		_character.IsCharging = false;
		_expectedBashTime = _currentBashLengh / _bashSpeed;
		_currentBashTime = 0;
		_collider.enabled = true;
		_animator.SetBool(AnimatorMeta.Dog_Bash, true);
		while (_currentBashTime <= _expectedBashTime)
		{
			_character.IsControllable = false;
			_currentBashTime += Time.deltaTime;
			_character.transform.Translate(_bashSpeed * Time.deltaTime * _character.LookDir, Space.World);
			yield return new WaitForEndOfFrame();
		}
		_collider.enabled = false;
		_character.IsControllable = true;
		_character.EnableBasicAttack();
		_animator.SetBool(AnimatorMeta.Dog_Bash, false);
		_currentCoroutine = null;
		yield break;
	}
}
