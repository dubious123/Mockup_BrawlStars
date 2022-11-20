using System;

public static partial class Enums
{
	public enum NetObjectTag
	{
		Character,
		Wall,
	}

	public enum NetObjectId
	{
		Wall,
		Player_Dog,
	}

	[Flags]
	public enum CCFlags
	{
		None = 0,
		Knockback = 1,
		Stun = 2,
	}

	public enum TeamType
	{
		None,
		Red,
		Blue,
	}
}


