using Server.Game;

using UnityEngine;

public class CPlayerShelly : CPlayer
{
	[field: SerializeField] public CShellyBuckShot Buckshot { get; set; }
	[field: SerializeField] public CShellySuperShell SuperShell { get; set; }

	public override void Init(NetCharacter character)
	{
		base.Init(character);
		Buckshot.Init(this);
		SuperShell.Init(this);
	}

	public override void OnNetFrameUpdate()
	{
		base.OnNetFrameUpdate();

		if (Now.IsBasicAttack)
		{
			Buckshot.HandleAttack();
		}
		if (Now.IsSpecialAttack)
		{
			SuperShell.HandleAttack();
		}
	}
}
