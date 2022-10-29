
public class NetCircleCollider2D : NetCollider2D
{
	public sfloat Radius { get; init; }
	public sfloat RadiusSquared { get; init; }
	public sVector3 Center => NetObject.Position;
	public NetCircleCollider2D(INetObject obj, sVector2 offset, sfloat radius) : base(obj, offset)
	{
		Radius = radius;
		RadiusSquared = radius * radius;
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
}


