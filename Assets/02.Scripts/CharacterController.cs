using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
	#region SerializeFields
	[SerializeField] private float _smoothInputSpeed;
	[SerializeField] private float _moveSpeed;
	#endregion

	private PlayerInput _playerInput;
	private InputAction _moveAction;
	private Vector2 _currentInputVector;
	private Vector2 _smoothVelocity;
	private void Awake()
	{
		_playerInput = GetComponent<PlayerInput>();
		_moveAction = _playerInput.actions["Move"];
	}
	private void Update()
	{
		_currentInputVector = Vector2.SmoothDamp(_currentInputVector, _moveAction.ReadValue<Vector2>(), ref _smoothVelocity, _smoothInputSpeed);
		transform.Translate(_moveSpeed * Time.deltaTime * new Vector3(_currentInputVector.x, 0, _currentInputVector.y));
		Debug.Log(_moveAction.ReadValue<Vector2>());
	}
}
