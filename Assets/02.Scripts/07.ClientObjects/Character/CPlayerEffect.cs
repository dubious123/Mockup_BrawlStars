using UnityEngine;

public class CPlayerEffect : MonoBehaviour
{
	[SerializeField] private ParticleSystem _playerDeadEffectBlue;
	[SerializeField] private ParticleSystem _playerDeadEffectRed;
	[SerializeField] private ParticleSystem _specialAttackEffect;
	[SerializeField] private ParticleSystem _chargeEffect;

	private void Update()
	{
		var direction = (Camera.main.transform.position - transform.position).normalized;
		_playerDeadEffectBlue.transform.parent.position = transform.position + direction * 3;
	}

	public void PlayeDeadEffect(bool isTeam)
	{
		if (isTeam)
		{
			Play(_playerDeadEffectBlue);
		}
		else
		{
			Play(_playerDeadEffectRed);
		}
	}

	public void PlaySpecialAttackEffect()
	{
		Play(_specialAttackEffect);
	}

	public void PlayChargeEffect()
	{
		Play(_chargeEffect);
	}

	private void Play(ParticleSystem effect)
	{
		effect.gameObject.SetActive(true);
		effect.Play(true);
	}
}
