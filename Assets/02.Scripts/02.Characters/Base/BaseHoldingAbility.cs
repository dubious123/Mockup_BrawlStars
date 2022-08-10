using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseHoldingAbility : BaseAbility
{
	#region Serialize Field
	[SerializeField] protected Animator _animator;
	#endregion
	protected AnimationClip _clip;
	protected string _clipName;
	protected bool _isBasicAttackTriggered;
	protected virtual void Update()
	{
		_animator.SetBool(AnimatorMeta.BasicAttack_Bool, _isBasicAttackTriggered);
	}
	public override void Init(BaseCharacter character)
	{
		_character = character;
	}
	public virtual void Activate()
	{
		_isBasicAttackTriggered = true;
	}
	public virtual void Deactivate()
	{
		_isBasicAttackTriggered = false;
	}
}
