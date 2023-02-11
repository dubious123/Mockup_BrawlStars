using UnityEngine;

using static Enums;

public class Data : MonoBehaviour
{
	[SerializeField] private Sprite[] _characterProfiles;
	[SerializeField] private GameObject[] _characterPrefab;
	[SerializeField] private GameObject[] _projectilePrefab;

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

	public static GameObject GetCharacterGamePrefab(NetObjectType characterType)
	{
		return _instance._characterPrefab[characterType - NetObjectType.Character_Shelly];
	}

	public static GameObject GetCharacterGamePrefab(int index)
	{
		return _instance._characterPrefab[index];
	}

	public static GameObject GetProjectilePrefab(NetObjectType projectiletype)
	{
		return _instance._projectilePrefab[projectiletype - NetObjectType.Projectile_Shelly_Buckshot];
	}

	public static GameObject GetProjectilePrefab(int index)
	{
		return _instance._projectilePrefab[index];
	}

	public static string GetCharacterName(NetObjectType characterType)
	{
		return characterType switch
		{
			NetObjectType.Character_Shelly => "Shelly",
			NetObjectType.Character_Spike => "Spike",
			_ => null,
		};
	}
}
