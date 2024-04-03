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
        protected SeamstressController seamCon;

        protected NeedleController needleCon;

        protected bool scissorRight;

        protected bool scissorLeft;

        protected int needleCount;

        protected bool hasNeedles;

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
            needleCount = characterBody.GetBuffCount(SeamstressBuffs.needles);
            scissorRight = characterBody.HasBuff(SeamstressBuffs.scissorRightBuff);
            scissorLeft = characterBody.HasBuff(SeamstressBuffs.scissorLeftBuff);
            if (!seamCon)
            {
                seamCon = base.GetComponent<SeamstressController>();
            }
            if (!needleCon)
            {
                needleCon = base.GetComponent<NeedleController>();
            }
            if (seamCon)
            {
                empowered = characterBody.HasBuff(SeamstressBuffs.instatiable);
            }
        }
    }
}
