using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyController : MonoBehaviour
{
	#region SerializeFields		
	[SerializeField] private BaseCharacter _currentPlayer;
	[SerializeField] private Transform _currentTarget;
	#endregion

	public bool _click = false;
	public float _clickCooldown = 0;
	private void Update()
	{
		if (_currentPlayer == null || _currentPlayer.IsControllable == false)
		{
			return;
		}

		_clickCooldown += Time.deltaTime;
		if (_click == false && _clickCooldown > 2)
		{
			Debug.Log("Click");
			(_currentPlayer as Dog_Character).ChargeBash();
			_clickCooldown = 0;
			_click = true;
		}
		if (_click == true && _clickCooldown > 5)
		{
			Debug.Log("UnClick");
			(_currentPlayer as Dog_Character).ReleaseBash();
			_clickCooldown = 0;
			_click = false;
		}
		_currentPlayer.Move((_currentTarget.position - transform.position).normalized);
		#region Move

		#endregion
		#region Rotate
		_currentPlayer.Look((_currentTarget.position - transform.position).normalized);
		#endregion

		#region Attack
		if ((_currentTarget.position - transform.position).magnitude < 3)
			_currentPlayer.ActivateBasicAttack();
		if ((_currentTarget.position - transform.position).magnitude > 6)
			_currentPlayer.DeactivateBasicAttack();
		#endregion
	}
}
