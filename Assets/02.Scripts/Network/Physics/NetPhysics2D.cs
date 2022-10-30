using System;
using System.Collections.Generic;

using UnityEngine;

using static Enums;

public class NetPhysics2D : MonoBehaviour
{
	private List<NetCollider2D>[] _colliders;

	public void Init()
	{
		var length = Enum.GetValues(typeof(NetObjectTag)).Length;
		_colliders = new List<NetCollider2D>[length];
		for (int i = 0; i < length; i++)
		{
			_colliders[i] = new();
		}

		var colliders = transform.GetComponentsInChildren<NetCollider2D>();
		foreach (var collider in colliders)
		{
			_colliders[(int)collider.NetObject.Tag].Add(collider);
		}
	}

	public void RegisterNetCollider(NetCollider2D collider)
	{
		_colliders[(int)collider.NetObject.Tag].Add(collider);
	}

	public bool DetectCollision(NetCollider2D collider, NetObjectTag tag)
	{
		foreach (var c in _colliders[(int)tag])
		{
			if (c != collider && collider.CheckCollision(c))
			{
				return true;
			}
		}

		return false;
	}

	public void GetCollisions(NetCollider2D collider, NetObjectTag tag, IList<NetCollision2D> collisions)
	{
		foreach (var c in _colliders[(int)tag])
		{
			if (c == collider)
			{
				continue;
			}

			var collision = collider.GetCollision(c);
			if (collision is not null)
			{
				collisions.Add(collision);
			}
		}
	}

	public void Clear()
	{
		foreach (var list in _colliders)
		{
			list.Clear();
		}
	}
}


