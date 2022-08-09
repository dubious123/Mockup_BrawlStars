using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gizmo_Tool : MonoBehaviour
{
	#region Serialize Fields
	[SerializeField] private bool _enableDrawSphere;
	[SerializeField] private Color _idleColor;
	[SerializeField] private Color _activatedColor;
	[SerializeField] private float _shpereRadius;
	#endregion

	private Color _currentColor;
	private void OnDrawGizmos()
	{
		if (_enableDrawSphere)
		{
			Gizmos.color = _currentColor;
			Gizmos.DrawSphere(transform.position, _shpereRadius);
		}
	}
	private void Update()
	{
		var hits = Physics.OverlapSphere(transform.position, _shpereRadius, LayerMeta.Character_Opponent);
		_currentColor = hits.Length > 0 ? _activatedColor : _idleColor;
	}
}
