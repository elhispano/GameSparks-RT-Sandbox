using System.Collections;
using System.Collections.Generic;
using GameSparks.Api.Responses;
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
	private Text m_loginStatus;

	[SerializeField]
	private GameObject m_loginPanel = null;

	void OnEnable()
	{
		GS.GameSparksAvailable += GameSparksAvailable;		
	}

	private void OnDisable()
	{
		GS.GameSparksAvailable -= GameSparksAvailable;
	}

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
	
	#region Unity UI Inspector Events

	public void LoginButton()
	{
		MyGameSparksManager.Instance().AuthenticateUser(m_userName.text,
			m_password.text,
			UserRegisteredCallback,
			UserLoggedCallback);
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
	}

	#endregion

	#region UI Methods

	private void ToggleLogin(bool toggle)
	{
		m_loginPanel.SetActive(toggle);
		m_loginButton.gameObject.SetActive(toggle);
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
				ToggleLogin(false);
			}
		}
	}
}
