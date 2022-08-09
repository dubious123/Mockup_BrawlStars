using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
	#region SerializeFields
	[SerializeField] private float _smoothInputSpeed;
	[SerializeField] private float _runSpeed;
	[SerializeField] private float _walkSpeed;
	[SerializeField] private float _rotationSpeed;
	[SerializeField] private Animator _animator;
	#endregion

	private float _currentMoveSpeed;
	private PlayerInput _playerInput;
	private InputAction _lookAction;
	private InputAction _moveAction;
	private InputAction _basicAttackAction;
	private Vector2 _currentInputVector;
	private Vector2 _smoothVelocity;
	private Vector3 _targetMoveDir;
	private Vector3 _targetLookDir;
	private RaycastHit _lookHit;
	private Quaternion _targetRotation;

	private AnimationClip _basicAttackClip;
	private bool _isBasicAttackTriggered;
	private bool _isAttacking;
	private void Awake()
	{
		_currentMoveSpeed = _runSpeed;
		_playerInput = GetComponent<PlayerInput>();
		_moveAction = _playerInput.actions[InputActionMeta.Move];
		_lookAction = _playerInput.actions[InputActionMeta.Look];
		_basicAttackAction = _playerInput.actions[InputActionMeta.BasicAttack];
		_basicAttackAction.started += _ => _isBasicAttackTriggered = true;
		_basicAttackAction.canceled += _ => _isBasicAttackTriggered = false;
		_basicAttackClip = _animator.runtimeAnimatorController.GetAnimationClipOrNull(AnimatorMeta.BasicAttack_ClipName);
		var basicAttackStartEvent = new AnimationEvent();
		{
			basicAttackStartEvent.time = 0;
			basicAttackStartEvent.functionName = "OnBasicAttackStart";
		}
		var basicAttackEndEvent = new AnimationEvent();
		{
			basicAttackEndEvent.time = _basicAttackClip.length;
			basicAttackEndEvent.functionName = "OnBasicAttackEnd";
		}
		_basicAttackClip.AddEvent(basicAttackStartEvent);
		_basicAttackClip.AddEvent(basicAttackEndEvent);



	}
	private void Update()
	{
		#region Move
		_currentMoveSpeed =
			_moveAction.ReadValue<Vector2>() == Vector2.zero ? 0f :
			_isAttacking ? _walkSpeed :
			_runSpeed;

		_currentInputVector = Vector2.SmoothDamp(_currentInputVector, _moveAction.ReadValue<Vector2>(), ref _smoothVelocity, _smoothInputSpeed);
		_targetMoveDir = new Vector3(_currentInputVector.x, 0, _currentInputVector.y);
		transform.Translate(_currentMoveSpeed * Time.deltaTime * _targetMoveDir, Space.World);
		#endregion
		#region Rotate
		if (Physics.Raycast(Camera.main.ScreenPointToRay(_lookAction.ReadValue<Vector2>()), out _lookHit, Camera.main.farClipPlane, LayerMeta.Floor))
		{
			_targetLookDir = _lookHit.point - transform.position;
			_targetLookDir.y = 0;
			_targetLookDir = _targetLookDir.normalized;
		}
		_targetRotation = Quaternion.LookRotation(Time.deltaTime * _targetLookDir, Vector3.up);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, Time.deltaTime * _rotationSpeed);
		#endregion
		#region Animatior
		_animator.SetFloat(AnimatorMeta.Speed_Float, _currentMoveSpeed);
		_animator.SetBool(AnimatorMeta.BasicAttack_Bool, _isBasicAttackTriggered);
		#endregion
	}
	private void OnBasicAttackStart()
	{
		_isAttacking = true;
	}
	private void OnBasicAttackEnd()
	{
		_isAttacking = false;

	}
}