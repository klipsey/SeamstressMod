using RoR2;
using UnityEngine.Networking;
using RoR2.Skills;
using UnityEngine;
using SeamstressMod.Seamstress.Content;

namespace SeamstressMod.Seamstress.Components
{
    public class ScissorController : NetworkBehaviour
    {
        private CharacterBody characterBody;

        private int oldScissorCount = 0;

        private int scissorCount = 0;

        public bool isRight;

        public void Awake()
        {
            characterBody = GetComponent<CharacterBody>();
        }

        public void Start()
        {
        }

        public void FixedUpdate()
        {
            if (hasAuthority)
            {
                if (characterBody.skillLocator.special.stock != scissorCount)
                {
                    Util.PlaySound("Play_item_proc_novaonheal_spawn", gameObject);
                    scissorCount = characterBody.skillLocator.special.stock;
                }
                if (scissorCount != oldScissorCount)
                {
                    CmdUpdateScissors(scissorCount);

                    if(scissorCount > oldScissorCount)
                    {
                        if (isRight)
                        {
                        }
                        else if (!isRight) characterBody.GetComponent<SeamstressController>().ReactivateScissor("ScissorLModel", true);
                    }
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
                switch (newCount)
                {
                    case 0:
                        if (buffLeft == 1)
                        {
                            characterBody.RemoveBuff(SeamstressBuffs.scissorLeftBuff);
                            characterBody.GetComponent<SeamstressController>().ReactivateScissor("ScissorLModel", false);
                        }
                        if (buffRight == 1)
                        {
                            characterBody.RemoveBuff(SeamstressBuffs.scissorRightBuff);
                            characterBody.GetComponent<SeamstressController>().ReactivateScissor("ScissorRModel", false);
                        }
                        break;
                    case 1:
                        if (isRight)
                        {
                            if (buffRight == 0)
                            {
                                characterBody.AddBuff(SeamstressBuffs.scissorRightBuff);
                                characterBody.GetComponent<SeamstressController>().ReactivateScissor("ScissorRModel", true);
                            }
                            if (buffLeft == 1)
                            {
                                characterBody.RemoveBuff(SeamstressBuffs.scissorLeftBuff);
                                characterBody.GetComponent<SeamstressController>().ReactivateScissor("ScissorLModel", false);
                            }
                        }
                        if (!isRight)
                        {
                            if (buffRight == 1)
                            {
                                characterBody.RemoveBuff(SeamstressBuffs.scissorRightBuff);
                                characterBody.GetComponent<SeamstressController>().ReactivateScissor("ScissorLModel", false);
                            }
                            if (buffLeft == 0)
                            {
                                characterBody.AddBuff(SeamstressBuffs.scissorLeftBuff);
                                characterBody.GetComponent<SeamstressController>().ReactivateScissor("ScissorLModel", true);
                            }
                        }
                        break;
                    case 2:
                        if (buffLeft == 0)
                        {
                            characterBody.AddBuff(SeamstressBuffs.scissorLeftBuff);
                            characterBody.GetComponent<SeamstressController>().ReactivateScissor("ScissorLModel", true);
                        }
                        if (buffRight == 0)
                        {
                            characterBody.AddBuff(SeamstressBuffs.scissorRightBuff);
                            characterBody.GetComponent<SeamstressController>().ReactivateScissor("ScissorRModel", true);
                        }
                        break;
                }
            }
        }
    }
}
