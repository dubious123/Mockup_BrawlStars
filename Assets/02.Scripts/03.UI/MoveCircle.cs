using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MoveCircle : MonoBehaviour
{
	[SerializeField] private CPlayer _player;

	private void Update()
	{
		transform.position = _player.transform.position + (Vector3)_player.MoveDir;
	}
}
