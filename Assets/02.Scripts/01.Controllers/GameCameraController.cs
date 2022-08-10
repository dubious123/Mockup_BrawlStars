using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCameraController : MonoBehaviour
{
	#region SerializeFields
	[SerializeField] private Transform _followTarget;
	[SerializeField] private Vector3 _offSet;
	#endregion
	private Vector3 _targetPos;

	private void Update()
	{
		_targetPos = _followTarget.position + _offSet;
		transform.position = _targetPos;
	}
}
