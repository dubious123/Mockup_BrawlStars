using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class LayerMeta
{
	public static readonly int Default = 1 << 0;
	public static readonly int UI = 1 << 1;
	public static readonly int Env = 1 << 19;

	public static readonly int Character_Self = 1 << 7;
	public static readonly int Character_Ally = 1 << 10;
	public static readonly int Character_Opponent = 1 << 11;

}
