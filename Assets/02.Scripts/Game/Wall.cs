public class Wall : INetObject
{
	public sVector3 Position { get; set; }
	public sQuaternion Rotation { get; set; }
	public Enums.NetObjectTag Tag { get; set; } = Enums.NetObjectTag.Wall;
}
