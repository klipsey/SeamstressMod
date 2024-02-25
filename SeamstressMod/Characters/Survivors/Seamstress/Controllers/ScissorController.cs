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
                int buffCount = characterBody.GetBuffCount(SeamstressBuffs.scissorCount);
                if (buffCount < newCount)
                {
                    int diff = newCount - buffCount;
                    for (int i = 0; i < diff; i++)
                    {
                        characterBody.AddBuff(SeamstressBuffs.scissorCount);
                    }
                }
                else if (buffCount > newCount)
                {
                    for (int i = 0; i < buffCount; i++)
                    {
                        characterBody.RemoveBuff(SeamstressBuffs.scissorCount);
                    }
                    for (int i = 0; i < newCount; i++)
                    {
                        characterBody.AddBuff(SeamstressBuffs.scissorCount);
                    }
                }
            }
        }
    }
}
