using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Scene_Loading_Game : BaseScene
{
	[SerializeField] private ProgressBar _progressBar;
	[SerializeField] private AudioSource _bgm;

	public override void Init(object param)
	{

	}

	private void Start()
	{
		_bgm.Play();
		StartCoroutine(CoUpdateProgress());
	}

	private IEnumerator CoUpdateProgress()
	{
		for (int i = 10; i < 101; i++)
		{
			_progressBar.UpdateProgress(i);
			yield return new WaitForSeconds(0.1f);
		}
	}
}
