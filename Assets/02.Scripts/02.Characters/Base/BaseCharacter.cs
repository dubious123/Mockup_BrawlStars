using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MEC;
using TMPro;

public class BaseCharacter : MonoBehaviour
{
	public int TeamId { get; set; }
	#region SerializeFields
	[SerializeField] private float _smoothInputSpeed;
	[SerializeField] private float _runSpeed;
	[SerializeField] private float _walkSpeed;
	[SerializeField] private float _rotationSpeed;


	[SerializeField] protected int _maxHp;

	[SerializeField] private Rigidbody _rigidBody;
	[SerializeField] private Animator _animator;
	[SerializeField] private Transform _playerCenter;
	#region Abilities
	[Header("Abilities")]
	[SerializeField] private BaseHoldingAbility _basicAttack;

	[Header("Network")]
	[SerializeField] protected float _broadcastFreq;
	#endregion

	#endregion
	protected float _currentMoveSpeed;
	protected bool _isAwake;
	protected bool _isAttacking;
	protected bool _interactable;
	protected bool _canBasicAttack;
	protected bool _isCharging;
	protected Vector3 _smoothVelocity;
	protected Vector3 _targetMoveDir;
	protected Vector3 _targetLookDir;
	protected Quaternion _targetRotation;
	protected TextMeshProUGUI _stunUI;


	protected int _currentHp;

	protected bool _controllable;

	public int GetHp => _currentHp;
	public bool IsControllable
	{
		get => _controllable;
		set => _controllable = value;
	}
	public bool IsInteractible => _interactable;
	public bool IsAttacking
	{
		get => _isAttacking;
		set => _isAttacking = value;
	}
	public bool IsCharging { set => _isCharging = value; }

	public Vector3 LookDir => _targetLookDir;
	public Vector3 PlayerCenter => _playerCenter.position;

	public virtual void Init()
	{
		_basicAttack?.Init(this);
		_isAwake = true;
		_interactable = true;
		_controllable = true;
		_canBasicAttack = true;
		_stunUI = GameObject.FindGameObjectWithTag("StunCoolTime").GetComponent<TextMeshProUGUI>();
		_stunUI.enabled = false;
		_rigidBody = _rigidBody == null ? GetComponent<Rigidbody>() : _rigidBody;
		Debug.Assert(_rigidBody is not null);
	}
	public void HandleInput(in InputInfo input)
	{
		_targetMoveDir = Vector3.SmoothDamp(_targetMoveDir, input.MoveInput, ref _smoothVelocity, _smoothInputSpeed);
		_targetLookDir = input.LookInput;
	}
	public void HandleOneFrame()
	{
		if (_controllable == false || _interactable == false) return;
		#region Move
		_currentMoveSpeed =
			_targetMoveDir == Vector3.zero ? 0f :
			(_isAttacking || _isCharging) ? _walkSpeed :
			_runSpeed;
		transform.Translate(_currentMoveSpeed * Time.deltaTime * _targetMoveDir, Space.World);
		#endregion
		#region Rotate
		if (_targetLookDir != Vector3.zero) _targetRotation = Quaternion.LookRotation(Time.deltaTime * _targetLookDir, Vector3.up);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, Time.deltaTime * _rotationSpeed);
		#endregion
		#region Animatior
		_animator.SetFloat(AnimatorMeta.Speed_Float, _currentMoveSpeed);
		#endregion
	}
	public void StopAll()
	{
		if (_isAwake == false) return;
		_isAwake = false;
		_rigidBody.Sleep();
		_animator.speed = 0;
	}
	public void AwakeAll()
	{
		if (_isAwake) return;
		_rigidBody.WakeUp();
		_animator.speed = 1;
	}
	public void ActivateBasicAttack()
	{
		if (_canBasicAttack == false) return;
		_basicAttack.Activate();
	}
	public void DeactivateBasicAttack()
	{
		_basicAttack.Deactivate();
	}

	public void DisableBasicAttack()
	{
		_canBasicAttack = false;
	}
	public void EnableBasicAttack()
	{
		_canBasicAttack = true;
	}

	public virtual void OnHit(HitInfo info)
	{
		_currentHp = Mathf.Max(0, _currentHp - info.Damage);
		if (info.KnockbackDist > 0)
		{
			Timing.RunCoroutine((Co_OnKnockBack(transform.position - info.Pos, info.KnockbackDuration, info.KnockbackSpeed)), GetInstanceID().ToString());
		}
		if (info.IsStun)
		{
			Timing.RunCoroutine(Co_OnStun(info.StunDuration), GetInstanceID().ToString());
		}
		if (_currentHp <= 0)
		{
			OnDead();
			return;
		}
		_animator.SetTrigger(AnimatorMeta.GetHIt_Trigger);
	}
	protected virtual IEnumerator<float> Co_OnStun(float duration)
	{
		float currentStunTime = 0;
		DeactivateBasicAttack();
		float stunDuration = duration;
		_stunUI.enabled = true;
		Timing.CallPeriodically(stunDuration, 0.1f, () =>
		{
			stunDuration -= 0.1f;
			_stunUI.text = stunDuration.ToString("0.0");
		},
		() =>
		{
			_stunUI.enabled = false;
		});
		while (currentStunTime < duration)
		{
			_animator.SetBool(AnimatorMeta.IsStun_Bool, true);
			_controllable = false;
			_canBasicAttack = false;
			currentStunTime += Time.deltaTime;
			yield return Timing.WaitForOneFrame;
		}
		_animator.SetBool(AnimatorMeta.IsStun_Bool, false);
		_controllable = true;
		_canBasicAttack = true;
		yield break;
	}
	protected virtual IEnumerator<float> Co_OnKnockBack(Vector3 dir, float duration, float knockbackSpeed)
	{
		dir.y = 0;
		float expectedKnockbackTime = 0.3f;
		float currentKnockbackTime = 0;
		DeactivateBasicAttack();
		while (currentKnockbackTime < expectedKnockbackTime)
		{
			_controllable = false;
			_canBasicAttack = false;
			currentKnockbackTime += Time.deltaTime;
			transform.Translate(10 * Time.deltaTime * dir.normalized, Space.World);
			yield return Timing.WaitForOneFrame;
		}
		_controllable = true;
		_canBasicAttack = true;
		yield break;
	}
	protected virtual void OnDead()
	{
		DeactivateBasicAttack();
		Timing.KillCoroutines(GetInstanceID().ToString());
		_basicAttack.OnDead();
		_animator.SetBool(AnimatorMeta.IsStun_Bool, false);
		_animator.SetBool(AnimatorMeta.BasicAttack_Bool, false);
		_animator.SetBool(AnimatorMeta.Dog_Bash, false);
		_animator.SetBool(AnimatorMeta.IsDead_Bool, true);
		_controllable = false;
		_interactable = false;
	}

}
