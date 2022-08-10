using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent_Tool : MonoBehaviour
{
	private Dictionary<int, Action> _eventDict = new Dictionary<int, Action>();
	private static int _idx = 0;
	public void AssignEvent(AnimationClip clip, float time, Action action)
	{
		_eventDict.Add(_idx, action);
		var animEvent = new AnimationEvent();
		{
			animEvent.time = time;
			animEvent.functionName = "InvokeEvent";
			animEvent.intParameter = _idx;
		}
		clip.AddEvent(animEvent);
		_idx++;
	}
	private void InvokeEvent(int idx)
	{
		switch (idx)
		{
			case 0:
				Debug.Log("start");
				break;
			case 1:
				Debug.Log("perform");

				break;
			case 2:
				Debug.Log("end");

				break;
		}
		_eventDict[idx].Invoke();
	}
}
