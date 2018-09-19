using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public static GameController Instance { get; private set; }
	
	public List<Survivor> survivors = new List<Survivor>();
	
	#region Unity

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		
	}

	private void OnDestroy()
	{
		Instance = null;
	}
	
	#endregion
	
	#region Public Interface

	public void CreateAndSetupPlayer(bool isPlayer)
	{
		// TODO: Use ObjectSpawner to create the player
		// TODO: Initialize the Survivor component in the player
		// TODO: Setup camera with instantiated player
		// TODO: Disable components if not player to avoid input control
	}
	
	#endregion
}
