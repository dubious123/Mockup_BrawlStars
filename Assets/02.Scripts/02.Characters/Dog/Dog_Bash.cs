using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog_Bash : BaseAbility
{
	#region SerializeFields
	[SerializeField] float _maxBashlength;
	[SerializeField] float _chargingTimeLimit;
	[SerializeField] float _smoothSpeed;
	[SerializeField] float _bashSpeed;
	#endregion

	private float _currentBashLengh;
	private float _currentBashTime;
	private float _currentChargingTime;
	private float _expectedBashTime;

	private float _smoothVelocity;
	private bool _released;
	public override void Init(BaseCharacter character)
	{
		_character = character;
	}

	public void ChargeBash()
	{
		_released = false;
		_currentBashLengh = 0;
		_currentChargingTime = 0;
		_currentCoroutine = StartCoroutine(Perform());
	}
	public void ReleaseBash()
	{
		_released = true;
	}
	public void CancelBash()
	{

	}
	protected override IEnumerator Perform()
	{
		while (_currentChargingTime <= _chargingTimeLimit && _released == false)
		{
			_currentChargingTime += Time.deltaTime;
			_currentBashLengh = Mathf.SmoothDamp(_currentBashLengh, _maxBashlength, ref _smoothVelocity, _smoothSpeed);
			_currentBashLengh = Mathf.Min(_currentBashLengh, _maxBashlength);
			yield return new WaitForEndOfFrame();
		}

		_expectedBashTime = _currentBashLengh / _bashSpeed;
		_currentBashTime = 0;
		while (_currentBashTime <= _expectedBashTime)
		{
			_character.IsControllable = false;
			_currentBashTime += Time.deltaTime;
			_character.transform.Translate(_bashSpeed * Time.deltaTime * _character.LookDir, Space.World);
			yield return new WaitForEndOfFrame();
		}
		_character.IsControllable = true;
		yield break;
	}
}
