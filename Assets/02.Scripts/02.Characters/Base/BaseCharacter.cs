using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaseCharacter : MonoBehaviour
{
	#region SerializeFields
	[SerializeField] private float _smoothInputSpeed;
	[SerializeField] private float _runSpeed;
	[SerializeField] private float _walkSpeed;
	[SerializeField] private float _rotationSpeed;

	[SerializeField] protected int _maxHp;

	[SerializeField] private Animator _animator;
	[SerializeField] private Transform _playerCenter;
	#region Abilities
	[SerializeField] private BaseHoldingAbility _basicAttack;
	#endregion

	#endregion
	protected float _currentMoveSpeed;
	protected bool _isAttacking;
	protected bool _interactable;
	protected bool _canBasicAttack;
	protected Vector3 _smoothVelocity;
	protected Vector3 _targetMoveDir;
	protected Vector3 _targetLookDir;
	protected Quaternion _targetRotation;

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
	public Vector3 LookDir => _targetLookDir;
	public Vector3 PlayerCenter => _playerCenter.position;

	protected virtual void Awake()
	{
		_basicAttack?.Init(this);
		_interactable = true;
		_controllable = true;
		_canBasicAttack = true;
	}
	private void Update()
	{
		if (_controllable == false || _interactable == false) return;
		#region Move
		_currentMoveSpeed =
			_targetMoveDir == Vector3.zero ? 0f :
			_isAttacking ? _walkSpeed :
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
	public void Move(Vector3 moveDir)
	{
		_targetMoveDir = Vector3.SmoothDamp(_targetMoveDir, moveDir, ref _smoothVelocity, _smoothInputSpeed);
	}
	public void Look(Vector3 lookDir)
	{
		_targetLookDir = lookDir;
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

	public virtual void OnHit(int demage)
	{
		_animator.SetTrigger(AnimatorMeta.GetHIt_Trigger);
	}
	protected virtual void OnDead()
	{
		DeactivateBasicAttack();
		_animator.SetBool(AnimatorMeta.IsDead_Bool, true);
		_controllable = false;
		_interactable = false;
	}
}
