using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameSparks.Api.Responses;
using GameSparks.Api.Messages;
using GameSparks.Core;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
	private const string  USERNAME_PPREFS_KEY = "UserName";
	private const string  PASSWORD_PPREFS_KEY = "Password";
	
	[SerializeField]
	private InputField m_userId;
	
	[SerializeField]
	private InputField m_userName;
	
	[SerializeField]
	private InputField m_password;

	[SerializeField]
	private Button m_loginButton;
	
	[SerializeField]
	private Button m_findMatchButton;

	[SerializeField]  
	private Text m_loginStatus;
	
	[SerializeField]  
	private Text m_playerList;

	[SerializeField]
	private GameObject m_loginPanel = null;
	
	[SerializeField]
	private GameObject m_matchesPanel = null;
	
	#region Unity Methods

	void Awake()
	{
		ToggleFindMatch(false);
	}

	void OnEnable()
	{
		GS.GameSparksAvailable += GameSparksAvailable;
		MatchFoundMessage.Listener += OnMatchFound;
		MatchNotFoundMessage.Listener += OnMatchFoundFailed;
	}

	private void OnDisable()
	{
		GS.GameSparksAvailable -= GameSparksAvailable;
		MatchFoundMessage.Listener -= OnMatchFound;
		MatchNotFoundMessage.Listener += OnMatchFoundFailed;
	}
	
	#endregion
	
	#region Class Methods

	void TryAutoLogin()
	{
		if (PlayerPrefs.HasKey(USERNAME_PPREFS_KEY) && PlayerPrefs.HasKey(PASSWORD_PPREFS_KEY))
		{
			string userName = PlayerPrefs.GetString(USERNAME_PPREFS_KEY);
			string password = PlayerPrefs.GetString(PASSWORD_PPREFS_KEY);
			
			MyGameSparksManager.Instance().AuthenticateUser(userName,
				password,
				UserRegisteredCallback,
				UserLoggedCallback);
		}
	}
	
	#endregion

	#region UI Methods

	private void ToggleLogin(bool toggle)
	{
		m_loginPanel.SetActive(toggle);
		m_loginButton.gameObject.SetActive(toggle);
	}
	
	private void ToggleFindMatch(bool toggle)
	{
		m_matchesPanel.gameObject.SetActive(toggle);
		m_findMatchButton.gameObject.SetActive(toggle);
	}

	#endregion

	#region Unity UI Inspector Events

	public void LoginButton()
	{
		MyGameSparksManager.Instance().AuthenticateUser(m_userName.text,
			m_password.text,
			UserRegisteredCallback,
			UserLoggedCallback);
	}

	public void FindMatch()
	{
		MyGameSparksManager.Instance().FindPlayers();
		m_playerList.text = "Searching for players...";
	}

	#endregion

	#region Handlers

	private void GameSparksAvailable(bool available)
	{
		UpdateStatus();
		TryAutoLogin();
	}

	private void UserRegisteredCallback(RegistrationResponse response)
	{
		m_userId.text = response.UserId;
		m_loginStatus.text = "New user created";
		UpdateStatus();
		ToggleLogin(false);
		ToggleFindMatch(true);
	}

	private void UserLoggedCallback(AuthenticationResponse response)
	{
		if (!response.HasErrors)
		{
			PlayerPrefs.SetString(USERNAME_PPREFS_KEY,m_userName.text);	
			PlayerPrefs.SetString(PASSWORD_PPREFS_KEY,m_password.text);	
		}
		
		m_userId.text = response.UserId;
		UpdateStatus();
		ToggleLogin(false);
		ToggleFindMatch(true);
	}
	
	private void OnMatchFoundFailed(MatchNotFoundMessage message)
	{
		m_playerList.text = "Error finding a match";
	}

	private void OnMatchFound(MatchFoundMessage message)
	{
		Debug.Log ("Match Found!...");
		StringBuilder sBuilder = new StringBuilder ();
		sBuilder.AppendLine ("Match Found...");
		sBuilder.AppendLine ("Host URL:" + message.Host);
		sBuilder.AppendLine ("Port:"+message.Port);
		sBuilder.AppendLine ("Access Token:"+message.AccessToken);
		sBuilder.AppendLine ("MatchId:"+message.MatchId);
		sBuilder.AppendLine ("Opponents:"+message.Participants.Count());
		sBuilder.AppendLine ("_________________");
		sBuilder.AppendLine (); // we'll leave a space between the player-list and the match data
		foreach(MatchFoundMessage._Participant player in message.Participants){
			sBuilder.AppendLine ("Player:"+player.PeerId+" User Name:"+player.DisplayName); // add the player number and the display name to the list
		}
		m_playerList.text = sBuilder.ToString (); // set the string to be the player-list field
	}

	#endregion

	void UpdateStatus()
	{
		if (!GS.Available)
		{
			m_loginStatus.text = "GS Not Available";
		}
		else
		{
			m_loginStatus.text = "GS Available";

			if (GS.Authenticated)
			{
				m_loginStatus.text = "User Logged";
			}
		}
	}
}
