using UnityEngine;

using static Enums;

public class Data : MonoBehaviour
{
	[SerializeField] private Sprite[] _characterProfiles;

	private static Data _instance;

	public static void Init()
	{
		_instance = GameObject.Find("@Data").GetComponent<Data>();
	}

	public static Sprite GetCharacterProfile(NetObjectType characterType)
	{
		return _instance._characterProfiles[characterType - NetObjectType.Character_Shelly];
	}

	public static Sprite GetCharacterProfile(int characterIndex)
	{
		return _instance._characterProfiles[characterIndex];
	}
}
