using System.Collections;
using System.Collections.Generic;
using GameSparks.RT;
using UnityEngine;

public class PacketsProcessor : MonoBehaviour
{
	public void UpdateOtherPlayerTransforms(RTPacket packet)
	{
		var gameController = GameController.Instance;

		Vector3 position = packet.Data.GetVector3(1).Value;
		Vector3 rotation = packet.Data.GetVector3(2).Value;
		Vector3 velocity = packet.Data.GetVector3(3).Value;

		Vector3 inputMovement = packet.Data.GetVector3(4).Value;

		Vector2 jumpAndCrouch = packet.Data.GetVector2(5).Value;
		bool jump 	= jumpAndCrouch.x == 1f;
		bool crouch = jumpAndCrouch.y == 1f;
		
		Survivor survivor = gameController.SearchSurvivor(packet.Sender);
		if (survivor != null)
		{
			survivor.RTUserControl.SetMovement(position,rotation,velocity,inputMovement,jump,crouch);
		}
	}

	public void OtherPlayerShoot(RTPacket packet)
	{
		
	}

	public void UpdateOpponentShoots(RTPacket packet)
	{
		
	}

	public void RegisterOtherPlayerHit(RTPacket packet)
	{
		
	}

	public void OnPlayerDisconnected(RTPacket packet)
	{
		
	}
}
