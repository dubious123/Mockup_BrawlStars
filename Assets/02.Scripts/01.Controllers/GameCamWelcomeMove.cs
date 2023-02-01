using System.Collections.Generic;

using MEC;

using UnityEngine;

public class GameCamWelcomeMove : MonoBehaviour
{
	[SerializeField] private Transform _startTarget;
	[SerializeField] private Transform _endTarget;
	[SerializeField] private float _verticalTravelTime;
	[SerializeField] private float _zoomTime;
	[SerializeField] private float _startDist;
	[SerializeField] private float _targetDist;
	[SerializeField] private float _angle;
	[SerializeField] private GameCameraController _controller;

	private void Start()
	{
		if (User.Team == Enums.TeamType.Red)
		{
			(_endTarget, _startTarget, _angle) = (_startTarget, _endTarget, 180 - _angle);
			var rot = transform.rotation.eulerAngles;
			rot.y = 180;
			transform.rotation = Quaternion.Euler(rot);
		}

		Timing.RunCoroutine(CoRunWelcomeMove());
	}

	private IEnumerator<float> CoRunWelcomeMove()
	{
		var rad = Mathf.Deg2Rad * _angle;
		var deltaPos = new Vector3(0, _startDist * Mathf.Sin(rad), -_startDist * Mathf.Cos(rad));
		var startPos = _startTarget.position + deltaPos;
		var endPos = _endTarget.position + deltaPos;
		transform.position = startPos;
		for (float delta = 0; delta < _verticalTravelTime; delta += Time.deltaTime)
		{
			transform.position = Vector3.Lerp(startPos, endPos, delta / _verticalTravelTime);
			yield return Timing.WaitForOneFrame;
		}

		startPos = endPos;
		endPos = _endTarget.position + new Vector3(0, _targetDist * Mathf.Sin(rad), -_targetDist * Mathf.Cos(rad));
		transform.position = startPos;
		for (float delta = 0; delta < _zoomTime; delta += Time.deltaTime)
		{
			transform.position = Vector3.Lerp(startPos, endPos, delta / _zoomTime);
			yield return Timing.WaitForOneFrame;
		}

		transform.position = endPos;
		_controller.enabled = true;
		enabled = false;
		yield break;
	}
}
