using System;
using System.Collections.Generic;

using static Enums;

public class NetPhysics2D
{
	private List<NetCollider2D>[] _colliders;

	public NetPhysics2D()
	{
		var length = Enum.GetValues(typeof(NetObjectTag)).Length;
		_colliders = new List<NetCollider2D>[length];
		for (int i = 0; i < length; i++)
		{
			_colliders[i] = new();
		}
	}

	public NetBoxCollider2D GetNewBoxCollider2D(INetObject obj, sVector2 offset, sVector2 size)
	{
		var res = new NetBoxCollider2D(obj, offset, size);
		_colliders[(int)obj.Tag].Add(res);
		return res;
	}

	public NetCircleCollider2D GetNewCircleCollider2D(INetObject obj, sVector2 offset, sfloat radius)
	{
		var res = new NetCircleCollider2D(obj, offset, radius);
		_colliders[(int)obj.Tag].Add(res);
		return res;
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


