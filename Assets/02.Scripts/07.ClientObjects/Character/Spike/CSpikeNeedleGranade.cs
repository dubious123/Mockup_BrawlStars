using MEC;

using Server.Game;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CSpikeNeedleGranade : MonoBehaviour
{
	[SerializeField] private Graphic _indicator;
	[SerializeField] private AudioClip _audio;
	[SerializeField] private Color _red;
	[SerializeField] private Color _white;

	public bool Active { get; set; }
	public ClientCharacterSpike Character { get; set; }

	private NSpikeNeedleGrenade _netNeedleGrenade;
	private CoroutineHandle _coHandle;

	public void Init(ClientCharacterSpike spike)
	{
		Character = spike;
		_netNeedleGrenade = (spike.NPlayer as NCharacterSpike).BasicAttack as NSpikeNeedleGrenade;
		if (spike.TeamId == User.TeamId)
		{
			GameInput.BasicAttackInputAction.started += OnPressed;
			GameInput.BasicAttackInputAction.canceled += OnReleased;
			_indicator.transform.localScale = new Vector3(10 / 1.5f, 1, 1);
			_coHandle = Timing.CallContinuously(float.MaxValue, () => _indicator.color = _netNeedleGrenade.CurrentShellCount > 0 ? _white : _red);
		}
	}

	public void HandleAttack()
	{
		Audio.PlayOnce(_audio, 0.5f);
	}

	private void OnPressed(InputAction.CallbackContext _)
	{
		_indicator.gameObject.SetActive(true);
	}

	private void OnReleased(InputAction.CallbackContext _)
	{
		_indicator.gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		GameInput.BasicAttackInputAction.started -= OnPressed;
		GameInput.BasicAttackInputAction.canceled -= OnReleased;
		Timing.KillCoroutines(_coHandle);
	}
}
