using UnityEngine;
using System;
using FSM = HutongGames.PlayMaker;

public class PlayerController : MonoBehaviour {

    /// <summary>
    /// The speed multiplier of the player
    /// </summary>
    public float WalkSpeed = 450.0f;

    /// <summary>
    /// The speed multiplier of the player
    /// </summary>
    public float RunSpeed = 600.0f;

    /// <summary>
    /// 
    /// </summary>
    public float FlipSpeed = 2.0f;

    /// <summary>
    /// 
    /// </summary>
    public float RotationSpeed = 100.0f;

    /// <summary>
    /// 
    /// </summary>
    private PlayMakerFSM m_stateMachine = null;

    /// <summary>
    /// 
    /// </summary>
    private FSM.FsmBool m_canPlayerMove = null;

    /// <summary>
    /// 
    /// </summary>
    private FSM.FsmBool m_canPlayerLook = null;

    /// <summary>
    /// 
    /// </summary>
    private FSM.FsmBool m_playerHasLookedAround = null;

    /// <summary>
    /// 
    /// </summary>
    private FSM.FsmBool m_playerHasGun = null;

    /// <summary>
    /// The players rigidbody
    /// </summary>
    private Rigidbody m_rigidbody = null;

    /// <summary>
    /// 
    /// </summary>
    private Camera m_viewCamera = null;

    /// <summary>
    /// 
    /// </summary>
    private bool m_isFlipping = false;

    /// <summary>
    /// 
    /// </summary>
    private bool m_canPickup = true;

    /// <summary>
    /// 
    /// </summary>
    private SpringJoint m_pickupHook = null;

    private Vector3 m_startRotation = Vector3.zero;
    private Vector3 m_endRotation = Vector3.zero;
    private float m_flipStartTime = -1.0f;
    private Color m_startColor = Color.red;
    private Color m_endColor = Color.green;

    private AudioSource m_gunAudio = null;
    private Material m_gunMaterial = null;

    private CharacterControlActions m_controlActions = null;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start ()
    {
        m_rigidbody = this.GetComponent<Rigidbody>();
        if (m_rigidbody == null)
        {
            Debug.LogError("Failed to find a rigid body on the player.");
        }

        m_viewCamera = Camera.main;

        m_stateMachine = this.GetComponent<PlayMakerFSM>();
        if (m_stateMachine == null)
        {
            Debug.LogError("Failed to find a state machine on the player.");
        }

        m_canPlayerMove = m_stateMachine.FsmVariables.FindFsmBool("PlayerCanMove");
        m_canPlayerLook = m_stateMachine.FsmVariables.FindFsmBool("PlayerCanLook");
        m_playerHasLookedAround = m_stateMachine.FsmVariables.FindFsmBool("PlayerHasLookedAround");
        m_playerHasGun = m_stateMachine.FsmVariables.FindFsmBool("HasGun");

        m_pickupHook = this.GetComponentInChildren<SpringJoint>();

        var gun = this.transform.FindChild("Main Camera/Gun");
        m_gunMaterial = gun.GetComponent<Renderer>().sharedMaterial;
        m_gunAudio = gun.GetComponent<AudioSource>();

        m_gunMaterial.SetColor("_EmissionColor", m_startColor);

        m_controlActions = new CharacterControlActions();
        m_controlActions.Setup();

        Cursor.lockState = CursorLockMode.Locked;
        
    }
	
    void Update()
    {
        Cursor.visible = false;
    }

	/// <summary>
    /// Called once per fixed timestep
    /// </summary>
	void FixedUpdate ()
    {
        HandlePlayerMovement();
	}

    /// <summary>
    /// Handle player movement controls
    /// </summary>
    /// <param name="device">The input device controlling the player</param>
    private void HandlePlayerMovement()
    {
        if (m_controlActions.Pickup.WasPressed && m_canPickup)
        {
            m_canPickup = false;

            if (m_pickupHook.connectedBody == null)
            {
                RaycastHit hitInfo;
                var ray = new Ray(m_viewCamera.transform.position, m_viewCamera.transform.forward);
                if (Physics.Raycast(ray, out hitInfo, 1.5f))
                {
                    // Pickup and object
                    var pickup = hitInfo.collider;
                    if (pickup.tag == "Interactable")
                    {
                        var pickupRB = pickup.GetComponent<Rigidbody>();

                        pickupRB.velocity = Vector3.zero;
                        pickupRB.angularVelocity = Vector3.zero;
                        pickupRB.drag = 1000.0f;

                        pickup.transform.position = m_pickupHook.transform.position;
                        m_pickupHook.connectedBody = pickupRB;
                    }
                }
            }
            else
            {
                m_pickupHook.connectedBody.drag = 5.0f; // Store this
                m_pickupHook.connectedBody.AddForce(this.transform.forward.normalized);
                m_pickupHook.connectedBody = null;
            }            
        }

        if (m_controlActions.Pickup.WasReleased)
        {
            m_canPickup = true;
        }

        if (m_canPlayerMove.Value)
        {
            m_rigidbody.AddRelativeForce(Vector3.forward * m_controlActions.Vertical.Value * GetSpeedMultiplier());
            m_rigidbody.AddRelativeForce(Vector3.right * m_controlActions.Horizontal.Value * GetSpeedMultiplier());
        }

        if (m_canPlayerLook.Value)
        {
            m_rigidbody.AddRelativeTorque(Vector3.up * m_controlActions.LookHorizontal.Value * this.RotationSpeed);
            m_viewCamera.transform.Rotate(Vector3.left, m_controlActions.LookVertical.Value);

            if (!m_playerHasLookedAround.Value && (m_controlActions.LookHorizontal.Value != 0.0f || m_controlActions.LookVertical.Value != 0.0f))
            {
                m_playerHasLookedAround.Value = true;
            }
        }

        if (m_playerHasGun.Value)
        {
            if (m_controlActions.Fire.Value >= 0.4f && !m_isFlipping)
            {
                m_isFlipping = true;
                Physics.gravity = Physics.gravity * -1.0f;
                m_startRotation = this.transform.localEulerAngles;
                m_endRotation = m_startRotation + new Vector3(180.0f, 180.0f, 0.0f);
                m_flipStartTime = Time.time;

                m_startColor = m_gunMaterial.GetColor("_EmissionColor");
                m_endColor = m_startColor == Color.red ? Color.green : Color.red;

                m_gunAudio.Play();
            }

            if (m_isFlipping)
            {
                var time = (Time.time - m_flipStartTime) * this.FlipSpeed;

                this.transform.localEulerAngles = Vector3.Lerp(m_startRotation, m_endRotation, time);
                m_gunMaterial.SetColor("_EmissionColor", Color.Lerp(m_startColor, m_endColor, time));

                if (time >= 1.0f)
                {
                    m_isFlipping = false;
                    m_flipStartTime = -1.0f;
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private float GetSpeedMultiplier()
    {
        // TODO: Add run mode
        return this.WalkSpeed;
    }
}
