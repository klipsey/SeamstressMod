﻿using SeamstressMod.SkillStates;
using SeamstressMod.SkillStates.BaseStates;

namespace SeamstressMod.Survivors.Seamstress
{
    public static class SeamstressStates
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(Trim));

            Modules.Content.AddEntityState(typeof(Weave));

            Modules.Content.AddEntityState(typeof(Reap));

            Modules.Content.AddEntityState(typeof(Sew));
        }
    }
}