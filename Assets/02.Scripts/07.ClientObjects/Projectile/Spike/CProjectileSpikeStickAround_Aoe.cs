using System.Collections.Generic;

using MEC;

using Server.Game;

using Unity.VisualScripting;

using UnityEngine;

public class CProjectileSpikeStickAround_Aoe : CProjectile
{
	[SerializeField] private float _fadeTime;
	[SerializeField] private Color _colBlue;
	[SerializeField] private Color _colRed;
	[SerializeField] private Renderer _renderer;
	[SerializeField] private Transform _spikeParentTrans;
	[SerializeField] private Transform[] _spikes;
	[SerializeField] private Vector3[] _resetInfo;
	[SerializeField] private Sprite[] _blue;
	[SerializeField] private Sprite[] _red;

	public override void Init(NetProjectile projectile)
	{
		base.Init(projectile);
		(var color, var sprites) = User.Team == projectile.Owner.GetComponent<NetCharacter>().Team ?
			(_colBlue, _blue) : (_colRed, _red);
		_renderer.material.SetColor("_MainColor", color);
		foreach (var renderer in Renderers)
		{
			renderer.sprite = sprites[renderer.name[4] - '0'];
		}

		transform.rotation = User.Team == Enums.TeamType.Blue ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
	}

	public override void Interpretate(float ratio)
	{
		if (Now.Active is false)
		{
			DeActivate();
			return;
		}

		var pos = Vector3.Lerp(Now.Position, Next.Position, ratio);
		transform.position = new Vector3(pos.x, 0.2f, pos.z);
	}

	protected override void Activate()
	{
		transform.position = Now.Position;
		Reset();
		BulletGo.SetActive(true);
		Timing.CallDelayed(0.25f, () => Timing.RunCoroutine(CoSpikeAnim(), "CoSpikeAnim"));
		Active = true;
	}

	protected override void DeActivate()
	{
		if (Active is false)
		{
			return;
		}

		Active = false;
		Timing.RunCoroutine(CoDeActivate());
	}

	private IEnumerator<float> CoDeActivate()
	{
		Timing.KillCoroutines("CoPointAnim");
		Timing.KillCoroutines("CoSpikeAnim");
		foreach (var spike in _spikes)
		{
			Timing.RunCoroutine(CoShrinkAnim(spike), "CoShrinkAnim");
		}

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
		Timing.KillCoroutines("CoShrinkAnim");
		System.External_Return(this);
		yield break;
	}

	private void Reset()
	{
		for (int i = _spikes.Length - 1; i >= 0; --i)
		{
			_spikes[i].localScale = _resetInfo[i];
			_spikes[i].gameObject.SetActive(true);
		}
	}

	private IEnumerator<float> CoSpikeAnim()
	{
		while (true)
		{
			for (int i = _spikes.Length - 1; i >= 0;)
			{
				Timing.RunCoroutine(CoPointAnim(_spikes[i--]), "CoPointAnim");
				if (i >= 0)
				{
					Timing.RunCoroutine(CoPointAnim(_spikes[i--]), "CoPointAnim");
				}

				yield return Timing.WaitForSeconds(0.1f);
			}

			yield return Timing.WaitForSeconds(0.6f);
		}
	}

	private IEnumerator<float> CoPointAnim(Transform spikeTrans)
	{
		var targetScale = spikeTrans.localScale;
		var startScale = spikeTrans.localScale * 1.3f;
		for (float deltaTime = 0; deltaTime < 0.5f; deltaTime += Time.deltaTime)
		{
			spikeTrans.localScale = Vector3.Lerp(startScale, targetScale, deltaTime * 2);
			yield return 0f;
		}

		spikeTrans.localScale = targetScale;
	}

	private IEnumerator<float> CoShrinkAnim(Transform spikeTrans)
	{
		var startScale = spikeTrans.localScale;
		for (float deltaTime = 0; deltaTime < 0.5f; deltaTime += Time.deltaTime)
		{
			spikeTrans.localScale = Vector3.Lerp(startScale, Vector3.zero, deltaTime * 2);
			yield return 0f;
		}

		spikeTrans.gameObject.SetActive(false);
	}

#if UNITY_EDITOR
	public void GenerateResetData()
	{
		_resetInfo = new Vector3[_spikes.Length];
		for (int i = 0; i < _spikes.Length; ++i)
		{
			_resetInfo[i] = _spikes[i].localScale;
		}
	}
#endif
}
