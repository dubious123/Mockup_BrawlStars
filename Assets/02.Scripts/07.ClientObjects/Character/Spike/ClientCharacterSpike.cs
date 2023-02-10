using Server.Game;

using UnityEngine;

public class ClientCharacterSpike : ClientCharacter
{
	[field: SerializeField] public CSpikeNeedleGranade NeedleGranade { get; set; }
	[field: SerializeField] public CSpikeStickAround StickAround { get; set; }

	public override void Init(NetCharacter character)
	{
		base.Init(character);
		NeedleGranade.Init(this);
		StickAround.Init(this);
	}

	public override void OnNetFrameUpdate()
	{
		base.OnNetFrameUpdate();

		if (Now.IsBasicAttack)
		{
			NeedleGranade.HandleAttack();
		}
		if (Now.IsSpecialAttack)
		{
			StickAround.HandleAttack();
		}
	}
}
