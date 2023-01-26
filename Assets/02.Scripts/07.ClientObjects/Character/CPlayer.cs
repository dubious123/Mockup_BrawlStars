using MEC;

using Server.Game;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using static Enums;

public class CPlayer : MonoBehaviour
{
	public NetCharacter NPlayer { get; set; }
	public TeamType Team => NPlayer.Team;
	public int TeamId { get; set; }
	public int MaxHp => NPlayer.MaxHp;
	public int Hp => NPlayer.Hp;
	public sVector3 Lookdir => NPlayer.TargetLookDir;
	public sVector3 MoveDir => NPlayer.TargetMoveDir;
	[field: SerializeField] public Animator Animator { get; set; }
	[field: SerializeField] public Image StunIndicator { get; set; }
	[field: SerializeField] public Sprite ProfileIcon { get; set; }

	protected bool Active { get; set; }

	[SerializeField] private CPlayerUI _ui;
	[SerializeField] private GameObject _mesh;
	[SerializeField] private ParticleSystem _moveSmokeEffect;
	[SerializeField] private CPlayerEffect _cPlayerEffect;

	public virtual void Init(NetCharacter character)
	{
		NPlayer = character;
		TeamId = NPlayer.NetObjId.InstanceId;

		_ui.Init(this);
	}

	public virtual void OnGameStart()
	{
		Active = true;
	}

	public virtual void OnRoundStart()
	{
		_ui.OnRoundStart();
	}

	public virtual void HandleOneFrame()
	{
		if (Active is false)
		{
			return;
		}

		if (NPlayer.IsVisible is false)
		{
			if (NPlayer.Team != User.Team)
			{
				_mesh.SetActive(false);
				_ui.gameObject.SetActive(false);
			}
		}
		else
		{
			_mesh.SetActive(true);
			_ui.gameObject.SetActive(true);
		}

		_ui.HandleOneFrame();

		if (NPlayer.CCFlag.HasFlag(Enums.CCFlags.Stun))
		{
			Animator.SetBool(AnimatorMeta.IsStun_Bool, true);
		}

		transform.SetPositionAndRotation((Vector3)NPlayer.Position, (Quaternion)NPlayer.Rotation);

		if (NPlayer.TargetMoveDir != sVector3.zero && _moveSmokeEffect.isPlaying is false)
		{
			_moveSmokeEffect.gameObject.SetActive(true);
			_moveSmokeEffect.Play();
		}
	}

	public virtual void OnDead()
	{
		Debug.Log("onDead");
		_cPlayerEffect.PlayeDeadEffect();
		_mesh.SetActive(false);
		_ui.OnPlayerDead();
		Active = false;
	}

	public virtual void OnRoundClear()
	{
		if (Active is false)
		{
			return;
		}

		_cPlayerEffect.PlayeDeadEffect();
		_mesh.SetActive(false);
		_ui.OnRoundClear();
	}

	public virtual void OnRoundReset()
	{
		Debug.Log("Reset");
		_mesh.SetActive(true);
		_ui.OnRoundReset();
		Active = true;
	}
}