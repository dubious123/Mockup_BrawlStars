using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
	#region SerializeFields		
	[SerializeField] private BaseCharacter _currentPlayer;
	#endregion



	private PlayerInput _playerInput;
	private InputAction _lookAction;
	private InputAction _moveAction;
	private InputAction _basicAttackAction;
	private RaycastHit _lookHit;
	private Vector3 _lookdir;
	private void Awake()
	{
		_playerInput = GetComponent<PlayerInput>();
		_moveAction = _playerInput.actions[InputActionMeta.Move];
		_lookAction = _playerInput.actions[InputActionMeta.Look];
		_basicAttackAction = _playerInput.actions[InputActionMeta.BasicAttack];
		_basicAttackAction.started += _ => _currentPlayer.ActivateBasicAttack();
		_basicAttackAction.canceled += _ => _currentPlayer.DeactivateBasicAttack();
	}
	private void Update()
	{
		var moveInput = _moveAction.ReadValue<Vector2>();
		_currentPlayer.Move(new Vector3(moveInput.x, 0, moveInput.y));
		#region Move

		#endregion
		#region Rotate
		if (Physics.Raycast(Camera.main.ScreenPointToRay(_lookAction.ReadValue<Vector2>()), out _lookHit, Camera.main.farClipPlane, LayerMeta.Floor))
		{
			_lookdir = _lookHit.point - transform.position;
			_lookdir.y = 0;
			_lookdir = _lookdir.normalized;
		}
		_currentPlayer.Look(_lookdir);
		#endregion
	}
}
