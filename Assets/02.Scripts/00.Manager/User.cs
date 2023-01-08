using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using static Enums;

public class User : MonoBehaviour
{
	public static NetObjectType CharType;
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
