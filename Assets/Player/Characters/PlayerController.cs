using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Assertions;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (Rigidbody))]
    [RequireComponent(typeof (CapsuleCollider))]
    public class PlayerController : MonoBehaviour
    {
		public GameObject PlayerHudPrefab;
		private PlayerHudController m_PlayerHud;

		[Serializable]
        public class MovementSettings
        {
            public float ForwardSpeed = 8.0f;   // Speed when walking forward
            public float BackwardSpeed = 4.0f;  // Speed when walking backwards
            public float StrafeSpeed = 4.0f;    // Speed when walking sideways
            public float RunMultiplier = 2.0f;   // Speed when sprinting
	        public KeyCode RunKey = KeyCode.LeftShift;
            public float JumpForce = 30f;
            public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
            [HideInInspector] public float CurrentTargetSpeed = 8f;

			public void UpdateDesiredTargetSpeed(Vector2 input)
            {
	            if (input == Vector2.zero) return;
				if (input.x > 0 || input.x < 0)
				{
					//strafe
					CurrentTargetSpeed = StrafeSpeed;
				}
				if (input.y < 0)
				{
					//backwards
					CurrentTargetSpeed = BackwardSpeed;
				}
				if (input.y > 0)
				{
					//forwards
					//handled last as if strafing and moving forward at the same time forwards speed should take precedence
					CurrentTargetSpeed = ForwardSpeed;
				}
            }
        }


        [Serializable]
        public class AdvancedSettings
        {
            public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
            public float stickToGroundHelperDistance = 0.0f; // stops the character
            public float slowDownRate = 100f; // rate at which the controller comes to a stop when there is no input
            public bool airControl; // can the user control the direction that is being moved in the air
            [Tooltip("set it to 0.1 or more if you get stuck in wall")]
            public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
        }
		
        public Camera cam;
        public MovementSettings movementSettings = new MovementSettings();
        public MouseLook mouseLook = new MouseLook();
        public AdvancedSettings advancedSettings = new AdvancedSettings();
		
        private Rigidbody m_RigidBody;
        private CapsuleCollider m_Capsule;
        private float m_YRotation;
        private Vector3 m_GroundContactNormal;
        private bool m_Jump, m_PreviouslyGrounded, m_Jumping, m_IsGrounded;

		bool m_UImode = false;
		InteractableObject m_CurrentInteractable = null;

		private GameState m_GameState;

        public Vector3 Velocity
        {
            get { return m_RigidBody.velocity; }
        }

        public bool Grounded
        {
            get { return m_IsGrounded; }
        }

        public bool Jumping
        {
            get { return m_Jumping; }
        }

        public bool Running
        {
            get
            {
	            return false;
            }
        }

		private void Awake()
		{
			GameObject prefab = Instantiate( PlayerHudPrefab, new Vector3(0, 0, 0), Quaternion.identity ) as GameObject;	
			m_PlayerHud = prefab.GetComponent<PlayerHudController>( );
			if( m_PlayerHud == null )
			{
				Debug.LogError( "MISSING PLAYER HUD CONTROLLER" );
			}
		}

		private void Start()
        {
            m_RigidBody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>( );
            mouseLook.Init ( transform, cam.transform );

			m_GameState = (GameState)FindObjectOfType(typeof(GameState));
			Assert.IsNotNull( m_GameState , "GameState is null" );					
			StartIntoduction( );
        }
		private void StartIntoduction( ) //Bad place for these functions
		{
			OnPauseGame( true );
			m_PlayerHud.SetIntroScreen( true );
		}

		public void StartGameWon( ) //Bad place for these functions
		{
			
			OnPauseGame( true );
			m_PlayerHud.SetWinScreen( true );
		}

		public void StartGameLost( ) //Bad place for these functions
		{
			OnPauseGame( true );
			m_PlayerHud.SetLoseScreen( true );
		}

        private void Update()
        {
			 HandleInput( );
        }

		void HandleInput( )
		{
			if( m_UImode )
			{
				if( CrossPlatformInputManager.GetButtonDown("Map") && m_PlayerHud.GetMapActive( ) )
				{
					m_PlayerHud.HideMap( );
					OnResumepGame( );
				}				
			
				if ( (Input.GetKeyDown("escape") ) && m_PlayerHud.GeIngameMenuActive( ) )
				{
					m_PlayerHud.SetIngameMenu( false );
					OnResumepGame( );
				}
			}
			else
			{
				RotateView( );
				LookForInteractable( );

				if( CrossPlatformInputManager.GetButtonDown("Use") )
				{
					OnUse( );
				}

				if( CrossPlatformInputManager.GetButtonDown("Map") )
				{
					m_PlayerHud.ShowMap( );
					OnPauseGame( false );
				}
								
				if( CrossPlatformInputManager.GetButtonDown("Objectives") )
				{
					m_PlayerHud.SetIntroScreen( true );
					OnPauseGame( false );
				}
					

				if (CrossPlatformInputManager.GetButtonDown("Jump") && !m_Jump)
				{
					m_Jump = true;
				}

				if (Input.GetKeyDown("escape"))
				{
					m_PlayerHud.SetIngameMenu( true );
					OnPauseGame( true );
				}				
			}     
		}

		void OnUse( )
		{
			if( m_CurrentInteractable && m_CurrentInteractable.OnUse( ) )
			{
				OnPauseGame( true );				
			}		
		}

		public void OnPauseGame( bool with_cursor )
		{
			if( with_cursor )
			{
				Cursor.visible = true;			
				Cursor.lockState = CursorLockMode.None;
			}
			m_UImode = true;
			if( m_GameState ) { m_GameState.PauseTimer( ); }
		}

		public void OnResumepGame( )
		{			
			m_UImode = false;
			if( m_GameState ) { m_GameState.ResumeTimer( ); }
			Cursor.visible = false;
		}

		public void UpdateStoryProgressHUD( int unlocked , int total )
		{
			if( m_PlayerHud )
			{
				m_PlayerHud.UpdateDialogProgress( unlocked , total );
			}
		}
		void LookForInteractable( )
		{
			float interactionDistance = 4f;			
			RaycastHit hit;

			if( Physics.Raycast( cam.transform.position , cam.transform.forward, out hit, interactionDistance ) )
			{
				Debug.DrawRay( cam.transform.position, cam.transform.forward * hit.distance, Color.yellow);

				InteractableObject target = hit.transform.GetComponent<InteractableObject>( );				
				if( target )
				{
					if( m_CurrentInteractable == target ) { return; }
					else if( m_CurrentInteractable ) { m_CurrentInteractable.OnFocusLost( ); }

					m_PlayerHud.ShowHint( );
					m_CurrentInteractable = target;
					m_CurrentInteractable.OnFocusGained( );

				}
				else if ( m_CurrentInteractable )
				{
					m_CurrentInteractable.OnFocusLost( );
					m_CurrentInteractable = null;
				}
			}
			else
			{
				if ( m_CurrentInteractable  )
				{
					m_CurrentInteractable.OnFocusLost( );
					m_CurrentInteractable = null;
					m_PlayerHud.HideHint( );
				}
				Debug.DrawRay( cam.transform.position , cam.transform.forward * interactionDistance , Color.white);
			}
		}	

		/// 
        private void FixedUpdate()
        {
            GroundCheck();
            Vector2 input = GetInput();
			bool isMovementImput = ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) );
			
			if( m_UImode == false )
			{
				if ( isMovementImput && (advancedSettings.airControl || m_IsGrounded))
				{
					// always move along the camera forward as it is the direction that it being aimed at
					Vector3 desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
					desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;

					desiredMove.x = desiredMove.x * movementSettings.CurrentTargetSpeed;
					desiredMove.z = desiredMove.z * movementSettings.CurrentTargetSpeed;
					desiredMove.y = desiredMove.y * movementSettings.CurrentTargetSpeed;
					if ( m_RigidBody.velocity.sqrMagnitude < ( movementSettings.CurrentTargetSpeed * movementSettings.CurrentTargetSpeed ) )
					{
						m_RigidBody.AddForce( desiredMove * SlopeMultiplier( ), ForceMode.Impulse );
					}
				}
			}

            if ( m_IsGrounded )
            {
				if( isMovementImput )
					{  m_RigidBody.drag = 5; }
				else
					{  m_RigidBody.drag = 3000; }

                if ( m_Jump )
                {
                    m_RigidBody.drag = 0f;
                    m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                    m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
                    m_Jumping = true;
                }
				
                if ( !m_Jumping && Mathf.Abs( input.x ) < float.Epsilon && Mathf.Abs( input.y ) < float.Epsilon && m_RigidBody.velocity.magnitude < 1f )
                {
                    m_RigidBody.Sleep( );
                }
            }
            else
            {
                m_RigidBody.drag = 0f;
                if ( m_PreviouslyGrounded && !m_Jumping )
                {
                    StickToGroundHelper( );
                }
            }
            m_Jump = false;
        }
		
        private float SlopeMultiplier()
        {
            float angle = Vector3.Angle(m_GroundContactNormal, Vector3.up);
            return movementSettings.SlopeCurveModifier.Evaluate(angle);
        }
		
        private void StickToGroundHelper()
        {
            RaycastHit hitInfo;
            if ( Physics.SphereCast( transform.position, m_Capsule.radius * ( 1.0f - advancedSettings.shellOffset ), Vector3.down, out hitInfo,
                                   ( ( m_Capsule.height/2f ) - m_Capsule.radius ) +
                                   advancedSettings.stickToGroundHelperDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore ) )
            {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
                {
                    m_RigidBody.velocity = Vector3.ProjectOnPlane( m_RigidBody.velocity, hitInfo.normal );
                }
            }
        }


        private Vector2 GetInput()
        {            
            Vector2 input = new Vector2
                {
                    x = CrossPlatformInputManager.GetAxis("Horizontal"),
                    y = CrossPlatformInputManager.GetAxis("Vertical")
                };
			movementSettings.UpdateDesiredTargetSpeed(input);
            return input;
        }


        private void RotateView()
        {
            //avoids the mouse looking if the game is effectively paused
            if ( Mathf.Abs( Time.timeScale ) < float.Epsilon ) return;

            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;

            mouseLook.LookRotation ( transform, cam.transform );

            if (m_IsGrounded || advancedSettings.airControl)
            {
                // Rotate the rigidbody velocity to match the new direction that the character is looking
                Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
                m_RigidBody.velocity = velRotation*m_RigidBody.velocity;
            }
        }

        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            m_PreviouslyGrounded = m_IsGrounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height/2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                m_IsGrounded = true;
                m_GroundContactNormal = hitInfo.normal;
            }
            else
            {
                m_IsGrounded = false;
                m_GroundContactNormal = Vector3.up;
            }
            if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping)
            {
                m_Jumping = false;
            }
        }
    }
}
