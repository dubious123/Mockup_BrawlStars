using System;
using System.Drawing;

using UnityEngine.UIElements;

public class NetBoxCollider2D : NetCollider2D
{
	public sfloat MaxX => NetObject.Position.x + _deltaX;
	public sfloat MinX => NetObject.Position.x - _deltaX;
	public sfloat MaxY => NetObject.Position.z + _deltaY;
	public sfloat MinY => NetObject.Position.z - _deltaY;

	private readonly sfloat _deltaX;
	private readonly sfloat _deltaY;

	public NetBoxCollider2D(INetObject obj, sVector2 offset, sVector2 size) : base(obj, offset)
	{
		var deltaX = size.x * (sfloat)0.5f;
		var deltaY = size.y * (sfloat)0.5f;
		_deltaX = offset.x + deltaX;
		_deltaY = offset.y + deltaY;
	}

	public override bool CheckCollision(NetCollider2D other)
	{
		return other switch
		{
			NetBoxCollider2D box => CheckBoxBoxCollision(this, box),
			NetCircleCollider2D circle => CheckBoxCircleCollision(this, circle),
			_ => false
		};
	}
}

