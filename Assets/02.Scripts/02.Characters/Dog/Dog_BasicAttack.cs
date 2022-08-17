using MEC;
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
	private bool _isRunning;
	private List<BaseCharacter> _targets = new List<BaseCharacter>();

	public override void Init(BaseCharacter character)
	{
		base.Init(character);
		_clip = _animator.runtimeAnimatorController.GetAnimationClipOrNull(AnimatorMeta.BasicAttack_ClipName);
		_isRunning = false;
		_hitInfo = new HitInfo
		{
			Damage = 10,
			IsStun = false,
			KnockbackDist = 0,
		};
	}
	public override void OnDead()
	{
		Timing.KillCoroutines(_coHandle);
	}
	protected override void Update()
	{
		base.Update();
		if (_isBasicAttackTriggered && _isRunning == false)
		{
			_isRunning = true;
			_coHandle = Timing.RunCoroutine(Co_Perform());
		}

		//Debug.Log(_targets.Count);
	}
	protected override IEnumerator<float> Co_Perform()
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

		yield return Timing.WaitForSeconds(_clip.length / 2);
		#region OnPerform
		foreach (var target in _targets)
		{
			target.OnHit(_hitInfo);
		}
		#endregion

		yield return Timing.WaitForSeconds(_clip.length / 2);
		#region OnEnd
		_character.IsAttacking = false;
		_targets.Clear();
		_isRunning = false;
		#endregion
		yield break;
	}
}
