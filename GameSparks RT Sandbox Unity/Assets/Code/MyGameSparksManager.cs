﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using GameSparks.Api.Responses;
using GameSparks.Core;
using GameSparks.RT;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyGameSparksManager : MonoBehaviour
{
	private const string MATCHMAKING_CODE = "RealTimeMatch";
	
	private static MyGameSparksManager instance = null;

	private GameSparksRTUnity gameSparksRTUnity;

	private RTSessionInfo m_rtSessionInfo;

	private PacketsProcessor packetsProcessor;

	public static MyGameSparksManager Instance()
	{
		
		if (instance != null)
		{
			return instance; 
		}
		else
		{ 
			Debug.LogError ("GSM| GameSparksManager Not Initialized...");
		}
		
		return null;
	}
	
	public static bool Online { get; private set; }
	
	void Awake()
	{
		packetsProcessor = GetComponent<PacketsProcessor>();
		instance = this; 
		DontDestroyOnLoad(this.gameObject);
	}
	
	#region Login & Registration
	
	public delegate void AuthCallback(AuthenticationResponse _authresp2);
	public delegate void RegCallback(RegistrationResponse _authResp);
	
	/// <summary>
	/// Sends an authentication request or registration request to GS.
	/// </summary>
	/// <param name="_callback1">Auth-Response</param>
	/// <param name="_callback2">Registration-Response</param>
	public void AuthenticateUser (string _userName, string _password, RegCallback _regcallback, AuthCallback _authcallback)
	{
	
		Debug.Log("AuthenticateUser "+_userName+ " "+_password);
		
	  new GameSparks.Api.Requests.RegistrationRequest()
	  // this login method first attempts a registration //
	  // if the player is not new, we will be able to tell as the registrationResponse has a bool 'NewPlayer' which we can check
	  // for this example we use the user-name was the display name also //
				.SetDisplayName(_userName)
				.SetUserName(_userName)
				.SetPassword(_password)
				.Send((regResp) => {
					if(!regResp.HasErrors){ // if we get the response back with no errors then the registration was successful
						Debug.Log("GSM| Registration Successful..."); 
						_regcallback(regResp);
					}else{
						// if we receive errors in the response, then the first thing we check is if the player is new or not
						if(!(bool)regResp.NewPlayer) // player already registered, lets authenticate instead
						{
							Debug.LogWarning("GSM| Existing User, Switching to Authentication");
							new GameSparks.Api.Requests.AuthenticationRequest()
								.SetUserName(_userName)
								.SetPassword(_password)
								.Send((authResp) => {
									if(!authResp.HasErrors){
										Debug.Log("Authentication Successful...");
										_authcallback(authResp);
									}else{
										Debug.LogWarning("GSM| Error Authenticating User \n"+authResp.Errors.JSON);
									}
								});
						}else{
						  // if there is another error, then the registration must have failed
						  Debug.LogWarning("GSM| Error Authenticating User \n"+regResp.Errors.JSON); 
						}
					}
				});
	}
	#endregion

	#region Matchmaking Request

	/// <summary>
	/// This will request a match between as many players you have set in the match.
	/// When the max number of players is found each player will receive the MatchFound message
	/// </summary>
	public void FindPlayers()
	{
		Debug.Log("GSM| Attempting Matchmaking...");
		new GameSparks.Api.Requests.MatchmakingRequest()
			.SetMatchShortCode(MATCHMAKING_CODE) // set the shortCode to be the same as the one we created in the first tutorial
			.SetSkill(0) // in this case we assume all players have skill level zero and we want anyone to be able to join so the skill level for the request is set to zero
			.Send((response) =>
			{
				if (response.HasErrors)
				{
					// check for errors
					Debug.LogError("GSM| MatchMaking Error \n" + response.Errors.JSON);
				}
			});
	}

	#endregion
	
	#region Realtime

	public RTSessionInfo GetRTSessionInfo()
	{
		return m_rtSessionInfo;
	}
	
	public GameSparksRTUnity GetRTUnitySession()
	{
		return gameSparksRTUnity;
	}

	public void StartNewRealTimeSession(RTSessionInfo sessionInfo)
	{
		m_rtSessionInfo = sessionInfo;
		gameSparksRTUnity = gameObject.AddComponent<GameSparksRTUnity>();

		GSRequestData mockedResponse = new GSRequestData()
			.AddNumber("port", (double) sessionInfo.GetPortId())
			.AddString("host", sessionInfo.GetHostUrl())
			.AddString("accessToken", sessionInfo.GetAccessToken());
		
		FindMatchResponse response = new FindMatchResponse(mockedResponse);
		
		
		gameSparksRTUnity.Configure(response,
			OnPlayerConnect,
			OnPlayerDisconnect,
			OnReady,
			OnPacket);
		
		gameSparksRTUnity.Connect();
	}

	private void OnPacket(RTPacket rtPacket)
	{
		if (packetsProcessor == null)
		{
			Debug.LogErrorFormat("[ERROR] Missing PacketsProcessor component");
			return;
		}

		OpCodes opCodeType = (OpCodes) rtPacket.OpCode;
		switch (opCodeType)
		{
			case OpCodes.PlayerTransform:
				packetsProcessor.UpdateOtherPlayerTransforms(rtPacket);
			break;
			case OpCodes.PlayerShoot:
				packetsProcessor.OtherPlayerShoot(rtPacket);
			break;
			case OpCodes.UpdatePlayerShoots:
				packetsProcessor.UpdateOpponentShoots(rtPacket);
			break;
			case OpCodes.RegisterShootHit:
				packetsProcessor.RegisterOtherPlayerHit(rtPacket);
			break;
			case OpCodes.AllPlayersConnected:
				AllPlayersConnected();
				break;
			case OpCodes.TimeStamp:
				RTClock.Instance.ProcessTimeStampPacket(rtPacket);
				break;
			case OpCodes.Countdown:
				if (RTClock.Instance != null)
				{
					RTClock.Instance.SyncClock(rtPacket);					
				}
				break;
			default:
				Debug.LogErrorFormat("[ERROR] Unrecognized OpCode: {0} ",rtPacket.OpCode);
			break;
		}
	}

	private void OnReady(bool isReady)
	{
		Debug.Log ("GSM| Player IsReady, "+isReady);

		/*if (isReady)
		{
			Online = true;
			SceneManager.LoadScene("GameScene");
		}*/
	}

	private void OnPlayerDisconnect(int peerId)
	{
		Debug.Log ("GSM| Player Disconnected, "+peerId);	
	}

	private void OnPlayerConnect(int peerId)
	{
		Debug.Log ("GSM| Player Connected, "+peerId);
	}

	private void AllPlayersConnected()
	{
		Debug.Log("GSM| ALL players connected");
		
		Online = true;
		SceneManager.LoadScene("GameScene");
	}

	#endregion
}
