using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GameCameraController : MonoBehaviour
{
	#region SerializeFields
	[SerializeField] private Vector3 _offSet;
	#endregion
	private Transform _followTarget;
	public Transform FollowTarget { set { _followTarget = value; } }
	private void Update()
	{
		if (_followTarget is null) return;
		transform.position = new Vector3(0, _followTarget.position.y + _offSet.y, _followTarget.position.z + _offSet.z);
	}

	public void Init(Transform target)
	{
		_followTarget = target;
	}
}
