using ServerCore.Packets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class User : MonoBehaviour
{
	public static CharacterType CharType;
	public static int UserId;
	public static int GameRoomId;
	public static short TeamId;
	public static void Init(S_Login res)
	{
		UserId = res.userId;
	}
}
