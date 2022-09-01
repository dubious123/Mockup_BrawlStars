using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class PacketQueue : MonoBehaviour
{
	static PacketQueue _instance;
	ConcurrentQueue<Action> _packetQueue;
	void Start()
	{
		_packetQueue = new ConcurrentQueue<Action>();
		_instance = GameObject.Find("@PacketQueue").GetComponent<PacketQueue>();
	}

	void Update()
	{
		while (_packetQueue.Count > 0)
		{
			_packetQueue.TryDequeue(out Action job);
			job.Invoke();
		}
	}

	public static void Push(Action job)
	{
		_instance._packetQueue.Enqueue(job);
	}
}
