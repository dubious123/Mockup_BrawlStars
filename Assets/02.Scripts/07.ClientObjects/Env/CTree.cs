using System.Collections.Generic;

using MEC;

using Server.Game;

using UnityEngine;

public class CTree : CEnv
{
	[SerializeField] private CEnvFade _effect;
	[SerializeField] private Transform _target;
	[SerializeField] private float _shakePeriod, _shakeAmplitude;
	[SerializeField] private int _shakeTime;
	private NetTree _netTree;
	private int _userCount;

	public void FadeIn() => _effect.FadeIn();
	public void FadeOut() => _effect.FadeOut();

	public override void Init(NetEnv tree)
	{
		_netTree = tree as NetTree;
		if (_netTree is null)
		{
			throw new System.Exception();
		}
	}

	public void Shake()
	{
		Timing.RunCoroutine(CoShake());
	}

	public void OnCharacterEnter(NetCharacter character)
	{
		Shake();
		if (character.Team != User.Team)
		{
			return;
		}

		if (_userCount++ == 0)
		{
			FadeOut();
		}
	}

	public void OnCharacterExit(NetCharacter character)
	{
		if (character.Team != User.Team)
		{
			return;
		}

		if (--_userCount == 0)
		{
			FadeIn();
		}
	}

	public override void Reset()
	{
		if (_userCount != 0)
		{
			_userCount = 0;
			FadeIn();
		}
	}

	private IEnumerator<float> CoShake()
	{
		for (int i = 0; i < _shakeTime; i++)
		{
			for (float delta = 0f; delta < _shakePeriod; delta += Time.deltaTime)
			{
				var t = delta / _shakePeriod - 1;
				var rotationZ = Mathf.Sin(2 * Mathf.PI * (Mathf.Pow(t, 3) + 1)) * _shakeAmplitude;
				_target.rotation = Quaternion.Euler(0, 0, rotationZ);
				yield return 0f;
			}
		}

		_target.rotation = Quaternion.identity;
		yield break;
	}
}
