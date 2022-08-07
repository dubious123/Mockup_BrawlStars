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
	private InputAction _moveAction;
	private Vector2 _moveActionValue;
	private Vector2 _currentInputVector;
	private Vector2 _smoothVelocity;
	private Vector3 _targetDir;
	private Quaternion _targetRotation;
	private void Awake()
	{
		_playerInput = GetComponent<PlayerInput>();
		_moveAction = _playerInput.actions["Move"];
	}
	private void Update()
	{
		_moveActionValue = _moveAction.ReadValue<Vector2>();
		#region Move
		_currentInputVector = Vector2.SmoothDamp(_currentInputVector, _moveActionValue, ref _smoothVelocity, _smoothInputSpeed);
		_targetDir = new Vector3(_currentInputVector.x, 0, _currentInputVector.y);
		transform.Translate(_moveSpeed * Time.deltaTime * _targetDir, Space.World);
		#endregion
		#region Rotate
		if (_moveActionValue != Vector2.zero)
		{
			_targetRotation = Quaternion.LookRotation(_moveSpeed * Time.deltaTime * _targetDir, Vector3.up);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, Time.deltaTime * _rotationSpeed);
		}
		#endregion



	}
}
