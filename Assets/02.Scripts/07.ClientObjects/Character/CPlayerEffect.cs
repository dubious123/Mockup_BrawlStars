using UnityEngine;

public class CPlayerEffect : MonoBehaviour
{
	[SerializeField] private ParticleSystem _playerDeadEffect;
	[SerializeField] private ParticleSystem _specialAttackEffect;
	[SerializeField] private ParticleSystem _chargeEffect;

	private void Update()
	{
		var direction = (Camera.main.transform.position - transform.position).normalized;
		_playerDeadEffect.gameObject.transform.position = transform.position + direction * 3;
	}

	public void PlayeDeadEffect()
	{
		Play(_playerDeadEffect);
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
