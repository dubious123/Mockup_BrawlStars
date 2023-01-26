using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Server.Game;

using UnityEngine;

using static Enums;

public class CPlayerSystem : CBaseComponentSystem<NetCharacter>, IEnumerable<CPlayer>
{
	[SerializeField] private CPlayer _shellyPrefab;
	private CPlayer[] _cPlayers;
	private NetCharacterSystem _netSystem;

	public override void Init(NetBaseComponentSystem<NetCharacter> netSystem)
	{
		_netSystem = netSystem as NetCharacterSystem;
		_cPlayers = new CPlayer[Config.MAX_PLAYER_COUNT];
		foreach (var nPlayer in netSystem.ComponentDict)
		{
			_cPlayers[nPlayer.NetObjId.InstanceId] = Instantiate(_shellyPrefab, (Vector3)nPlayer.Position, (Quaternion)nPlayer.Rotation, transform).GetComponent<CPlayer>();
			_cPlayers[nPlayer.NetObjId.InstanceId].Init(nPlayer);
#if UNITY_EDITOR
			_cPlayers[nPlayer.NetObjId.InstanceId].gameObject.name = Enum.GetName(typeof(NetObjectType), nPlayer.NetObjId.Type) + nPlayer.NetObjId.InstanceId;
#endif
			if (nPlayer.NetObjId.InstanceId == User.TeamId)
			{
				Camera.main.GetComponent<GameCameraController>().Init(_cPlayers[nPlayer.NetObjId.InstanceId].transform);
			}
		}
	}

	public override void MoveClientLoop()
	{
		//todo
	}

	public override void Reset()
	{
		//foreach (var cPlayer in _cPlayers)
		//{
		//	cPlayer.OnRoundReset();
		//}
	}

	public IEnumerator<CPlayer> GetEnumerator()
	{
		return _cPlayers.Cast<CPlayer>().GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
