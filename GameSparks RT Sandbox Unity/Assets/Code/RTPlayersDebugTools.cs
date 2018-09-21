using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityStandardAssets.Characters.ThirdPerson;

public class RTPlayersDebugTools : MonoBehaviour
{
	[SerializeField] private GameObject m_serverPositionDebugPrefab = null;
	
	[SerializeField]
	private List<GameObject> m_serverPositionDebugInstances = new List<GameObject>();
	
	private GameController m_gameController;

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
				if (m_serverPositionDebugInstances.Count <= i-1)
				{
					m_serverPositionDebugInstances.Add(Instantiate<GameObject>(m_serverPositionDebugPrefab));
				}
				
				RTUserControl userControl = survivor.GetComponent<RTUserControl>();
				m_serverPositionDebugInstances[i-1].transform.position = userControl.DesiredPosition;
				m_serverPositionDebugInstances[i-1].transform.rotation = userControl.DesiredRotation;
			}
		}
	}
}
