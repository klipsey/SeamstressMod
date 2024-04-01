using System.Collections.Generic;
using EntityStates;
using RoR2.HudOverlay;
using RoR2.UI;
using SeamstressMod.Modules.BaseStates;
using UnityEngine;

namespace SeamstressMod.Seamstress.SkillStates
{
    public class Exhaustion : BaseSeamstressSkillState
    {
        public override void OnEnter()
        {
            Log.Error("Error: You should not have been able to achieve this.");
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }
    }
}


