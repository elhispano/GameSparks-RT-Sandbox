using System;
using GameSparks.RT;
using UnityEngine;

public static class PacketsFactory
{
    private static readonly DateTime UNIX_REFERENCE_TIME = new DateTime(1970, 1, 1, 0, 0, 0);

    private static readonly int[] SERVER_PEER_ID = new int[] {0};
    
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

    public static void SendTimeStamp(GameSparksRTUnity rtSession)
    {
        if (rtSession == null)
            return;
        
        using (RTData data = RTData.Get())
        {
            long unixTime = GetUnixTime();
            data.SetLong(1, (long) unixTime);

            rtSession.SendData((int)OpCodes.TimeStamp, GameSparksRT.DeliveryIntent.UNRELIABLE, data, SERVER_PEER_ID);
        }
    }

    public static long GetUnixTime()
    {
        return (long)(DateTime.UtcNow - UNIX_REFERENCE_TIME).TotalMilliseconds;
    }
}
