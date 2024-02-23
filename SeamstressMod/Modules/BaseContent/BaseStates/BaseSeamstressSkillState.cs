using EntityStates;
using RoR2;
using SeamstressMod.Survivors.Seamstress;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace SeamstressMod.Modules.BaseStates
{
    public abstract class BaseSeamstressSkillState : BaseSkillState
    {
        protected SeamstressController seamCon;

        protected int scissorCount;

        protected int needleCount;

        protected bool empowered;

        public override void OnEnter()
        {
            RefreshState();
            base.OnEnter();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
        protected void RefreshState()
        {
            if (NetworkServer.active)
            {
                scissorCount = characterBody.GetBuffCount(SeamstressBuffs.scissorCount);
                needleCount = characterBody.GetBuffCount(SeamstressBuffs.needles);

            }
            if (!seamCon)
            {
                seamCon = base.GetComponent<SeamstressController>();
            }
            if (seamCon)
            {
                empowered = characterBody.HasBuff(SeamstressBuffs.butchered);
            }
        }
    }
}
