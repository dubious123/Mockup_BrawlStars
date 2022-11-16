
using System.Collections;
using System.Collections.Generic;

using Server.Game;

using UnityEngine;

public class CKnightWhirlwind : MonoBehaviour, ICBaseSkill
{
	public NetKnightWhirlwind NetWhirlwind { get; set; }
	public CPlayerKnight Player { get; set; }
	public bool Performing { get; set; }
	public bool Active { get; set; }
	[SerializeField] protected ParticleSystem _effects;
	[SerializeField] private AudioClip _audioClip;
	private IEnumerator<int> _handler;

	public void Init(CPlayerKnight player)
	{
		Player = player;
		NetWhirlwind = (player.NPlayer as NetCharacterKnight).Whirlwind as NetKnightWhirlwind;
	}

	public void HandleOneFrame()
	{
		Player.Animator.SetBool(AnimatorMeta.IsBasicAttack, NetWhirlwind.Performing);
		if (NetWhirlwind.CurrentSpinFrame == NetWhirlwind.SpinIntervalFrame)
		{
			AudioSource.PlayClipAtPoint(_audioClip, transform.position);
			Debug.Log("Spin");
			_effects.Emit(100);
			return;
		}
	}
}
