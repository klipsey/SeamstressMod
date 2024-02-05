using SeamstressMod.SkillStates;

namespace SeamstressMod.Survivors.Seamstress
{
    public static class SeamstressStates
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(Trim));

            Modules.Content.AddEntityState(typeof(Flurry));

            Modules.Content.AddEntityState(typeof(WeaveLeap));

            Modules.Content.AddEntityState(typeof(Weave));

            Modules.Content.AddEntityState(typeof(BlinkSeamstress));

            Modules.Content.AddEntityState(typeof(ReapRecast));

            Modules.Content.AddEntityState(typeof(Sew));
        }
    }
}
