using System.Collections.Generic;

using MEC;

using Server.Game;

using UnityEngine;

public class CProjectileSpikeStickAround_Aoe : CProjectile
{
	[SerializeField] private float _fadeTime;
	[SerializeField] private Color _colBlue;
	[SerializeField] private Color _colRed;
	[SerializeField] private Renderer _renderer;

	public override void Init(NetProjectile projectile)
	{
		base.Init(projectile);
		_renderer.material.SetColor("_MainColor", User.Team == projectile.Owner.GetComponent<NetCharacter>().Team ? _colBlue : _colRed);
	}

	protected override void Activate()
	{
		transform.position = Now.Position;
		BulletGo.SetActive(true);
		Active = true;
	}

	protected override void DeActivate()
	{
		Active = false;
		System.External_Return(this);
		Timing.RunCoroutine(CoDeActivate());
	}

	private IEnumerator<float> CoDeActivate()
	{
		var backGroundAlpha = _renderer.material.GetFloat("_BackgroundAlpha");
		var ringTh = _renderer.material.GetFloat("_RingTh");
		for (float deltaTime = 0; deltaTime < _fadeTime; deltaTime += Time.deltaTime)
		{
			var ratio = deltaTime / _fadeTime;
			_renderer.material.SetFloat("_BackgroundAlpha", Mathf.Lerp(backGroundAlpha, 0, ratio));
			_renderer.material.SetFloat("_RingTh", Mathf.Lerp(ringTh, 0, ratio));
			yield return 0f;
		}

		BulletGo.SetActive(false);
		_renderer.material.SetFloat("_BackgroundAlpha", backGroundAlpha);
		_renderer.material.SetFloat("_RingTh", ringTh);
		yield break;
	}
}
