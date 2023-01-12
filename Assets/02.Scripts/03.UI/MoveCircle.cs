using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MoveCircle : MonoBehaviour
{
	[SerializeField] private CPlayer _player;
	[SerializeField] private Transform _indicatorAnchor;

	private void Update()
	{
		transform.position = _indicatorAnchor.position + (Vector3)_player.MoveDir;
	}
}
