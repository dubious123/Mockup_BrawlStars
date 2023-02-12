using System.Collections.Generic;

using MEC;

using UnityEngine;

public class Audio : MonoBehaviour
{
	[SerializeField] private GameObject _audioSourcePrefab;
	[SerializeField] private AudioClip _btnPressedNormal;
	private static Audio _instance;
	private Dictionary<string, AudioSource> _audioDict;

	public static void Init()
	{
		_instance = GameObject.Find("@Audio").GetComponent<Audio>();
		_instance._audioDict = new();
	}

	public static void PlayOnce(AudioClip clip, float volume = 1f) => PlayOnce(clip, Vector3.zero, volume);

	public static void PlayOnce(AudioClip clip, Vector3 position, float volume = 1f)
	{
		var source = Instantiate(_instance._audioSourcePrefab, _instance.transform).GetComponent<AudioSource>();
		source.transform.position = position;
		source.clip = clip;
		source.volume = volume;
		source.Play();
		Timing.CallDelayed(clip.length + 0.5f, () => Destroy(source.gameObject));
	}

	public static void PauseAudio(string name)
	{
		var audio = _instance._audioDict[name];
		if (audio is not null)
		{
			audio.Pause();
		}
	}

	public static void ResumeAudio(string name)
	{

	}

	public static void StopAudio(string name)
	{
		var audio = _instance._audioDict[name];
		if (audio is null)
		{
			return;
		}

		_instance._audioDict.Remove(name);
		audio.Stop();
		Destroy(audio.gameObject);
	}

	public static void StopAudio(string name, float fadeTime)
	{
		var audio = _instance._audioDict[name];
		if (audio is null)
		{
			return;
		}

		Timing.RunCoroutine(CoInternalFade(audio, name, fadeTime));

		static IEnumerator<float> CoInternalFade(AudioSource audio, string name, float fadeTime)
		{
			for (float delta = 0; delta < fadeTime; delta += Time.deltaTime)
			{
				audio.volume = Mathf.Lerp(1, 0, delta / fadeTime);
				yield return 0f;
			}

			_instance._audioDict.Remove(name);
			audio.Stop();
			Destroy(audio.gameObject);
		}
	}

	public static void PlayAudio(AudioClip clip, string name, bool loop, bool allowMany = false)
	{
		if (_instance._audioDict.TryGetValue(name, out var audio))
		{
			if (allowMany is false && audio.isPlaying is false)
			{
				audio.Play();
			}

			return;
		}

		audio = Instantiate(_instance._audioSourcePrefab, _instance.transform).GetComponent<AudioSource>();
		audio.clip = clip;
		audio.loop = loop;
		_instance._audioDict.Add(name, audio);
		audio.Play();
	}

	public static void PlayBtnPressedNormal()
	{
		PlayAudio(_instance._btnPressedNormal, _instance._btnPressedNormal.name, false);
	}
}
