using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Enums;

namespace Server.Game
{
	public class NetEnvSystem : NetBaseComponentSystem<NetEnv>
	{
		public override void Reset()
		{
			var list = ComponentDict.AsEnumerable().ToArray();
			foreach (var env in list)
			{
				env.NetObj.Destroy();
			}

			foreach (var netObjData in World.Data.NetObjectDatas)
			{
				var obj = World.ObjectBuilder.GetNewObject(NetObjectType.Env_Wall)
					.SetPositionAndRotation(netObjData.Position, netObjData.Rotation);
				var collider = obj.GetComponent<NetBoxCollider2D>();
				collider.SetOffsetAndSize(netObjData.BoxCollider.Offset, netObjData.BoxCollider.Size);
			}
		}
	}
}
