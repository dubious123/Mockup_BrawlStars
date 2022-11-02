using System.Collections;
using System.Collections.Generic;

using Logging;

using Server.Game;

using UnityEngine;
using UnityEngine.InputSystem;

using static Enums;

public class CharacterController : MonoBehaviour
{
	protected NetCharacter _currentPlayer;
	protected PlayerInput _playerInput;
	protected InputAction _lookAction;
	protected InputAction _moveAction;
	protected InputAction _basicAttackAction;
	protected InputAction _abilityQ;
	protected InputAction _abilityCancel;
	protected RaycastHit _lookHit;
	protected Vector3 _lookdir;
	protected Scene_Map1 _game;
	protected bool _isReady = false;
	protected byte _buttonPressed = 0;

	public virtual void Init(NetCharacter playableCharacter)
	{
		_currentPlayer = playableCharacter;
		_playerInput = GetComponent<PlayerInput>();
		_moveAction = _playerInput.actions[InputActionMeta.Move];
		_lookAction = _playerInput.actions[InputActionMeta.Look];
		_basicAttackAction = _playerInput.actions[InputActionMeta.BasicAttack];
		{
			_basicAttackAction.started += _ => _buttonPressed |= 0b0001;
			_basicAttackAction.canceled += _ => _buttonPressed &= 0b1110;
		}
		_abilityQ = _playerInput.actions[InputActionMeta.Q];
		{
			_abilityQ.started += _ => _buttonPressed |= 0b0010;
			_abilityQ.canceled += _ => _buttonPressed &= 0b1101;
		}
		_abilityCancel = _playerInput.actions[InputActionMeta.CancelAbility];
		{
			_abilityCancel.started += _ => _buttonPressed |= 0b0100;
			_abilityCancel.canceled += _ => _buttonPressed &= 0b1011;
		}
		Debug.Assert(Scene.CurrentScene is Scene_Map1);
		_game = Scene.CurrentScene as Scene_Map1;
		_isReady = true;
	}

	private void FixedUpdate()
	{
		if (_isReady == false) return;
		//if (_currentPlayer == null || _currentPlayer.IsControllable == false) return;
		if (_game.GameStarted == false) return;
		#region Move
		var moveInput = _moveAction.ReadValue<Vector2>();
		#endregion
		#region Look
		if (Physics.Raycast(Camera.main.ScreenPointToRay(_lookAction.ReadValue<Vector2>()), out _lookHit, Camera.main.farClipPlane, LayerMeta.Floor))
		{
			_lookdir = _lookHit.point - transform.position;
			_lookdir.y = 0;
			_lookdir = _lookdir.normalized;
		}
		#endregion

		//Todo object pooling to reduce gc
		//LogMgr.Log(LogSourceType.Debug, $"[Tick : {_game.CurrentTick}]\ninput ¹ß»ý, move : {moveInput}, look : {_lookdir}");
		Network.RegisterSend(new C_PlayerInput(User.UserId, _game.CurrentTick, (sVector2)moveInput, new sVector2(_lookdir.x, _lookdir.z), _buttonPressed));
		//_game.EnqueueInputInfo(User.TeamId, new InputInfo()
		//{
		//	LookInput = _lookdir,
		//	MoveInput = new Vector3(moveInput.x, 0, moveInput.y),
		//	StartTick = _game.CurrentTick,
		//	TargetTick = _game.CurrentTick + 6, //input lag = 0.1sec
		//});
	}
}
