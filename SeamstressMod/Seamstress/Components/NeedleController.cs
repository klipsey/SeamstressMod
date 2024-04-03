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

        public bool consumeNeedle;

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
            if (characterMotor.jumpCount >= characterBody.maxJumpCount && consumeNeedle && characterBody.HasBuff(SeamstressBuffs.needles))
            {
                Log.Debug("Consume Needle in controller: " + consumeNeedle);
                CmdUpdateNeedles();
            }
            consumeNeedle = false;
        }

        [Command]
        public void CmdUpdateNeedles()
        {
            if (!NetworkServer.active)
            {
                Log.Error("Network Server Not Active");
                return;
            }
            characterBody.RemoveBuff(SeamstressBuffs.needles);
        }
    }
}
