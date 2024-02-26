using RoR2;
using UnityEngine.Networking;
using RoR2.Skills;
using UnityEngine;

namespace SeamstressMod.Survivors.Seamstress
{
    public class ScissorController : NetworkBehaviour
    {
        private CharacterBody characterBody;

        private int oldScissorCount = 0;

        private int scissorCount = 0;
        public void Awake()
        {
            characterBody = base.GetComponent<CharacterBody>();
        }
        public void Start()
        {
        }
        public void FixedUpdate()
        {
            if(hasAuthority) 
            {
                if (characterBody.skillLocator.special.stock != scissorCount)
                {
                    Util.PlaySound("Play_bandit2_m2_alt_throw", base.gameObject);
                    scissorCount = characterBody.skillLocator.special.stock;
                }
                if(scissorCount != oldScissorCount) 
                {
                    CmdUpdateScissors(scissorCount);
                }
                oldScissorCount = scissorCount;
            }
        }
        [ClientRpc]
        public void RpcAddSpecialStock()
        {
            if (hasAuthority && characterBody.skillLocator.special.stock < characterBody.skillLocator.special.maxStock)
            {
                characterBody.skillLocator.special.stock++;
                if (characterBody.skillLocator.special.stock == characterBody.skillLocator.special.maxStock)
                {
                    characterBody.skillLocator.special.rechargeStopwatch = 0f;
                }
            }
        }


        [Command]
        public void CmdUpdateScissors(int newCount)
        {
            if (NetworkServer.active)
            {
                int buffLeft = characterBody.GetBuffCount(SeamstressBuffs.scissorLeftBuff);
                int buffRight = characterBody.GetBuffCount(SeamstressBuffs.scissorRightBuff);
                //fix how scissor pickups work
                if (newCount == 2 && buffLeft == 0)
                {
                    characterBody.AddBuff(SeamstressBuffs.scissorLeftBuff);
                }
                else if(newCount == 2 && buffLeft == 0 && buffRight == 1) characterBody.AddBuff(SeamstressBuffs.scissorLeftBuff);

                else if (newCount == 1 && buffRight == 0)
                {
                    characterBody.AddBuff(SeamstressBuffs.scissorRightBuff);
                }
                else if(newCount == 1 && buffLeft == 1)
                {
                    characterBody.RemoveBuff(SeamstressBuffs.scissorLeftBuff);
                }
                else if(newCount == 0 && buffRight == 1)
                {
                    characterBody.RemoveBuff(SeamstressBuffs.scissorRightBuff);
                } 
            }
        }
    }
}
