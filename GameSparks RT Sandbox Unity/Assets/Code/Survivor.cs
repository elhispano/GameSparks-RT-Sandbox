using System.Collections;
using System.Collections.Generic;
using GameSparks.Core;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Survivor : MonoBehaviour
{
    private const float SAME_POSITION_THRESHOLD = 0.2f;
    private const float VELOCITY_THRESHOLD = 0.05f;
    
    private SkinnedMeshRenderer skinnedMeshRenderer = null;
    private DebugOnlinePlayerTool m_debugOnlinePlayer = null;
    private Rigidbody m_rigidbody = null;
    private bool m_wasMoving = false;
    private bool m_wasJumping = false;
    private bool m_wasCrouch = false;
    
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
        m_rigidbody = GetComponent<Rigidbody>();
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
        bool hasSpeed = m_rigidbody.velocity.magnitude > VELOCITY_THRESHOLD;
        bool movingStateChanged = hasSpeed != m_wasMoving;
        bool crouchStateChanged = UserControl.Crouch != m_wasCrouch;
        bool jumpStateChanged = UserControl.Jump != m_wasJumping;

        if (hasSpeed
            || movingStateChanged
            || crouchStateChanged
            || jumpStateChanged)
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

        if (hasSpeed)
        {
            Debug.Log("SENDING: "+m_rigidbody.velocity+" TIME: "+Time.time);
        }

        if (!hasSpeed && m_wasMoving)
        {
            Debug.LogError("STOP MOVING "+Time.time);
        }
        
        //Debug.Log("isMoving: "+isMoving+" movingStateChanged: "+movingStateChanged+" crouchStateChanged: "+crouchStateChanged+" jumpStateChanged: "+jumpStateChanged);

        m_wasMoving = hasSpeed;
        m_wasJumping = UserControl.Jump;
        m_wasCrouch = UserControl.Crouch;
        previousPosition = transform.position;
        
        yield return new WaitForSeconds(GameController.UPDATE_RATE);
        
        StartCoroutine(SendMovement(debug));
    }
}
