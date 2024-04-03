using RoR2;
using UnityEngine.Networking;
using RoR2.Skills;
using UnityEngine;
using SeamstressMod.Seamstress.Content;

namespace SeamstressMod.Seamstress.Components
{
    public class NeedleController : NetworkBehaviour
    {
        private CharacterBody characterBody;

        private CharacterMotor characterMotor;

        public bool inNeedleConsumeRange;

        public void Awake()
        {
            characterBody = GetComponent<CharacterBody>();
            characterMotor = GetComponent<CharacterMotor>();
        }

        public void Start()
        {
        }

        public void FixedUpdate()
        {
            if (hasAuthority)
            {
                if (characterMotor.jumpCount >= characterBody.maxJumpCount && inNeedleConsumeRange)
                {
                    CmdUpdateNeedles();
                    inNeedleConsumeRange = false;
                }
            }
        }

        [Command]
        public void CmdUpdateNeedles()
        {
            if (NetworkServer.active)
            {
                characterBody.RemoveBuff(SeamstressBuffs.needles);
            }
        }
    }
}
