using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugOnlinePlayerTool : MonoBehaviour
{
	[SerializeField]
	[Range(0f,2000f)]
	private float m_constantLagInMs = 0f;
	
	private Survivor m_debugSurvivor = null;

	private bool m_debugOnlinePlayerSetup = false;
	private float m_timeCounter;

	private void Update()
	{
		if (!m_debugOnlinePlayerSetup)
		{
			var survivors = GameObject.FindObjectsOfType<Survivor>();

			foreach (Survivor survivor in survivors)
			{
				if (survivor.IsPlayer)
				{
					survivor.SetDebugOnlinePlayer(this);
					Color ghostColor = Color.cyan;
					ghostColor.a = 0.3f;
					m_debugSurvivor = GameController.Instance.CreateAndSetupPlayer(1,"DebugPlayer",false,ghostColor);
					m_debugOnlinePlayerSetup = true;
					break;
				}
			}
		}
	}

	public void SendMovement(Survivor playerSurvivor)
	{
		StartCoroutine(DelayedPackageArrival(playerSurvivor));
	}

	IEnumerator DelayedPackageArrival(Survivor playerSurvivor)
	{
		Vector3 position = playerSurvivor.transform.position;
		Vector3 rotation = playerSurvivor.transform.rotation.eulerAngles;
		Vector3 velocity = playerSurvivor.GetComponent<Rigidbody>().velocity;
		Vector3 inputMovement = playerSurvivor.UserControl.Move;
		bool jump = playerSurvivor.UserControl.Jump;
		bool crouch = playerSurvivor.UserControl.Crouch;
		
		yield return new WaitForSeconds(m_constantLagInMs / 1000f);
		
		var rtUserControl = m_debugSurvivor.GetComponent<RTUserControl>();
		rtUserControl.SetMovement(position,rotation,velocity,inputMovement,jump,crouch,m_constantLagInMs);
	}
}
