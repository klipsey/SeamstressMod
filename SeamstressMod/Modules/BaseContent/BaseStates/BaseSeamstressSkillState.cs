using EntityStates;
using RoR2;
using SeamstressMod.Seamstress.Components;
using SeamstressMod.Seamstress.Content;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace SeamstressMod.Modules.BaseStates
{
    public abstract class BaseSeamstressSkillState : BaseSkillState
    {
        protected SeamstressController seamstressController;

        protected bool scissorRight;

        protected bool scissorLeft;

        protected NeedleController needleCon;

        protected int needleCount;

        protected bool isInsatiable;

        protected bool inDash;

        public override void OnEnter()
        {
            RefreshState();
            GetModelAnimator().SetLayerWeight(GetModelAnimator().GetLayerIndex("Scissor, Override"), 0f);
            base.OnEnter();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            RefreshState();
        }
        protected void RefreshState()
        {
            needleCount = characterBody.GetBuffCount(SeamstressBuffs.Needles);
            scissorRight = characterBody.HasBuff(SeamstressBuffs.ScissorRightBuff);
            scissorLeft = characterBody.HasBuff(SeamstressBuffs.ScissorLeftBuff);

            if (!seamstressController)
            {
                seamstressController = base.GetComponent<SeamstressController>();
            }
            if (!needleCon)
            {
                needleCon = base.GetComponent<NeedleController>();
            }
            if (seamstressController)
            {
                isInsatiable = characterBody.HasBuff(SeamstressBuffs.SeamstressInsatiableBuff);
            }
        }
    }
}
