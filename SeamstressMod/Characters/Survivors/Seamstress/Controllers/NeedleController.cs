using RoR2;
using UnityEngine.Networking;
using RoR2.Skills;
using UnityEngine;

namespace SeamstressMod.Survivors.Seamstress
{
    public class NeedleController : NetworkBehaviour, IOnKilledOtherServerReceiver
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
            characterBody.skillLocator.secondary.RemoveAllStocks();
        }
        public void FixedUpdate()
        {
            if(hasAuthority) 
            {
                if (characterBody.skillLocator.secondary.stock != needleCount)
                {
                    Util.PlaySound("Play_bandit2_m2_alt_throw", base.gameObject);
                    needleCount = characterBody.skillLocator.secondary.stock;
                }
                if(needleCount != oldNeedleCount) 
                {
                    CmdUpdateNeedles(needleCount);
                }
                oldNeedleCount = needleCount;
            }
        }

        public void OnKilledOtherServer(DamageReport damageReport)
        {
            if (NetworkServer.active && damageReport.attacker == base.gameObject)
            {
                RpcAddSecondaryStock();
            }
        }
        [ClientRpc]
        public void RpcAddSecondaryStock()
        {
            if (hasAuthority && characterBody.skillLocator.secondary.stock < characterBody.skillLocator.secondary.maxStock)
            {
                characterBody.skillLocator.secondary.stock++;
                if (characterBody.skillLocator.secondary.stock == characterBody.skillLocator.secondary.maxStock)
                {
                    characterBody.skillLocator.secondary.rechargeStopwatch = 0f;
                }
            }
        }


        [Command]
        public void CmdUpdateNeedles(int newCount)
        {
            if (NetworkServer.active)
            {
                Log.Debug("Uh hello??");
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
