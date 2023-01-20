using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class CPlayerEffect : MonoBehaviour
{
	[SerializeField] private ParticleSystem _playerDeadEffect;

	private void Update()
	{
		var direction = (Camera.main.transform.position - transform.position).normalized;
		_playerDeadEffect.gameObject.transform.position = transform.position + direction * 3;
	}

	public void PlayeDeadEffect()
	{
		_playerDeadEffect.gameObject.SetActive(true);
		_playerDeadEffect.Play(true);
	}
}
