using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyController : MonoBehaviour
{
	#region SerializeFields		
	[SerializeField] private BaseCharacter _currentPlayer;
	[SerializeField] private Transform _currentTarget;
	#endregion
	private void Update()
	{
		if (_currentPlayer == null || _currentPlayer.IsControllable == false)
		{
			return;
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
