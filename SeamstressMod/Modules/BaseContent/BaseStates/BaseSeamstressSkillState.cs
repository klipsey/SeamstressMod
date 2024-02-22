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

        protected int baseNeedleAmount;

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
