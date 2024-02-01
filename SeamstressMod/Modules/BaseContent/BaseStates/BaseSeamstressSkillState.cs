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

        public int baseNeedleAmount;

        public bool empowered;

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
            if (!this.seamCon)
            {
                this.seamCon = this.GetComponent<SeamstressController>();
            }
            if ((bool)this.seamCon)
            {
                this.empowered = this.seamCon.butchered;
                this.baseNeedleAmount = this.seamCon.baseNeedleAmount;
            }
        }
        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.empowered);
        }
        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.empowered = reader.ReadBoolean();
        }
    }
}
