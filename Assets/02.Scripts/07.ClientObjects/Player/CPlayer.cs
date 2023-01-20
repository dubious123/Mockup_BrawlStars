using MEC;

using Server.Game;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CPlayer : MonoBehaviour
{
	public NetCharacter NPlayer { get; set; }
	public short TeamId { get; set; }
	public int MaxHp => NPlayer.MaxHp;
	public int Hp => NPlayer.Hp;
	public sVector3 Lookdir => NPlayer.TargetLookDir;
	public sVector3 MoveDir => NPlayer.TargetMoveDir;
	[field: SerializeField] public Animator Animator { get; set; }
	[field: SerializeField] public Image StunIndicator { get; set; }
	[field: SerializeField] public Sprite ProfileIcon { get; set; }

	protected bool Active { get; set; }

	[SerializeField] private Sprite _selectCircleSelf;
	[SerializeField] private Sprite _selectCircleRed;
	[SerializeField] private Sprite _selectCircleBlue;
	[SerializeField] private SpriteRenderer SelectCircle;
	[SerializeField] private GameObject _hud;
	[SerializeField] private GameObject _mesh;
	[SerializeField] private ParticleSystem _moveSmokeEffect;
	[SerializeField] private CPlayerEffect _cPlayerEffect;
	[SerializeField] private MoveCircle _moveCircle;

	public virtual void Init(NetCharacter character, short teamId)
	{
		NPlayer = character;
		TeamId = teamId;
		if (teamId == User.TeamId)
		{
			SelectCircle.sprite = _selectCircleSelf;
			_moveCircle.gameObject.SetActive(true);
		}
		else if (character.Team == User.Team)
		{
			SelectCircle.sprite = _selectCircleBlue;
		}
		else
		{
			SelectCircle.sprite = _selectCircleRed;
		}
	}

	public virtual void OnMatchStart()
	{
		_hud.SetActive(true);
		Active = true;
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
				_hud.SetActive(false);
			}
		}
		else
		{
			_mesh.SetActive(true);
			_hud.SetActive(true);
		}

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
		_hud.SetActive(false);
		Active = false;
	}

	public virtual void OnClear()
	{
		if (Active is false)
		{
			return;
		}

		_cPlayerEffect.PlayeDeadEffect();
		_mesh.SetActive(false);
		_hud.SetActive(false);
	}

	public virtual void Reset()
	{
		Debug.Log("Reset");
		_mesh.SetActive(true);
		_hud.SetActive(true);
		Active = true;
	}
}