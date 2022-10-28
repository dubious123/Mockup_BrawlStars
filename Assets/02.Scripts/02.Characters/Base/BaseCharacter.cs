using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using MEC;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

using Utils;

using static Enums;
using static S_BroadcastGameState;
using static UnityEngine.GraphicsBuffer;

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
	#region Skills
	[Header("Skills")]
	[SerializeField] private BaseSkill _basicAttack;

	#endregion

	#endregion
	protected float _currentMoveSpeed;
	protected bool _isAwake;
	protected bool _interactable;
	protected sVector3 _position;
	protected sVector3 _smoothVelocity;
	protected sVector3 _targetMoveDir;
	protected sVector3 _targetLookDir;
	protected sQuaternion _targetRotation;
	protected sQuaternion _rotation;
	protected TextMeshProUGUI _stunUI;

	public void EnableMoveControll(bool value) => _moveControllEnabled = value;
	protected bool _moveControllEnabled = true;
	public void EnableLookControll(bool value) => _lookControllEnabled = value;
	protected bool _lookControllEnabled = true;

	protected int _currentHp;
	protected bool _controllable;
	public int GetHp => _currentHp;
	public bool IsControllable
	{
		get => _controllable;
		set => _controllable = value;
	}
	public bool IsInteractible => _interactable;
	public sVector3 LookDir => _targetLookDir;
	public sVector3 PlayerCenter => (sVector3)_playerCenter.position;

	public sVector3 Position => _position;
	public sQuaternion Rotation => _rotation;

	public virtual void Init()
	{
		_basicAttack.Init(this);
		_isAwake = true;
		_interactable = true;
		_controllable = true;
		//_canBasicAttack = true;
		_stunUI = GameObject.FindGameObjectWithTag("StunCoolTime").GetComponent<TextMeshProUGUI>();
		_stunUI.enabled = false;
		_rigidBody = _rigidBody == null ? GetComponent<Rigidbody>() : _rigidBody;
		Debug.Assert(_rigidBody is not null);
		_position = (sVector3)transform.position;
		_rotation = (sQuaternion)transform.rotation;
	}

	public virtual void HandleInput(ref Vector2 moveDir, ref Vector2 lookDir, ushort buttonPressed)
	{
		_targetMoveDir = sVector3.SmoothDamp(_targetMoveDir, new sVector3(moveDir.x, 0, moveDir.y), ref _smoothVelocity, (sfloat)_smoothInputSpeed, sfloat.PositiveInfinity, ((sfloat)1 / (sfloat)60f));
		_targetLookDir = new sVector3(lookDir.x, 0, lookDir.y);
		_basicAttack.HandleInput((buttonPressed & 1) == 1);
	}

	public virtual HitInfo GetHitInfo(uint actionCode)
	{
		switch (actionCode)
		{
			case 0:
				return new HitInfo
				{
					Damage = 10,
					IsStun = false,
					KnockbackDist = 0,
				};
			default:
				return null;
		}
	}

	public virtual void HandleOneFrame()
	{
		#region Move
		if (_moveControllEnabled)
		{
			_currentMoveSpeed = _targetMoveDir == sVector3.zero ? 0f : _runSpeed;
			var delta = (sfloat)_currentMoveSpeed * ((sfloat)1 / (sfloat)60f) * _targetMoveDir;
			_position += delta;
			transform.position = new Vector3((float)_position.x, transform.position.y, (float)_position.z);
		}

		#endregion
		#region Rotate
		if (_lookControllEnabled)
		{
			if (_targetLookDir != sVector3.zero)
			{
				_targetRotation = sQuaternion.LookRotation((sfloat)Time.fixedDeltaTime * _targetLookDir, sVector3.up);
			}

			_rotation = sQuaternion.RotateTowards(_rotation, _targetRotation, (sfloat)Time.fixedDeltaTime * (sfloat)_rotationSpeed);
			transform.rotation = (Quaternion)_rotation;
		}

		#endregion
		#region Animatior
		_animator.SetFloat(AnimatorMeta.Speed_Float, _currentMoveSpeed);
		#endregion


		#region Skills
		_basicAttack.HandleOneFrame();
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

	public virtual void SetOtherSkillsActive(uint skillId, bool active)
	{
		if (_basicAttack.Id == skillId == false) _basicAttack.SetActive(active);
	}

	public virtual void OnGetHit(in HitInfo info)
	{
		_currentHp = Mathf.Max(0, _currentHp - info.Damage);
		if (_currentHp <= 0)
		{
			OnDead();
			return;
		}
		if (info.KnockbackDist > 0)
		{
			Timing.RunCoroutine((Co_OnKnockBack(transform.position - info.Pos, info.KnockbackDuration, info.KnockbackSpeed)), GetInstanceID().ToString());
		}
		if (info.IsStun)
		{
			Timing.RunCoroutine(Co_OnStun(info.StunDuration), GetInstanceID().ToString());
		}

		_animator.SetTrigger(AnimatorMeta.GetHIt_Trigger);
	}
	protected virtual IEnumerator<float> Co_OnStun(float duration)
	{
		float currentStunTime = 0;
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
			//_canBasicAttack = false;
			currentStunTime += Time.deltaTime;
			yield return Timing.WaitForOneFrame;
		}
		_animator.SetBool(AnimatorMeta.IsStun_Bool, false);
		_controllable = true;
		//_canBasicAttack = true;
		yield break;
	}
	protected virtual IEnumerator<float> Co_OnKnockBack(Vector3 dir, float duration, float knockbackSpeed)
	{
		dir.y = 0;
		float expectedKnockbackTime = 0.3f;
		float currentKnockbackTime = 0;
		while (currentKnockbackTime < expectedKnockbackTime)
		{
			_controllable = false;
			//_canBasicAttack = false;
			currentKnockbackTime += Time.deltaTime;
			transform.Translate(10 * Time.deltaTime * dir.normalized, Space.World);
			yield return Timing.WaitForOneFrame;
		}
		_controllable = true;
		//_canBasicAttack = true;
		yield break;
	}
	protected virtual void OnDead()
	{
		Timing.KillCoroutines(GetInstanceID().ToString());
		_animator.SetBool(AnimatorMeta.IsStun_Bool, false);
		_animator.SetBool(AnimatorMeta.BasicAttack_Bool, false);
		_animator.SetBool(AnimatorMeta.Dog_Bash, false);
		_animator.SetBool(AnimatorMeta.IsDead_Bool, true);
		_controllable = false;
		_interactable = false;
	}

}
