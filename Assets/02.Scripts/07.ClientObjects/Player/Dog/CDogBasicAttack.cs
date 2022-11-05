
using System.Collections;
using System.Collections.Generic;

using Server.Game;

using UnityEngine;

public class CDogBasicAttack : MonoBehaviour, ICBaseSkill
{
	public NetDogBasicAttack NetBasicAttack { get; set; }
	public CPlayerDog Player { get; set; }
	public bool Performing { get; set; }
	public bool Active { get; set; }

	[SerializeField] protected ParticleSystem _effects;
	[SerializeField] protected AudioSource _audio;
	private IEnumerator<int> _handler;

	public void Init(CPlayerDog player)
	{
		Player = player;
		NetBasicAttack = (player.NPlayer as NetCharacterDog).BasicAttack as NetDogBasicAttack;
		_handler = Co_Perform();
	}

	public void HandleOneFrame()
	{
		if (NetBasicAttack.Performing)
		{
			_handler.MoveNext();
		}
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
