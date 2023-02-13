using System.Collections.Generic;

using MEC;

using UnityEngine;

public class GameCameraController : MonoBehaviour
{
	#region SerializeFields
	[SerializeField] private Vector3 _offSet;
	[SerializeField] private float _zoomOutFactor;
	[SerializeField] private float _zoomOutTime;
	#endregion
	public Transform FollowTarget { set { _followTarget = value; } }

	private Transform _followTarget;
	private Vector3 _shakeDelta;

	private void Update()
	{
		if (_followTarget is null) return;
		transform.position = new Vector3(0, _followTarget.position.y + _offSet.y, _followTarget.position.z + _offSet.z) + _shakeDelta;
	}

	public void Init(Transform target)
	{
		_followTarget = target;
		_shakeDelta = Vector3.zero;
		if (User.Team == Enums.TeamType.Red)
		{
			_offSet = new Vector3(_offSet.x, _offSet.y, -_offSet.z);
		}
	}

	public void Shake()
	{
		Timing.KillCoroutines("Internal_CoShake");
		Timing.RunCoroutine(Internal_CoShake(1, 0.5f), "Internal_CoShake");

		IEnumerator<float> Internal_CoShake(float amount, float duration)
		{
			for (float delta = 0; delta < duration; delta += Time.deltaTime)
			{
				_shakeDelta = Random.insideUnitSphere * amount;
				yield return Timing.WaitForOneFrame;
			}

			yield break;
		}
	}

	public void OnGameEnd()
	{
		enabled = false;
		Timing.RunCoroutine(CoOnGameEnd());
	}

	private IEnumerator<float> CoOnGameEnd()
	{
		var targetPos = _followTarget.position + _offSet * _zoomOutFactor;
		targetPos.x = 0;
		for (float delta = 0; delta < _zoomOutTime; delta += Time.deltaTime)
		{
			transform.position = Vector3.Lerp(transform.position, targetPos, delta / _zoomOutTime);
			yield return Timing.WaitForOneFrame;
		}

		transform.position = targetPos;
		yield break;
	}
}