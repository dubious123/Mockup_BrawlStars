using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBasicAttack : BaseSkill
{
	#region Serialize Field
	[SerializeField] protected ParticleSystem _effects;
	[SerializeField] protected AudioSource _audio;
	[SerializeField] protected Animator _animator;
	#endregion
	protected IEnumerator<int> _coHandler;
	protected bool _performing = false;
	public override void Init(BaseCharacter character)
	{
		base.Init(character);
		_coHandler = Co_Perform();
		Id = 1;
	}
	public override void HandleOneFrame()
	{
		if (_performing) _coHandler.MoveNext();
	}
	public override void HandleInput(bool buttonPressed)
	{
		if (_enabled == false) return;
		if (_performing) return;
		if (buttonPressed == false) return;
		_character.SetOtherSkillsActive(Id, false);
		_character.EnableLookControll(false);
		_character.EnableMoveControll(false);

		_performing = true;
	}
	public override void Cancel()
	{
		_coHandler = null;
	}
	protected IEnumerator<int> Co_Perform()
	{
		while (true)
		{
			_audio.Play();
			_effects.Emit(100);
			_animator.SetTrigger(AnimatorMeta.BasicAttack_Trigger);
			for (int i = 0; i < 60; i++)
			{
				yield return 0;
			}
			_performing = false;
			_character.SetOtherSkillsActive(Id, true);
			_character.EnableLookControll(true);
			_character.EnableMoveControll(true);
			yield return 0;
		}
	}
}
