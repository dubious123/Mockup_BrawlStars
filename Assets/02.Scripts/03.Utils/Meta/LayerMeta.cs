using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerMeta
{
	public readonly static int Default = 1 << 0;
	public readonly static int UI = 1 << 1;
	public readonly static int Floor = 1 << 6;

	public readonly static int Character_Self = 1 << 7;
	public readonly static int Character_Ally = 1 << 10;
	public readonly static int Character_Opponent = 1 << 11;

}
