
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
	private Vector3 _lookdir;
	private Scene_Map1 _game;
	private bool _isReady = false;
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

		Debug.Assert(Scene.CurrentScene is Scene_Map1);
		_game = Scene.CurrentScene as Scene_Map1;
		_isReady = true;
	}

	private float beforeTime;
	private void FixedUpdate()
	{
		if (_isReady == false) return;
		//if (_currentPlayer == null || _currentPlayer.IsControllable == false) return;
		if (_game.GameStarted == false) return;
		#region Move
		var moveInput = _moveAction.ReadValue<Vector2>();
		#endregion
		#region Look
		if (Physics.Raycast(Camera.main.ScreenPointToRay(_lookAction.ReadValue<Vector2>()), out _lookHit, Camera.main.farClipPlane, LayerMeta.Env))
		{
			_lookdir = _lookHit.point - transform.position;
			_lookdir.y = 0;
			_lookdir = _lookdir.normalized;
		}
		#endregion
		var t = Time.realtimeSinceStartup - beforeTime;
		if (t > 0.16f && beforeTime != 0f)
		{
			Loggers.Error.Error("{0}", t);
		}
		beforeTime = Time.realtimeSinceStartup;
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
