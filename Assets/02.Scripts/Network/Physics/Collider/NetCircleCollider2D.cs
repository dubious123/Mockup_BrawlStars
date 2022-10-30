#if UNITY_EDITOR
using UnityEditor;

using UnityEngine;
#endif

public class NetCircleCollider2D : NetCollider2D
{
	[SerializeField] private float _radius;
	public sfloat Radius { get; private set; }
	public sfloat RadiusSquared { get; private set; }
	public sVector3 Center => NetObject.Position + (sVector3)Offset;

	public override void Init(INetObject obj)
	{
		base.Init(obj);
		Radius = (sfloat)_radius;
		RadiusSquared = Radius * Radius;
	}

	public override bool CheckCollision(NetCollider2D other)
	{
		return other switch
		{
			NetBoxCollider2D box => CheckBoxCircleCollision(box, this),
			NetCircleCollider2D circle => CheckCircleCircleCollision(this, circle),
			_ => false
		};
	}

	public override void DrawGizmo()
	{
		Handles.DrawWireDisc((Vector3)NetObject.Position + Offset, Vector3.up, _radius);
	}
}


