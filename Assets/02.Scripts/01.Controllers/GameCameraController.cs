using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCameraController : MonoBehaviour
{
	#region SerializeFields
	[SerializeField] private Vector3 _offSet;
	#endregion
	private Vector3 _targetPos;
	private Transform _followTarget;
	public Transform FollowTarget { set { _followTarget = value; } }
	private void Update()
	{
		if (_followTarget == null) return;
		_targetPos = _followTarget.position + _offSet;
		transform.position = _targetPos;
	}
}
