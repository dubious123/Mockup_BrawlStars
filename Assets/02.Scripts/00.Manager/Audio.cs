using System.Collections;
using System.Collections.Generic;

using MEC;

using Unity.VisualScripting;

using UnityEngine;

public class Audio : MonoBehaviour
{
	[SerializeField] private GameObject _audioSourcePrefab;
	private static Audio _instance;
	private Dictionary<string, AudioSource> _audioDict;

	public static void Init()
	{
		_instance = GameObject.Find("@Audio").GetComponent<Audio>();
	}

	public static void PlayOnce(AudioClip clip) => PlayOnce(clip, Vector3.zero);

	public static void PlayOnce(AudioClip clip, Vector3 position)
	{
		var source = Instantiate(_instance._audioSourcePrefab, _instance.transform).GetComponent<AudioSource>();
		source.transform.position = position;
		source.clip = clip;
		source.Play();
		Timing.CallDelayed(clip.length + 0.5f, () => Destroy(source));
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
		if (audio is not null)
		{
			_instance._audioDict.Remove(name);
			audio.Stop();
			Destroy(audio);
		}
	}

	public static void PlayAudio(AudioClip clip, string name, bool loop)
	{
		if (_instance._audioDict.ContainsKey(name) is false)
		{
			var audio = Instantiate(_instance._audioSourcePrefab).GetComponent<AudioSource>();
			audio.clip = clip;
			audio.loop = loop;
			_instance._audioDict.Add(name, audio);
			return;
		}

		_instance._audioDict[name].Play();
	}
}
