
using static Enums;

public interface INetObject
{
	public sVector3 Position { get; set; }
	public sQuaternion Rotation { get; set; }
	public NetObjectTag Tag { get; set; }
}
