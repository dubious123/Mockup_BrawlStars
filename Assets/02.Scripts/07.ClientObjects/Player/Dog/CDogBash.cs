using System.Collections.Generic;

using Server.Game;

using TMPro;

using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CDogBash : MonoBehaviour, ICBaseSkill
{
	public bool Performing { get; set; }
	public bool Active { get; set; }
	public CPlayerDog Player { get; set; }
	public NetDogBash NetBash { get; set; }

	[SerializeField] private RawImage _skillIndicator;
	private TextMeshProUGUI _coolTimeIndicator;
	private IEnumerator<int> _coHandler;
	private int _holdFrame;

	public void Init(CPlayerDog player)
	{
		Player = player;
		NetBash = (player.NPlayer as NetCharacterDog).Bash as NetDogBash;
		_coolTimeIndicator = GameObject.Find("Cool Time").GetComponent<TextMeshProUGUI>();
		_coolTimeIndicator.enabled = false;
		_coHandler = Co_Perform();
	}

	public void HandleOneFrame()
	{
		if (NetBash.Performing)
		{
			_coHandler.MoveNext();
		}
	}

	public IEnumerator<int> Co_Perform()
	{
		while (true)
		{
			_skillIndicator.enabled = true;
			_skillIndicator.rectTransform.sizeDelta = new Vector2(100, 0);
			for (_holdFrame = 0; NetBash.Holding; _holdFrame++)
			{
				_skillIndicator.rectTransform.sizeDelta = new Vector2(100, (float)NetBash.MaxBashDistance * ((float)_holdFrame / NetBash.MaxHoldingFrame) * 100);
				yield return 0;
			}

			_skillIndicator.enabled = false;
			Player.Animator.SetBool(AnimatorMeta.Dog_Bash, true);
			for (int i = 0; i < NetBash.BashFrame; i++)
			{
				yield return 0;
			}

			Player.Animator.SetBool(AnimatorMeta.Dog_Bash, false);
			_coolTimeIndicator.enabled = true;
			for (int i = NetBash.CoolTimeFrame; 0 < i; i--)
			{
				_coolTimeIndicator.text = (i * Time.fixedDeltaTime).ToString("0.0");
				yield return 0;
			}

			_coolTimeIndicator.enabled = false;
			yield return 0;
		}
	}
}
