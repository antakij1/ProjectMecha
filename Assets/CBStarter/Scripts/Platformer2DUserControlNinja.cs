using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof (PlatformerCharacter2DNinja))]
    public class Platformer2DUserControlNinja : MonoBehaviour
    {
        private PlatformerCharacter2DNinja m_Character;
        private bool m_Jump;
        private bool attack1;
        private bool attack2;


        private void Awake()
        {
            m_Character = GetComponent<PlatformerCharacter2DNinja>();
        }


        private void Update()
        {
            if (!m_Jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
            attack1 = CrossPlatformInputManager.GetButtonDown("Fire1");
            attack2 = CrossPlatformInputManager.GetButtonDown("Fire2");
        }


        private void FixedUpdate()
        {
            // Read the inputs.
            bool crouch = Input.GetKey(KeyCode.S);
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            // Pass all parameters to the character control script.
            m_Character.Move(h, crouch, m_Jump, attack1, attack2);
            m_Jump = false;
            attack1 = false;
            attack2 = false;
        }
    }
}
