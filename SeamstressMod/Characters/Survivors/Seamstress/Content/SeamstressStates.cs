using SeamstressMod.SkillStates;

namespace SeamstressMod.Survivors.Seamstress
{
    public static class SeamstressStates
    {
        public static void Init()
        {

            Modules.Content.AddEntityState(typeof(SeamstressSpawnState));
            Modules.Content.AddEntityState(typeof(SeamstressMainState));
            Modules.Content.AddEntityState(typeof(SeamstressBlink));

            Modules.Content.AddEntityState(typeof(Trim));

            Modules.Content.AddEntityState(typeof(Flurry));

            Modules.Content.AddEntityState(typeof(SecondaryBlink));

            Modules.Content.AddEntityState(typeof(Clip));

            Modules.Content.AddEntityState(typeof(Exhaustion));

            //Modules.Content.AddEntityState(typeof(WeaveLeap));
            //Modules.Content.AddEntityState(typeof(Weave));
            //Modules.Content.AddEntityState(typeof(ReapRecast));
            //Modules.Content.AddEntityState(typeof(Sew));

            Modules.Content.AddEntityState(typeof(HealthCostBlink));

            Modules.Content.AddEntityState(typeof(Parry));
            Modules.Content.AddEntityState(typeof(ParryDash));
        }
    }
}
