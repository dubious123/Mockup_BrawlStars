using UnityEngine;

using static Enums;

public class User : MonoBehaviour
{
	public static NetObjectType CharType = NetObjectType.Character_Spike;
	public static int UserId { get; set; }
	public static short TeamId { get; set; }
	public static TeamType Team
	{
		get;
		set;
	}

	public static void Init(S_Login res)
	{
		UserId = res.userId;
	}
}
