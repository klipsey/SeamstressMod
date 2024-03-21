using BepInEx.Configuration;
using SeamstressMod.Modules;

namespace SeamstressMod.Survivors.Seamstress
{
    public static class SeamstressConfig
    {
        public static ConfigEntry<bool> funny;
        public static ConfigEntry<float> someConfigFloat;
        public static ConfigEntry<float> someConfigFloatWithCustomRange;

        public static void Init()
        {
            string section = "Seamstress";

            //add more here or else you're cringe
            funny = Config.BindAndOptions(
                section,
                "Pickup Big Enemies",
                false,
                "Allows you to pickup bigger enemies for fun.");

            someConfigFloat = Config.BindAndOptions(
                section,
                "Nothing. For now...",
                5f);//blank description will default to just the name

            someConfigFloatWithCustomRange = Config.BindAndOptions(
                section,
                "someConfigfloat2",
                5f,
                0,
                50,
                "Nothing. For now...");
        }
    }
}
