using System.Collections;
using System.Collections.Generic;

using MEC;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class Dog_Bash : BaseSkill
{
	#region Serialize Field
	[SerializeField] protected ParticleSystem _effects;
	[SerializeField] protected AudioSource _audio;
	[SerializeField] protected Animator _animator;
	[SerializeField] protected RawImage _skillIndicator;
	[SerializeField] private float _maxBashlength;
	[SerializeField] private int _maxChargeTime;
	[SerializeField] private int _holdingTimeLimit;
	[SerializeField] private float _bashSpeed;
	[SerializeField] private int _coolTime;
	#endregion
	protected TextMeshProUGUI _coolTimeIndicator;
	protected IEnumerator<int> _coHandler;

	protected bool _buttonPressed;
	protected bool _performing;
	public override void Init(BaseCharacter character)
	{
		base.Init(character);
		_coHandler = Co_Perform();
		_coolTimeIndicator = GameObject.FindGameObjectsWithTag("CoolTime")[0].GetComponent<TextMeshProUGUI>();
		_coolTimeIndicator.enabled = false;
		_performing = false;
		Id = 2;
	}
	public override void HandleOneFrame()
	{
		if (_performing) _coHandler.MoveNext();
	}
	public override void HandleInput(bool buttonPressed)
	{
		if (_enabled == false) return;
		_buttonPressed = buttonPressed;
		if (_performing == false && _buttonPressed)
		{
			_character.SetOtherSkillsActive(Id, false);
			_performing = true;
			return;
		}
	}
	public override void Cancel()
	{

	}
	protected IEnumerator<int> Co_Perform()
	{
		while (true)
		{
			sfloat bashLength;
			sfloat bashTime;
			int holdFrame = 0;
			CoroutineHandle handler;
			#region Charge
			{
				_skillIndicator.enabled = true;
				_skillIndicator.rectTransform.sizeDelta = new Vector2(100, 0);
				handler = Timing.CallContinuously(_holdingTimeLimit * 60, () =>
				{
					_skillIndicator.rectTransform.sizeDelta = new Vector2(100, _maxBashlength * Mathf.Min(holdFrame / (float)_maxChargeTime, 1) * 100);
				});
				for (; holdFrame < _holdingTimeLimit; holdFrame++)
				{
					yield return 0;
					if (_buttonPressed == false) break;
				}
			}
			#endregion
			#region Bash
			{
				_character.EnableLookControll(false);
				_character.EnableMoveControll(false);
				Timing.KillCoroutines(handler);
				_skillIndicator.enabled = false;
				_animator.SetBool(AnimatorMeta.Dog_Bash, true);
				bashLength = (sfloat)_maxBashlength * sMathf.Min((sfloat)holdFrame / (sfloat)_maxChargeTime, (sfloat)1f);
				bashTime = bashLength / (sfloat)_bashSpeed;

				Debug.Log($"bashTime : {bashTime}, bashLength : {bashLength}");
				for (sfloat current = (sfloat)0f; current <= bashTime; current += (sfloat)Time.fixedDeltaTime)
				{
					_character.Position += (_character.LookDir * (sfloat)_bashSpeed * (sfloat)Time.fixedDeltaTime);
					yield return 0;
				}
				_character.EnableLookControll(true);
				_character.EnableMoveControll(true);
			}
			#endregion

			_character.SetOtherSkillsActive(Id, true);

			#region Wait For CoolTime
			{
				_animator.SetBool(AnimatorMeta.Dog_Bash, false);
				_coolTimeIndicator.enabled = true;
				for (int i = _coolTime; i > 0; i--)
				{
					_coolTimeIndicator.text = (i * Time.fixedDeltaTime).ToString("0.0");
					yield return 0;
				}
				_coolTimeIndicator.enabled = false;
			}
			#endregion

			_performing = false;
			yield return 0;
		}
	}
}
