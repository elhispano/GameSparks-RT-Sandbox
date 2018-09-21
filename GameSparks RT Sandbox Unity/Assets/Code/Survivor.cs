using System.Collections;
using System.Collections.Generic;
using GameSparks.Core;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Survivor : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMeshRenderer = null;

    private DebugOnlinePlayerTool m_debugOnlinePlayer = null;
    
    #region  Properties

    public int Id { get; private set; }
    
    public bool IsPlayer { get; private set; }
    
    public string DisplayName { get; private set; }
    
    public ThirdPersonUserControl UserControl { get; private set;}
    
    public RTUserControl RTUserControl { get; private set;}

    #endregion

    
    void Awake()
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        UserControl = GetComponent<ThirdPersonUserControl>();
        RTUserControl = GetComponent<RTUserControl>();
    }

    public void Initialize(int id, bool isPlayer,string name)
    {
        Id = id;
        IsPlayer = isPlayer;
        DisplayName = name;

        if (IsPlayer && MyGameSparksManager.Online)
        {
            StartCoroutine(SendMovement(false));
        }
    }

    public void SetColor(Color color)
    {
        skinnedMeshRenderer.material.color = color;
    }

    public void SetDebugOnlinePlayer(DebugOnlinePlayerTool debugOnlinePlayer)
    {
        m_debugOnlinePlayer = debugOnlinePlayer;
        StartCoroutine(SendMovement(true));
    }

    Vector3 previousPosition = Vector3.zero;
    private IEnumerator SendMovement(bool debug)
    {
        bool samePosition = transform.position == previousPosition;
        bool thereIsSomeInput = UserControl.Move.magnitude > 0f || UserControl.Jump || UserControl.Crouch;

        if (!samePosition && thereIsSomeInput)
        {
            if (!debug)
            {
                GameSparksRTUnity rtSession = MyGameSparksManager.Instance().GetRTUnitySession();
                PacketsFactory.SendPlayerMovement(this,rtSession);
            }
            else
            {
                m_debugOnlinePlayer.SendMovement(this);
            }
        }
        
        previousPosition = transform.position;
        
        yield return new WaitForSeconds(GameController.UPDATE_RATE);
        
        StartCoroutine(SendMovement(debug));
    }
}
