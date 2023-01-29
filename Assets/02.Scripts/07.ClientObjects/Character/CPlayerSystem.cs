using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Server.Game;

using UnityEngine;

using static Enums;

public class CPlayerSystem : CBaseComponentSystem<NetCharacter>, IEnumerable<CPlayer>
{
	[SerializeField] private GameObject _shellyPrefab;
	private CPlayer[] _cPlayers;

	public override void Init(NetBaseComponentSystem<NetCharacter> netSystem)
	{
		_cPlayers = new CPlayer[Config.MAX_PLAYER_COUNT];
		foreach (var nPlayer in netSystem.ComponentDict)
		{
			_cPlayers[nPlayer.NetObjId.InstanceId] = Instantiate(_shellyPrefab, (Vector3)nPlayer.Position, (Quaternion)nPlayer.Rotation, transform).GetComponent<CPlayer>();
			_cPlayers[nPlayer.NetObjId.InstanceId].Init(nPlayer);
#if UNITY_EDITOR
			_cPlayers[nPlayer.NetObjId.InstanceId].gameObject.name = Enum.GetName(typeof(NetObjectType), nPlayer.NetObjId.Type) + nPlayer.NetObjId.InstanceId;
#endif
		}

		Camera.main.GetComponent<GameCameraController>().Init(_cPlayers[User.TeamId].transform);
		GameInput.SetGameInput(_cPlayers[User.TeamId].transform);
		GameInput.SetActive(true);
	}

	public override void OnNetFrameUpdate()
	{
		foreach (var cPlayer in _cPlayers)
		{
			if (cPlayer.Active)
			{
				cPlayer.OnNetFrameUpdate();
				cPlayer.Interpretate(0f);
			}
		}
	}

	public override void Interpretate(float ratio)
	{
		foreach (var cPlayer in _cPlayers)
		{
			if (cPlayer.Active)
			{
				cPlayer.Interpretate(ratio);
			}
		}
	}


	public override void OnRoundStart()
	{
		foreach (var cPlayer in _cPlayers)
		{
			cPlayer.OnRoundStart();
		}
	}

	public override void OnRoundEnd()
	{

	}

	public override void Clear()
	{
		foreach (var cPlayer in _cPlayers)
		{
			cPlayer.OnRoundClear();
		}
	}

	public override void Reset()
	{
		foreach (var cPlayer in _cPlayers)
		{
			cPlayer.Reset();
		}
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
