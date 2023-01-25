
using System;
using System.Threading;

using UnityEngine;
using UnityEngine.InputSystem;


public class CharacterController : MonoBehaviour
{
	private PlayerInput _playerInput;
	private InputAction _lookAction;
	private InputAction _moveAction;
	private InputAction _basicAttackAction;
	private InputAction _abilityQ;
	private InputAction _abilityCancel;
	private RaycastHit _lookHit;
	//private Vector3 _lookdir;
	//private Vector2 _moveInput;
	private long _moveInput;
	private long _lookInput;
	private byte _buttonPressed = 0;

	public virtual void Init()
	{
		_playerInput = GetComponent<PlayerInput>();
		if (User.Team == Enums.TeamType.Blue)
		{
			_moveAction = _playerInput.actions[InputActionMeta.Move];
		}
		else
		{
			_moveAction = _playerInput.actions[InputActionMeta.MoveInverted];
		}

		_lookAction = _playerInput.actions[InputActionMeta.Look];
		_basicAttackAction = _playerInput.actions[InputActionMeta.BasicAttack];
		{
			_basicAttackAction.started += _ => _buttonPressed |= 0b0001;
			_basicAttackAction.started += _ => Audio.PlayBtnPressedNormal();
			_basicAttackAction.canceled += _ => _buttonPressed &= 0b1110;
		}

		_abilityQ = _playerInput.actions[InputActionMeta.Q];
		{
			_abilityQ.started += _ => _buttonPressed |= 0b0010;
			_abilityQ.started += _ => Audio.PlayBtnPressedNormal();
			_abilityQ.canceled += _ => _buttonPressed &= 0b1101;
		}

		_abilityCancel = _playerInput.actions[InputActionMeta.CancelAbility];
		{
			_abilityCancel.started += _ => _buttonPressed |= 0b0100;
			_abilityCancel.canceled += _ => _buttonPressed &= 0b1011;
		}
	}

	private void FixedUpdate()
	{
		var moveDir = _moveAction.ReadValue<Vector2>();
		Interlocked.Exchange(ref _moveInput, ToLong(ref moveDir));
		if (Physics.Raycast(Camera.main.ScreenPointToRay(_lookAction.ReadValue<Vector2>()), out _lookHit, Camera.main.farClipPlane, LayerMeta.Env))
		{
			var temp = _lookHit.point - transform.position;
			var lookDir = new Vector2(temp.x, temp.z).normalized;
			Interlocked.Exchange(ref _lookInput, ToLong(ref lookDir));
		}
	}

	public void SendInput(int frameNum)
	{
		Network.RegisterSend(new C_PlayerInput(User.UserId, User.TeamId, frameNum, (sVector2)ToVector(_moveInput), (sVector2)ToVector(_lookInput), _buttonPressed));
	}

	private long ToLong(ref Vector2 vector)
	{
		return ((long)BitConverter.SingleToInt32Bits(vector.x) << 32) | (BitConverter.SingleToInt32Bits(vector.y) & 0x0000_0000_ffff_ffff);
	}

	private Vector2 ToVector(long l)
	{
		var b = (int)((ulong)l >> 32);
		var f = BitConverter.Int32BitsToSingle(b);
		return new Vector2(BitConverter.Int32BitsToSingle((int)((ulong)l >> 32)), BitConverter.Int32BitsToSingle((int)(l & 0x0000_0000_ffff_ffff)));
	}
}
