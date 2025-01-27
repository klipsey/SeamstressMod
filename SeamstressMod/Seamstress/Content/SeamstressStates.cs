﻿using SeamstressMod.Modules.BaseStates;
using SeamstressMod.Seamstress.SkillStates;

namespace SeamstressMod.Seamstress.Content
{
    public static class SeamstressStates
    {
        public static void Init()
        {

            Modules.Content.AddEntityState(typeof(SeamstressSpawnState));
            Modules.Content.AddEntityState(typeof(MainState));
            Modules.Content.AddEntityState(typeof(BaseSeamstressSkillState));
            Modules.Content.AddEntityState(typeof(BaseSeamstressState));
            Modules.Content.AddEntityState(typeof(SeamstressJump));
            Modules.Content.AddEntityState(typeof(SeamstressBlink));
            Modules.Content.AddEntityState(typeof(SeamstressBlinkUp));

            Modules.Content.AddEntityState(typeof(Trim));

            Modules.Content.AddEntityState(typeof(Flurry));

            Modules.Content.AddEntityState(typeof(Clip));
            Modules.Content.AddEntityState(typeof(Telekinesis));

            Modules.Content.AddEntityState(typeof(HealthCostDash));
            //Modules.Content.AddEntityState(typeof(Snapback));

            Modules.Content.AddEntityState(typeof(Parry));
            Modules.Content.AddEntityState(typeof(ParryDash));

            Modules.Content.AddEntityState(typeof(FireScissor));
            Modules.Content.AddEntityState(typeof(FireScissorScepter));

            Modules.Content.AddEntityState(typeof(HeartSpawnState));
            Modules.Content.AddEntityState(typeof(HeartStandBy));
        }
    }
}
