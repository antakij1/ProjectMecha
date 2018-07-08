using System;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class PlatformerCharacter2D : MonoBehaviour
    {
        [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
        [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        public bool m_FacingRight = true;   // For determining which way the player is currently facing.
		private float dashConstraint = 0.20f;// Time constraint, in seconds, within which the player must double-push a directional button to dash
		private int lastButtonPressed = 0;	/* Represents the last directional button recently pressed. -1 for left, 1 for right. 0 for no directional button 
											   pressed recently */
											   
		private float lastButtonPressedTime;// Time when the last directional button was pressed
		private float dashTime = 0.3f;
		private float whenDashed;
		private bool dashing = false;
		private float jumpStart; 			// Time when the player last began jumping 
		private float totalJumpForce = 0f;	// Total amount of upward force that has been applied to the player for a single jump
		public float jumpForceLimit = 2500f;// Highest amount of upward force that may be applied to a single jump
		public float jumpConstant = 100f;	// Some bs constant for now
		public float initialJumpForce = 625f; // Smallest jump theoretically possible. For use with new jump system.
		private Transform firePoint;
		bool jumpBegun;

        private void Awake()
        {
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
			Transform square = transform.Find("Square_0");
			firePoint = square.Find("FirePoint");
			if(firePoint == null) {
				Debug.LogError("No firepoint found from PlatformerCharacter2D script");
			}
        }

        private void FixedUpdate()
        {
            m_Grounded = false;
			jumpBegun = true;

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                    m_Grounded = true;
					totalJumpForce = 0f;
					jumpBegun = false;
					print("grounded");
            }
            m_Anim.SetBool("Ground", m_Grounded);

            // Set the vertical animation
            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);
        }


        public void Move(float move, bool crouch, bool jump)
        {
			if (Time.time - whenDashed >= dashTime || m_Rigidbody2D.velocity.x == 0)
			{
				dashing = false;
			}
			if (!crouch && !dashing)
			{
				if (Input.GetButtonDown("Horizontal"))
				{
					int currentSign = (int)Input.GetAxisRaw("Horizontal");
					if (currentSign != 0)
					{
						if (currentSign == lastButtonPressed && (Time.time - lastButtonPressedTime <= dashConstraint))
						{
							m_Rigidbody2D.AddForce(new Vector2(currentSign*m_JumpForce, 0f));
							whenDashed = Time.time;
							dashing = true;
						}
				
						lastButtonPressedTime = Time.time;
						lastButtonPressed = currentSign;
					}
				}
			}
			
			// If crouching, check to see if the character can stand up
            if (!crouch && m_Anim.GetBool("Crouch"))
            {
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
                {
                    crouch = true;
                }
            }

            // Set whether or not the character is crouching in the animator
            m_Anim.SetBool("Crouch", crouch);

            //only control the player if grounded or airControl is turned on
            if ((m_Grounded || m_AirControl))
            {
                // Reduce the speed if crouching by the crouchSpeed multiplier
                move = (crouch ? move*m_CrouchSpeed : move);

                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Speed", Mathf.Abs(move));

                // Move the character
				if(!dashing)
				{	
					m_Rigidbody2D.velocity = new Vector2(move*m_MaxSpeed, m_Rigidbody2D.velocity.y);
				}
				else
				{
					m_Rigidbody2D.AddForce(new Vector2(move*m_MaxSpeed, 0f));
				}

                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
                    // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
            }
            // If the player should begin a jump...
            if (m_Grounded && jump && m_Anim.GetBool("Ground"))
            {
				// Add an initial vertical force to the player.
                m_Grounded = false;
                m_Anim.SetBool("Ground", false);
                m_Rigidbody2D.AddForce(new Vector2(0f, initialJumpForce)); //originally was just m_JumpForce
				jumpBegun = true;
            }
			// If the player should hold the button down to jump higher...
			else if (!m_Grounded && Input.GetButton("Jump") && jumpBegun && totalJumpForce < jumpForceLimit){
				//Add more force to the jump, until it reaches the jumpForceLimit.
				float upForce = jumpStart/Time.time * jumpConstant;
				m_Rigidbody2D.AddForce(new Vector2(0f, upForce));
				totalJumpForce += upForce;
				print("extra jumped!");
			}
        }


        private void Flip()
        {
            // Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
			
			Vector3 firePointScale = firePoint.localScale;
			firePointScale.x *= -1;
			firePoint.localScale = firePointScale;
        }
		
		public void setJumpStart(float time){
			jumpStart = time;
		}
		
		public bool getIsFacingRight(){
			return m_FacingRight;
		}
    }
}
