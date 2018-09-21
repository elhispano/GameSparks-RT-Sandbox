using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.Utility;

public class RTUserControl : MonoBehaviour
{
	private const float INTERPOLATION_SPEED = 100f;
	private const float ROTATION_SPEED = 100f;
	
	public bool testMovemenetEnabled = false;
	public Vector3 movementTest = Vector3.zero;

	private ThirdPersonCharacter m_thirdPersonCharacter;
	private bool m_crouch = false;
	private bool m_jump = false;
	private bool m_moving = false;
	private float m_movementReceivedTimeStamp = 0f;
	private Vector3 m_desiredPosition = Vector3.zero;

	private Quaternion desiredRotation = Quaternion.identity;

	public Quaternion DesiredRotation
	{
		get { return desiredRotation; }
		set { desiredRotation = value; }
	}

	public Vector3 DesiredPosition
	{
		get { return m_desiredPosition; }
		set { m_desiredPosition = value; }
	}
	
	private void Awake()
	{
		m_thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
	}

	private void Start()
	{
		m_desiredPosition = transform.position;
		desiredRotation = transform.rotation;
		m_movementReceivedTimeStamp = Time.time;
	}

	private void Update()
	{
		if (testMovemenetEnabled)
		{
			m_thirdPersonCharacter.Move(movementTest,false,false);
		}
	}

	private void FixedUpdate()
	{
		float distanceToPosition = Vector3.Magnitude(transform.position-m_desiredPosition);

		if (distanceToPosition > 0.1f)
		{
			/*Vector3 movVector = m_desiredPosition - transform.position;
			m_thirdPersonCharacter.Move(movVector,m_crouch,m_jump);*/
			
			transform.position = Vector3.Lerp(transform.position,m_desiredPosition,Time.deltaTime/GameController.UPDATE_RATE);
			transform.rotation = Quaternion.Lerp(transform.rotation,desiredRotation,Time.deltaTime/GameController.UPDATE_RATE);
		}
		
		m_jump = false;
	}

	public void SetMovement(Vector3 position,
		Vector3 rotation,
		Vector3 velocity,
		Vector3 inputMovement,
		bool jump,
		bool crouch)
	{
		m_crouch = crouch;
		m_jump = m_jump;
		m_desiredPosition = position;
		desiredRotation = Quaternion.Euler(rotation);

		float currentGameTime = Time.time;
		float timeDiff = currentGameTime - m_movementReceivedTimeStamp;

		m_moving = timeDiff <= GameController.UPDATE_RATE;

		m_movementReceivedTimeStamp = currentGameTime;
	}
}
