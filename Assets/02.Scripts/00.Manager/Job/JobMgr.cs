using System;
using System.Collections.Concurrent;

using UnityEngine;

public class JobMgr : MonoBehaviour
{
	private static JobMgr _instance;
	private ConcurrentQueue<Action> _unityJobQueue;
	private ConcurrentDictionary<string, JobQueue> _jobDict = new();

	public static void Init()
	{
		_instance = GameObject.Find("@JobMgr").GetComponent<JobMgr>();
		_instance._unityJobQueue = new ConcurrentQueue<Action>();

		//CreatejobQueue("PacketSend", 1, true, 1);
		//CreatejobQueue("PacketRecv", 1, true, 1);
		//CreatejobQueue("PacketHandler", 1, true, 1);
		//CreatejobQueue("PacketParser", 1, true, 1);
	}

	private void FixedUpdate()
	{
		while (_unityJobQueue?.Count > 0)
		{
			_unityJobQueue.TryDequeue(out Action job);
			job.Invoke();
		}
	}

	public static JobQueue CreatejobQueue(string name, int waitTick, bool startNow, int threadNum)
	{
		JobQueue queue = new(name, threadNum, waitTick);
		if (_instance._jobDict.TryGetValue(name, out _)) throw new Exception();
		_instance._jobDict[name] = queue;
		if (startNow)
			queue.Start();
		return queue;
	}

	public static void PushUnityJob(Action job)
	{
		_instance._unityJobQueue.Enqueue(job);
	}

	public static void Push(string name, Action job)
	{
		_instance._jobDict[name].Push(job);
	}

	public static JobQueue GetQueue(string name)
	{
		var queue = _instance._jobDict[name];
		Debug.Assert(queue is not null);
		return queue;
	}
}
