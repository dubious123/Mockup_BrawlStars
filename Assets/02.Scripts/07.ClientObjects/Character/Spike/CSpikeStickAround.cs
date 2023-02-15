using MEC;

using Server.Game;

using UnityEngine;
using UnityEngine.InputSystem;

public class CSpikeStickAround : MonoBehaviour
{
	[SerializeField] private GameObject _indicator;
	[SerializeField] private GameObject _indicatorFollowCircle;
	[SerializeField] private AudioClip _audio;
	[SerializeField] private HudPowerCircle _hudPowerCircle;

	public bool Active { get; set; }
	public ClientCharacterSpike Character { get; set; }

	private NSpikeStickAround _netStickAround;
	private CoroutineHandle _coOnHold;
	private Vector3 _currentIndicatorMoveVelocity;

	public void Init(ClientCharacterSpike spike)
	{
		Character = spike;
		_netStickAround = (spike.NPlayer as NCharacterSpike).SpecialAttack as NSpikeStickAround;
		_hudPowerCircle.Init(_netStickAround);
		if (spike.TeamId == User.TeamId)
		{
			_indicatorFollowCircle.transform.parent = Scene.CurrentScene.transform;
			_indicatorFollowCircle.gameObject.SetActive(false);
			GameInput.PowerInputAction.started += OnPressed;
			GameInput.PowerInputAction.canceled += OnReleased;
		}
	}

	public void HandleAttack()
	{
		Audio.PlayOnce(_audio, 0.5f);
		Audio.PlayerPowerPerformed();
		Character.Animator.SetBool(AnimatorMeta.IsAttack, true);
		//Character.PlayerEffect.PlaySpecialAttackEffect();
	}

	public void Reset()
	{
		_hudPowerCircle.Reset();
	}

	private void OnPressed(InputAction.CallbackContext _)
	{
		if (_netStickAround.CanAttack())
		{
			Timing.KillCoroutines(_coOnHold);
			_coOnHold = Timing.CallPeriodically(float.MaxValue, 0.016f, OnHold);
		}
	}

	private void OnHold()
	{
		var targetDelta = GameInput.LookDelta;
		//Debug.Log(targetDelta);
		//Debug.Log(targetDelta.magnitude > _netStickAround.MaxRadius);
		var delta2 = targetDelta.magnitude > _netStickAround.MaxRadius ? targetDelta.normalized * (float)_netStickAround.MaxRadius : targetDelta;
		_indicatorFollowCircle.transform.position = Vector3.SmoothDamp(_indicatorFollowCircle.transform.position, Character.transform.position + new Vector3(delta2.x, 0.2001f, delta2.y), ref _currentIndicatorMoveVelocity, 0.016f, float.PositiveInfinity);
		_indicator.SetActive(true);
		_indicatorFollowCircle.SetActive(true);
	}

	private void OnReleased(InputAction.CallbackContext _)
	{
		_indicator.SetActive(false);
		_indicatorFollowCircle.SetActive(false);
		Timing.KillCoroutines(_coOnHold);
	}

	private void OnDestroy()
	{
		Timing.KillCoroutines(_coOnHold);
		GameInput.PowerInputAction.started -= OnPressed;
		GameInput.PowerInputAction.canceled -= OnReleased;
	}
}
