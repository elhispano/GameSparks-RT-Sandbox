using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.Utility;

public class RTUserControl : MonoBehaviour
{
	private const float INTERPOLATION_SPEED = 100f;
	private const float ROTATION_SPEED = 300f;

	[SerializeField]
	private float maxCharacterSpeed = 5f;

	private ThirdPersonCharacter m_thirdPersonCharacter;
	private bool m_crouch = false;
	private bool m_jump = false;
	private bool m_moving = false;
	private float m_movementReceivedTimeStamp = 0f;
	private Vector3 m_desiredPosition = Vector3.zero;
	private Vector3 m_previousPosition = Vector3.zero;
	private Rigidbody m_rigidbody = null;

	private Quaternion m_desiredRotation = Quaternion.identity;

	public Quaternion DesiredRotation
	{
		get { return m_desiredRotation; }
		set { m_desiredRotation = value; }
	}

	public Vector3 DesiredPosition
	{
		get { return m_desiredPosition; }
		set { m_desiredPosition = value; }
	}
	
	private void Awake()
	{
		m_thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
		m_rigidbody = GetComponent<Rigidbody>();
	}

	private void Start()
	{
		m_desiredPosition = transform.position;
		m_desiredRotation = transform.rotation;
		m_movementReceivedTimeStamp = Time.time;
		m_previousPosition = transform.position;
	}

	private void FixedUpdate()
	{
		float x = m_rigidbody.velocity.x/maxCharacterSpeed;
		float y = m_rigidbody.velocity.y/maxCharacterSpeed;
		float z = m_rigidbody.velocity.z/maxCharacterSpeed;
		Vector3 animatorVelocity = new Vector3(x,y,z);
		m_thirdPersonCharacter.Move(animatorVelocity,m_crouch,m_jump);

		float distanceToPosition = Vector3.Magnitude(transform.position-m_desiredPosition);
		if (distanceToPosition > 0.1f)
		{
			m_rigidbody.position = Vector3.MoveTowards(m_rigidbody.position, m_desiredPosition, Time.fixedDeltaTime / 0.2f);
		}

		float angle = Quaternion.Angle(m_rigidbody.rotation, m_desiredRotation);
		if (angle > 1f)
		{
			m_rigidbody.rotation = Quaternion.RotateTowards(m_rigidbody.rotation,m_desiredRotation, Time.fixedDeltaTime * ROTATION_SPEED);	
		}
	}

	public void SetMovement(Vector3 position,
		Vector3 rotation,
		Vector3 velocity,
		Vector3 inputMovement,
		bool jump,
		bool crouch,
		float packageTime)
	{
		m_crouch = crouch;
		m_jump = m_jump;

		m_desiredPosition = position;
		m_desiredRotation = Quaternion.Euler(rotation);	

		float lagInSeconds = (packageTime / 1000f);

		m_moving = velocity.magnitude > 0.05f;
		if (!m_moving)
		{
			Debug.Log("MOVING STOPPED: "+Time.time);
			m_rigidbody.velocity = Vector3.zero;
		}
		else
		{
			m_rigidbody.velocity = velocity;
			m_desiredPosition += m_rigidbody.velocity * lagInSeconds;
		}
		
		Debug.Log("RECEIVING: "+velocity+" TIME: "+Time.time);
	}
}
