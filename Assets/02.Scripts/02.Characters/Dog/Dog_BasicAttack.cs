using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Dog_BasicAttack : BaseHoldingAbility
{
	#region Serialize Field
	[SerializeField] private AnimationEvent_Tool _animEventTool;
	[SerializeField] private float _attackRadius;
	[SerializeField] private float _attackAngle;
	#endregion

	private List<BaseCharacter> _targets = new List<BaseCharacter>();
	public override void Init(BaseCharacter character)
	{
		base.Init(character);

		_clip = _animator.runtimeAnimatorController.GetAnimationClipOrNull(AnimatorMeta.BasicAttack_ClipName);
		_animEventTool.AssignEvent(_clip, 0, () => OnAbilityStart());
		_animEventTool.AssignEvent(_clip, _clip.length / 2, () => OnAbilityPerform());
		_animEventTool.AssignEvent(_clip, _clip.length, () => OnAbilityEnd());
	}
	protected override void Update()
	{
		base.Update();
		Debug.Log(_targets.Count);
	}

	protected override void OnAbilityStart()
	{
		_character.IsAttacking = true;
		var hits = Physics.OverlapSphere(_character.PlayerCenter, _attackRadius, LayerMeta.Character_Opponent);
		foreach (var hit in hits)
		{
			var target = hit.GetComponent<BaseCharacter>();
			if (target == null) continue;
			Vector3 targetVector = hit.gameObject.transform.position - transform.position;
			targetVector.y = 0;
			if (Vector3.Angle(targetVector, transform.forward) < Mathf.Abs(_attackAngle))
				_targets.Add(target);
		}
	}

	protected override void OnAbilityPerform()
	{
		foreach (var target in _targets)
		{

			target.OnHit();
		}
	}

	protected override void OnAbilityEnd()
	{
		_character.IsAttacking = false;
		_targets.Clear();
	}






}
