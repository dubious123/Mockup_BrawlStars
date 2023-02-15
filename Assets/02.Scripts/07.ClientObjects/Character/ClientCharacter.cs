using Server.Game;

using UnityEngine;
using UnityEngine.UI;

using static Enums;

public class ClientCharacter : ClientBaseComponent<NetCharacter>
{
	public NetCharacter NPlayer { get; private set; }
	public TeamType Team => NPlayer.Team;
	public int TeamId => NPlayer.TeamId;
	public int MaxHp => Now.MaxHp;
	public int Hp => Now.Hp;
	public Sprite ProfileIcon { get; private set; }

	[field: SerializeField] public Animator Animator { get; set; }
	[field: SerializeField] public Image StunIndicator { get; set; }
	[field: SerializeField] public CPlayerEffect PlayerEffect { get; set; }

	protected NPlayerSnapShot Now { get; private set; }
	protected NPlayerSnapShot Next { get; private set; }

	[SerializeField] private AudioClip[] _audio_start;
	[SerializeField] private CPlayerUI _ui;
	[SerializeField] private GameObject _mesh;
	[SerializeField] private ParticleSystem _moveSmokeEffect;

	public override void Init(NetCharacter character)
	{
		NPlayer = character;
		ProfileIcon = Data.GetCharacterProfile(NPlayer.NetObjId.Type);
		Now = new NPlayerSnapShot();
		Now.TakePicture(character);
		Next = new NPlayerSnapShot();
		Next.TakePicture(character);
		_ui.Init(this);
		Active = true;
	}

	public override void OnNetFrameUpdate()
	{
		if (Next.PowerAmount > Now.PowerAmount)
		{
			Audio.PlayerPowerGain();
			PlayerEffect.PlayChargeEffect();
		}

		(Now, Next) = (Next, Now);
		Next.TakePicture(NPlayer);
		Animator.SetBool(AnimatorMeta.IsAttack, Now.IsBasicAttack);
	}

	public override void Interpretate(float ratio)
	{
		if (Active is false)
		{
			return;
		}

		var pos = Vector3.Lerp(Now.Position, Next.Position, ratio);
		var rot = Quaternion.Lerp(Now.Rotation, Next.Rotation, ratio);
		transform.SetPositionAndRotation(pos, rot);
		SetVisible(Now.IsVisible);
		Animator.SetFloat(AnimatorMeta.Speed_Float, (float)NPlayer.MoveSpeed * (float)NPlayer.TargetMoveDir.magnitude);
		_ui.HandleOneFrame();
		if (Now.IsDead)
		{
			HandleDead();
		}
	}

	public virtual void OnGameStart()
	{
		Active = true;
		if (User.TeamId == TeamId && _audio_start?.Length > 0)
		{
			Audio.PlayOnce(_audio_start[Random.Range(0, _audio_start.Length)]);
		}
	}

	public virtual void OnRoundStart()
	{
		_ui.gameObject.SetActive(true);
	}

	public virtual void HandleDead()
	{
		PlayerEffect.PlayeDeadEffect(Team == User.Team);
		Audio.PlayPlayerDead(Team != User.Team);
		_mesh.SetActive(false);
		_ui.gameObject.SetActive(false);
		Active = false;
		(Scene.CurrentScene as Scene_Map1).UI.OnPlayerDead(this);
	}

	public virtual void OnRoundClear()
	{
		if (NPlayer.IsDead() is false)
		{
			PlayerEffect.PlayeDeadEffect(Team == User.Team);
			Audio.PlayPlayerDisabled();
		}

		_mesh.SetActive(false);
		_ui.gameObject.SetActive(false);
		Active = false;
	}

	public virtual void Reset()
	{
		transform.SetPositionAndRotation((Vector3)NPlayer.Position, (Quaternion)NPlayer.Rotation);
		Now.TakePicture(NPlayer);
		Next.TakePicture(NPlayer);
		SetVisible(true);
		_ui.Reset();
		Active = true;
	}

	protected virtual void SetVisible(bool visible)
	{
		if (visible is false && Team == User.Team)
		{
			return;
		}

		_mesh.SetActive(visible);
		_ui.gameObject.SetActive(visible);
	}

	protected class NPlayerSnapShot : SnapShot
	{
		public Vector3 Position { get; private set; }
		public Quaternion Rotation { get; private set; }
		public bool IsVisible { get; private set; }
		public bool IsDead { get; private set; }
		public bool IsBasicAttack { get; private set; }
		public bool IsSpecialAttack { get; private set; }
		public int MaxHp { get; private set; }
		public int Hp { get; private set; }
		public int PowerAmount { get; private set; }

		public override void TakePicture(NetCharacter nPlayer)
		{
			Position = (Vector3)nPlayer.Position;
			Rotation = (Quaternion)nPlayer.Rotation;
			IsVisible = nPlayer.IsVisible;
			IsDead = nPlayer.IsDead();
			IsBasicAttack = nPlayer.BasicAttack.IsAttack;
			IsSpecialAttack = nPlayer.SpecialAttack.IsAttack;
			Hp = nPlayer.Hp;
			MaxHp = nPlayer.MaxHp;
			PowerAmount = nPlayer.SpecialAttack.CurrentPowerAmount;
		}
	}
}