using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityStandardAssets.Characters.ThirdPerson;

public class RTPlayersDebugTools : MonoBehaviour
{
	[SerializeField] private GameObject m_serverPositionDebugPrefab = null;
	
	private GameController m_gameController;
	
	private Dictionary<Survivor,GameObject> debugObjects = new Dictionary<Survivor, GameObject>();

	private void Awake()
	{
		m_gameController = GetComponent<GameController>();
	}

	private void OnGUI()
	{
		int panelWidth = 150;
		
		for (int i = 0; i < m_gameController.survivors.Count; i++)
		{
			Survivor survivor = m_gameController.survivors[i];
			Vector3 velocity = survivor.GetComponent<Rigidbody>().velocity;
			ThirdPersonCharacter character = survivor.GetComponent<ThirdPersonCharacter>();
			
			Rect areaRect = new Rect(panelWidth*i,0f,panelWidth,300f);
			GUILayout.BeginArea(areaRect);
			GUILayout.Label("PeerId: "+survivor.Id);
			GUILayout.Label(survivor.DisplayName);
			GUILayout.Label(velocity.ToString());
			GUILayout.EndArea();
			
			Debug.DrawRay(survivor.transform.position+Vector3.up*0.1f,velocity,Color.magenta,Time.deltaTime);

			if (!survivor.IsPlayer)
			{
				if (!debugObjects.ContainsKey(survivor))
				{
					debugObjects.Add(survivor,Instantiate<GameObject>(m_serverPositionDebugPrefab));
				}
				
				RTUserControl userControl = survivor.GetComponent<RTUserControl>();
				debugObjects[survivor].transform.position = userControl.DesiredPosition;
				debugObjects[survivor].transform.rotation = userControl.DesiredRotation;
			}
		}
	}
}
