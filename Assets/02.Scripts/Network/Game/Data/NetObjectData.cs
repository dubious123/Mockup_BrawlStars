namespace Server.Game.Data
{
	public class NetObjectData
	{
		public uint NetObjectId { get; init; }
		public sVector3 Position { get; init; }
		public sQuaternion Rotation { get; init; }
		public NetCollider2DData Collider { get; init; }
	}
}
