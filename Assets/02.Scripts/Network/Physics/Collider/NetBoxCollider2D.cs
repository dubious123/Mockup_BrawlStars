#if UNITY_EDITOR
using UnityEngine;
#endif

public class NetBoxCollider2D : NetCollider2D
{
	[SerializeField] private Vector2 _size;

	private sfloat _deltaX;
	private sfloat _deltaY;


	public sfloat MaxX => NetObject.Position.x + _deltaX;
	public sfloat MinX => NetObject.Position.x - _deltaX;
	public sfloat MaxY => NetObject.Position.z + _deltaY;
	public sfloat MinY => NetObject.Position.z - _deltaY;

	public override void Init(INetObject obj)
	{
		base.Init(obj);
		_deltaX = Offset.x + _size.x * (sfloat)0.5f;
		_deltaY = Offset.y + _size.y * (sfloat)0.5f;
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

	public override void DrawGizmo()
	{
		Gizmos.DrawWireCube((Vector3)NetObject.Position + Offset, new Vector3(_size.x, 1, _size.y));
	}
}

