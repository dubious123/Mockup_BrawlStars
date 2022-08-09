using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
	#region SerializeFields
	[SerializeField] private float _smoothInputSpeed;
	[SerializeField] private float _moveSpeed;
	[SerializeField] private float _rotationSpeed;
	#endregion

	private PlayerInput _playerInput;
	private InputAction _lookAction;
	private InputAction _moveAction;
	private Vector2 _currentInputVector;
	private Vector2 _smoothVelocity;
	private Vector3 _targetMoveDir;
	private Vector3 _targetLookDir;
	private RaycastHit _lookHit;
	private Quaternion _targetRotation;
	private void Awake()
	{
		_playerInput = GetComponent<PlayerInput>();
		_moveAction = _playerInput.actions[InputActionMeta.Move];
		_lookAction = _playerInput.actions[InputActionMeta.Look];
	}
	private void Update()
	{

		#region Move
		_currentInputVector = Vector2.SmoothDamp(_currentInputVector, _moveAction.ReadValue<Vector2>(), ref _smoothVelocity, _smoothInputSpeed);
		_targetMoveDir = new Vector3(_currentInputVector.x, 0, _currentInputVector.y);
		transform.Translate(_moveSpeed * Time.deltaTime * _targetMoveDir, Space.World);
		#endregion
		#region Rotate
		if (Physics.Raycast(Camera.main.ScreenPointToRay(_lookAction.ReadValue<Vector2>()), out _lookHit, Camera.main.farClipPlane, LayerMeta.Floor))
		{
			_targetLookDir = _lookHit.point - transform.position;
			_targetLookDir.y = 0;
			_targetLookDir = _targetLookDir.normalized;
		}
		_targetRotation = Quaternion.LookRotation(_moveSpeed * Time.deltaTime * _targetLookDir, Vector3.up);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, Time.deltaTime * _rotationSpeed);
		#endregion



	}
}
