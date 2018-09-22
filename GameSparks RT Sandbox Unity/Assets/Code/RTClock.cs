using System;
using System.Collections;
using System.Collections.Generic;
using GameSparks.RT;
using UnityEngine;

public class RTClock : MonoBehaviour
{
	[SerializeField]
	private float m_sendTimeStampRatioInSeconds = 5f;
	
	public static RTClock Instance { get; private set; }

	private long timeDelta;
	private long latency;
	private long roundTrip;
	
	public DateTime ServerClock { get; private set; }
	
	public long TimeDelta
	{
		get { return timeDelta; }
		set { timeDelta = value; }
	}

	public long Latency
	{
		get { return latency; }
		set { latency = value; }
	}

	public long RoundTrip
	{
		get { return roundTrip; }
		set { roundTrip = value; }
	}

	private void Awake()
	{
		Instance = this;
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	private void Start()
	{
		StartCoroutine(SendClockTimeStamp());
	}
	
	IEnumerator SendClockTimeStamp()
	{
		GameSparksRTUnity rtSession = MyGameSparksManager.Instance().GetRTUnitySession();
		PacketsFactory.SendTimeStamp(rtSession);
		
		yield return new WaitForSeconds(m_sendTimeStampRatioInSeconds);
		StartCoroutine(SendClockTimeStamp());
	}

	public void ProcessTimeStampPacket(RTPacket rtPacket)
	{
		long originalUnixTime = rtPacket.Data.GetLong(1).Value;
		long now = PacketsFactory.GetUnixTime();
		roundTrip = now - originalUnixTime;

		latency = (long)(roundTrip / 2);

		long serverUnixTime = rtPacket.Data.GetLong(2).Value;
		long serverDelta = PacketsFactory.GetUnixTime() - serverUnixTime;

		timeDelta = serverDelta + latency;
	}

	public void SyncClock(RTPacket rtPacket)
	{
		long serverUnixTime = rtPacket.Data.GetLong(1).Value;
		long millisecondsToFixClock = serverUnixTime + timeDelta;
		ServerClock = DateTime.UtcNow.AddMilliseconds(millisecondsToFixClock);
	}

	private void OnGUI()
	{
		int width = 200;
		int height = 200;
		Rect areaRect = new Rect(Screen.width-width,0,width,height);
		
		GUILayout.BeginArea(areaRect);
		GUILayout.Label("Time Delta: "+timeDelta.ToString()+"ms");
		GUILayout.Label("Latency: "+latency.ToString()+"ms");
		GUILayout.Label("Server Time: "+ServerClock.ToString());
		GUILayout.EndArea();
	}
}
