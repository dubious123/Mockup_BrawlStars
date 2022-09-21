using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
	#region SerializeFields		
	[SerializeField] protected BaseCharacter _currentPlayer;
	#endregion
	protected PlayerInput _playerInput;
	protected InputAction _lookAction;
	protected InputAction _moveAction;
	protected InputAction _basicAttackAction;
	protected InputAction _abilityQ;
	protected InputAction _abilityCancel;
	protected RaycastHit _lookHit;
	protected Vector3 _lookdir;

	public virtual void Init(BaseCharacter playableCharacter)
	{
		_currentPlayer = playableCharacter;
		_playerInput = GetComponent<PlayerInput>();
		_moveAction = _playerInput.actions[InputActionMeta.Move];
		_lookAction = _playerInput.actions[InputActionMeta.Look];
		_basicAttackAction = _playerInput.actions[InputActionMeta.BasicAttack];
		_abilityQ = _playerInput.actions[InputActionMeta.Q];
		_abilityCancel = _playerInput.actions[InputActionMeta.CancelAbility];
	}

	private void Update()
	{
		if (_currentPlayer == null || _currentPlayer.IsControllable == false)
		{
			return;
		}
		#region Move
		var moveInput = _moveAction.ReadValue<Vector2>();
		_currentPlayer.Move(new Vector3(moveInput.x, 0, moveInput.y));
		Debug.Log(moveInput);
		#endregion
		#region Look
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
