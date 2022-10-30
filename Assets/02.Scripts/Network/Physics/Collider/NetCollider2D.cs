using System;
using System.Collections.Generic;

using UnityEngine;


public abstract class NetCollider2D : MonoBehaviour
{
	[field: SerializeField] protected Vector3 Offset { get; set; }
	public INetObject NetObject { get; protected set; }

	public virtual void Init(INetObject obj)
	{
		NetObject = obj;
	}

	public static bool CheckBoxBoxCollision(NetBoxCollider2D left, NetBoxCollider2D right)
	{
		return
			left.MinX <= right.MaxX &&
			left.MaxX >= right.MinX &&
			left.MinY <= right.MaxY &&
			left.MaxY >= right.MinY;
	}

	public static bool CheckBoxCircleCollision(NetBoxCollider2D box, NetCircleCollider2D circle)
	{
		var x = sMathf.Max(box.MinX, sMathf.Min(circle.Center.x, box.MaxX));
		var y = sMathf.Max(box.MinY, sMathf.Min(circle.Center.z, box.MaxY));
		return
			(x - circle.Center.x) * (x - circle.Center.x) +
			(y - circle.Center.z) * (y - circle.Center.z) <= circle.RadiusSquared;
	}

	public static bool CheckCircleCircleCollision(NetCircleCollider2D left, NetCircleCollider2D right)
	{
		var deltaX = left.Center.x - right.Center.x;
		var deltaY = left.Center.y - right.Center.y;
		var dist = left.Radius + right.Radius;
		return deltaX * deltaX + deltaY * deltaY <= dist * dist;
	}

	public NetCollision2D GetCollision(NetCollider2D other)
	{
		return CheckCollision(other) ? new NetCollision2D { Collider = other } : null;
	}

	public abstract bool CheckCollision(NetCollider2D other);

	public abstract void DrawGizmo();
}


