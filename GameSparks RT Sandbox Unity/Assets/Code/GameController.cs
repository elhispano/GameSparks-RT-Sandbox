using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using GameSparks.Core;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class GameController : MonoBehaviour
{
	public const float UPDATE_RATE = 0.1f;
	
	private static readonly Color[] PLAYER_COLORS = new Color[]{Color.blue,Color.yellow,Color.green,Color.red};
	
	public static GameController Instance { get; private set; }
	
	public List<Survivor> survivors = new List<Survivor>();


	private CinemachineVirtualCamera cinemachineCamera;
	
	#region Unity

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		cinemachineCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();

		if (MyGameSparksManager.Instance() != null)
		{
			RTSessionInfo sessionInfo = MyGameSparksManager.Instance().GetRTSessionInfo();
			if (sessionInfo != null)
			{
				var playerList = sessionInfo.GetPlayerList();
				survivors = new List<Survivor>(playerList.Count);
				
				Debug.Log("PLAYER LIST COUNT: "+playerList.Count);
				
				for (var index = 0; index < playerList.Count; index++)
				{
					RTSessionInfo.RtPlayer rtPlayer = playerList[index];
					
					Debug.Log(rtPlayer);
					
					Color playerColor = PLAYER_COLORS[index];
					
					CreateAndSetupPlayer(rtPlayer.PeerId,
						rtPlayer.DisplayName,
						rtPlayer.isMe,
						playerColor);
				}
			}
		}
		else
		{
			CreateAndSetupPlayer(0,"LocalPlayer",true,Color.gray);
		}
	}

	private void OnDestroy()
	{
		Instance = null;
	}
	
	#endregion
	
	#region Public Interface

	public Survivor CreateAndSetupPlayer(int id,string name,bool isPlayer, Color color)
	{
		Vector3 randomVector = UnityEngine.Random.insideUnitCircle;
		float randomDistance = UnityEngine.Random.Range(0f, 5f);
		randomVector *= randomDistance;
		Vector3 spawnPosition = new Vector3(randomVector.x,0f,randomVector.y);
		
		GameObject newPlayer = ObjectSpawner.Instance.SpawnPlayerCharacter(Vector3.zero);
		Survivor survivor = newPlayer.GetComponent<Survivor>();
		survivor.Initialize(id,isPlayer,name);
		survivor.SetColor(color);

		if (isPlayer)
		{
			cinemachineCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
			cinemachineCamera.Follow = newPlayer.transform;
			cinemachineCamera.LookAt = newPlayer.transform;
			
			survivor.GetComponent<ThirdPersonUserControl>().enabled = true;
			survivor.GetComponent<RTUserControl>().enabled = false;
			survivor.GetComponent<ThirdPersonCharacter>().ToggleRootMotion(true);
		}
		else
		{
			survivor.GetComponent<ThirdPersonUserControl>().enabled = false;
			survivor.GetComponent<RTUserControl>().enabled = true;
			survivor.GetComponent<ThirdPersonCharacter>().ToggleRootMotion(false);
		}
		
		survivors.Add(survivor);

		return survivor;
	}

	public Survivor SearchSurvivor(int peerId)
	{
		Survivor survivor = null;

		survivor = survivors.Find(x => x.Id == peerId);

		if (survivor == null)
		{
			Debug.LogErrorFormat("Survivor {0} not found",peerId);
		}

		return survivor;
	}
	
	#endregion
}
