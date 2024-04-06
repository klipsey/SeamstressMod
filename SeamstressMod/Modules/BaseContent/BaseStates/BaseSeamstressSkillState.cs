using EntityStates;
using RoR2;
using SeamstressMod.Seamstress.Components;
using SeamstressMod.Seamstress.Content;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace SeamstressMod.Modules.BaseStates
{
    public abstract class BaseSeamstressSkillState : BaseSkillState
    {
        protected SeamstressController seamCom;

        protected bool scissorRight;

        protected bool scissorLeft;

        protected int needleCount;

        protected bool insatiable;

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

            if (!seamCom)
            {
                seamCom = base.GetComponent<SeamstressController>();
            }
            if (seamCom)
            {
                insatiable = seamCom.inInsatiable;
                seamCom.isDashing = inDash;
            }
        }
    }
}
