using SeamstressMod.SkillStates;
using SeamstressMod.Modules.BaseStates;
namespace SeamstressMod.Survivors.Seamstress
{
    public static class SeamstressStates
    {
        public static void Init()
        {

            Modules.Content.AddEntityState(typeof(SeamstressSpawnState));
            Modules.Content.AddEntityState(typeof(SeamstressMainState));
            Modules.Content.AddEntityState(typeof(BaseSeamstressSkillState));
            Modules.Content.AddEntityState(typeof(BaseSeamstressState));
            Modules.Content.AddEntityState(typeof(SeamstressJump));
            Modules.Content.AddEntityState(typeof(SeamstressBlink));
            Modules.Content.AddEntityState(typeof(SeamstressBlinkUp));

            Modules.Content.AddEntityState(typeof(Trim));

            Modules.Content.AddEntityState(typeof(Flurry));

            Modules.Content.AddEntityState(typeof(SecondaryBlink));
            Modules.Content.AddEntityState(typeof(Clip));
            Modules.Content.AddEntityState(typeof(Telekinesis));

            Modules.Content.AddEntityState(typeof(Exhaustion));

            Modules.Content.AddEntityState(typeof(HealthCostBlink));

            Modules.Content.AddEntityState(typeof(Parry));
            Modules.Content.AddEntityState(typeof(ParryDash));

            Modules.Content.AddEntityState(typeof(FireScissor));
        }
    }
}
