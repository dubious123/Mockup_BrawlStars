using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Dog_BasicAttack : BaseHoldingAbility
{
	#region Serialize Field
	[SerializeField] private float _attackRadius;
	[SerializeField] private float _attackAngle;
	[SerializeField] private ParticleSystem _effects;
	[SerializeField] private AudioSource _audio;
	#endregion

	private List<BaseCharacter> _targets = new List<BaseCharacter>();
	public override void Init(BaseCharacter character)
	{
		base.Init(character);

		_clip = _animator.runtimeAnimatorController.GetAnimationClipOrNull(AnimatorMeta.BasicAttack_ClipName);
	}
	protected override void Update()
	{
		base.Update();
		if (_isBasicAttackTriggered && _currentCoroutine == null)
		{
			_currentCoroutine = StartCoroutine(Perform());
		}

		//Debug.Log(_targets.Count);
	}
	protected override IEnumerator Perform()
	{
		#region OnStart
		_audio.Play();
		_effects.Emit(100);
		_character.IsAttacking = true;
		var hits = Physics.OverlapSphere(_character.PlayerCenter, _attackRadius, LayerMeta.Character_Opponent);
		foreach (var hit in hits)
		{
			var target = hit.GetComponent<BaseCharacter>();
			if (target == null || target.IsInteractible == false || target == _character) continue;
			Vector3 targetVector = hit.gameObject.transform.position - transform.position;
			targetVector.y = 0;
			if (Vector3.Angle(targetVector, transform.forward) < Mathf.Abs(_attackAngle))
				_targets.Add(target);
		}
		#endregion

		yield return new WaitForSeconds(_clip.length / 2);
		#region OnPerform
		foreach (var target in _targets)
		{
			target.OnHit(10);
		}
		#endregion
		yield return new WaitForSeconds(_clip.length / 2);
		#region OnEnd
		_character.IsAttacking = false;
		_targets.Clear();
		_currentCoroutine = null;
		#endregion
		yield break;
	}
}
