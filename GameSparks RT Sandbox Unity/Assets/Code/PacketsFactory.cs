using GameSparks.RT;
using UnityEngine;

public static class PacketsFactory
{
    public static void SendPlayerMovement(Survivor survivor, GameSparksRTUnity rtSession)
    {
        using (RTData data = RTData.Get())
        {
            data.SetVector3(1,survivor.transform.position);
            data.SetVector3(2, survivor.transform.rotation.eulerAngles);
            data.SetVector3(3, survivor.GetComponent<Rigidbody>().velocity);

            data.SetVector4(4, survivor.UserControl.Move);
            
            float jumpFloat = survivor.UserControl.Jump ? 1 : 0;
            float crouchFloat = survivor.UserControl.Crouch ? 1 : 0;
            data.SetVector2(5, new Vector2(jumpFloat,crouchFloat));
            
            rtSession.SendData((int)OpCodes.PlayerTransform, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);  
        }
    }
}
