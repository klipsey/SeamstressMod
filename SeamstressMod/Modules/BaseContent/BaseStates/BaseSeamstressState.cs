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
    public abstract class BaseSeamstressState : BaseState
    {
        protected SeamstressController seamCom;

        protected NeedleController needleCon;

        protected bool scissorRight;

        protected bool scissorLeft;

        protected bool empowered;

        public override void OnEnter()
        {
            base.OnEnter();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            RefreshState();
        }
        protected void RefreshState()
        {
            scissorRight = characterBody.HasBuff(SeamstressBuffs.scissorRightBuff);
            scissorLeft = characterBody.HasBuff(SeamstressBuffs.scissorLeftBuff);
            if (!seamCom)
            {
                seamCom = base.GetComponent<SeamstressController>();
            }
            if (!needleCon)
            {
                needleCon = base.GetComponent<NeedleController>();
            }
            if (seamCom)
            {
                empowered = characterBody.HasBuff(SeamstressBuffs.instatiable);
            }
        }
    }
}
