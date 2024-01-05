using EntityStates;
using SeamstressMod.Survivors.Seamstress;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeamstressMod.SkillStates.BaseStates
{
    public abstract class BaseSeamstressSkillState : BaseSkillState
    {
        protected SeamstressController seamCon;

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
                seamCon = base.gameObject.GetComponent<SeamstressController>();
            }
            if ((bool)seamCon)
            {
                empowered = seamCon.butchered;
            }
        }
    }
}
