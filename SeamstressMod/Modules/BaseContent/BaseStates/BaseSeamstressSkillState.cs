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

        protected bool scissorRight;

        protected bool scissorLeft;

        protected int needleCount;

        protected bool empowered;

        protected bool inDash;

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
            needleCount = characterBody.GetBuffCount(SeamstressBuffs.needles);
            scissorRight = characterBody.HasBuff(SeamstressBuffs.scissorRightBuff);
            scissorLeft = characterBody.HasBuff(SeamstressBuffs.scissorLeftBuff);
            if (!seamCon)
            {
                seamCon = base.GetComponent<SeamstressController>();
            }
            if (seamCon)
            {
                empowered = characterBody.HasBuff(SeamstressBuffs.butchered);
                seamCon.isDashing = inDash;
            }
        }
    }
}
