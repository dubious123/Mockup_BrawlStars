
using System.Collections;
using System.Collections.Generic;

using Server.Game;

using UnityEngine;

public class CKnightBasicAttack : MonoBehaviour, ICBaseSkill
{
	public NetDogBasicAttack NetBasicAttack { get; set; }
	public CPlayerKnight Player { get; set; }
	public bool Performing { get; set; }
	public bool Active { get; set; }
	[SerializeField] protected ParticleSystem _effects;
	[SerializeField] protected AudioSource _audio;
	private IEnumerator<int> _handler;

	public void Init(CPlayerKnight player)
	{
		Player = player;
		NetBasicAttack = (player.NPlayer as NetCharacterKnight).Whirlwind as NetDogBasicAttack;
		_handler = Co_Perform();
	}

	public void HandleOneFrame()
	{
		if (NetBasicAttack.Performing)
		{
			Player.Animator.SetBool("WhilWind", true);
		}

		Player.Animator.SetBool("WhilWind", false);
	}

	public IEnumerator<int> Co_Perform()
	{
		while (true)
		{
			_audio.Play();
			_effects.Emit(100);
			Player.Animator.SetTrigger(AnimatorMeta.BasicAttack_Trigger);
			for (int i = 0; i < 59; i++)
			{
				yield return 0;
			}

			yield return 0;
		}
	}
}
