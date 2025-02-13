﻿﻿using EntityStates;
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
    public abstract class BaseSeamstressState : BaseState
    {
        protected SeamstressController seamstressController;

        protected NeedleController needleCon;

        protected bool scissorRight;

        protected bool scissorLeft;

        protected bool isInsatiable;
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