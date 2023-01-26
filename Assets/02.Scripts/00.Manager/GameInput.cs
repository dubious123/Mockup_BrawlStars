
using System;
using System.Threading;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.InputSystem;

using static UnityEditor.Experimental.GraphView.GraphView;

public class GameInput : MonoBehaviour
{
	private static GameInput _instance;
	private PlayerInput _playerInput;
	private InputAction _lookAction;
	private InputAction _moveAction;
	private InputAction _basicAttackAction;
	private InputAction _abilityQ;
	private InputAction _abilityCancel;
	private RaycastHit _lookHit;
	private long _moveInput;
	private long _lookInput;
	private byte _buttonPressed = 0;

	public static void Init()
	{
		_instance = GameObject.Find("@Input").GetComponent<GameInput>();
		_instance._playerInput = _instance.GetComponent<PlayerInput>();
		{
			_instance._playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
			_instance._playerInput.actions.Enable();
		}

		_instance._lookAction = _instance._playerInput.actions[InputActionMeta.Look];
		_instance._basicAttackAction = _instance._playerInput.actions[InputActionMeta.BasicAttack];
		{
			_instance._basicAttackAction.started += _ => _instance._buttonPressed |= 0b0001;
			_instance._basicAttackAction.started += _ => Audio.PlayBtnPressedNormal();
			_instance._basicAttackAction.canceled += _ => _instance._buttonPressed &= 0b1110;
		}

		_instance._abilityQ = _instance._playerInput.actions[InputActionMeta.Q];
		{
			_instance._abilityQ.started += _ => _instance._buttonPressed |= 0b0010;
			_instance._abilityQ.started += _ => Audio.PlayBtnPressedNormal();
			_instance._abilityQ.canceled += _ => _instance._buttonPressed &= 0b1101;
		}

		_instance._abilityCancel = _instance._playerInput.actions[InputActionMeta.CancelAbility];
		{
			_instance._abilityCancel.started += _ => _instance._buttonPressed |= 0b0100;
			_instance._abilityCancel.canceled += _ => _instance._buttonPressed &= 0b1011;
		}
	}

	public static void SetGameInput()
	{
		if (User.Team == Enums.TeamType.Blue)
		{
			_instance._moveAction = _instance._playerInput.actions[InputActionMeta.Move];
		}
		else
		{
			_instance._moveAction = _instance._playerInput.actions[InputActionMeta.MoveInverted];
		}
	}

	public static void SetActive(bool active)
	{
		_instance.enabled = active;
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

	public static void SendInput(int frameNum)
	{
		Network.RegisterSend(new C_PlayerInput(User.UserId, User.TeamId, frameNum, (sVector2)ToVector(_instance._moveInput), (sVector2)ToVector(_instance._lookInput), _instance._buttonPressed));
	}

	private static long ToLong(ref Vector2 vector)
	{
		return ((long)BitConverter.SingleToInt32Bits(vector.x) << 32) | (BitConverter.SingleToInt32Bits(vector.y) & 0x0000_0000_ffff_ffff);
	}

	private static Vector2 ToVector(long l)
	{
		return new Vector2(BitConverter.Int32BitsToSingle((int)((ulong)l >> 32)), BitConverter.Int32BitsToSingle((int)(l & 0x0000_0000_ffff_ffff)));
	}
}
