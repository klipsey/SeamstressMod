using RoR2;
using UnityEngine.Networking;
using RoR2.Skills;
using UnityEngine;

namespace SeamstressMod.Survivors.Seamstress
{
    public class NeedleController : NetworkBehaviour
    {
        private CharacterBody characterBody;

        private int oldNeedleCount = 0;

        private int needleCount = 0;
        public void Awake()
        {
            characterBody = base.GetComponent<CharacterBody>();
        }
        public void Start()
        {
        }
        public void FixedUpdate()
        {
            if (hasAuthority)
            {
                if(needleCount != oldNeedleCount)
                {
                    Util.PlaySound("Play_bandit2_m2_alt_throw", base.gameObject);
                    CmdUpdateNeedles(needleCount);
                }
                oldNeedleCount = needleCount;
            }
        }
        [ClientRpc]
        public void RpcAddNeedle()
        {
            if (hasAuthority && needleCount < SeamstressStaticValues.maxNeedleAmount)
            {
                needleCount++;
            }
        }

        [ClientRpc]
        public void RpcRemoveNeedle()
        {
            if (hasAuthority && needleCount > 0)
            {
                needleCount--;
            }
        }

        [Command]
        public void CmdUpdateNeedles(int newCount)
        {
            if (NetworkServer.active)
            {
                int buffCount = characterBody.GetBuffCount(SeamstressBuffs.needles);
                if (buffCount < newCount)
                {
                    int diff = newCount - buffCount;
                    for (int i = 0; i < diff; i++)
                    {
                        characterBody.AddBuff(SeamstressBuffs.needles);
                    }
                }
                else if (buffCount > newCount)
                {
                    for (int i = 0; i < buffCount; i++)
                    {
                        characterBody.RemoveBuff(SeamstressBuffs.needles);
                    }
                    for (int i = 0; i < newCount; i++)
                    {
                        characterBody.AddBuff(SeamstressBuffs.needles);
                    }
                }
            }
        }
    }
}