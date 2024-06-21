using BepInEx.Configuration;
using SeamstressMod.Modules;

namespace SeamstressMod.Seamstress.Content
{
    public static class SeamstressConfig
    {
        public static ConfigEntry<bool> heavyEnemy;
        public static ConfigEntry<bool> forceUnlockCharacter;
        public static ConfigEntry<bool> forceUnlockRaven;
        public static ConfigEntry<float> changeGroundedBlinkVelocity;

        public static void Init()
        {
            string section = "Seamstress - 01";

            //add more here or else you're cringe
            heavyEnemy = Config.BindAndOptions(
                section,
                "Pickup Big Enemies",
                false,
                "Allows you to pickup bigger enemies for fun.");

            forceUnlockCharacter = Config.BindAndOptions(
                section,
                "Force Unlock Seamstress",
                false,
                "Cheater...");

            forceUnlockRaven = Config.BindAndOptions(
                section,
                "Force Unlock Raven",
                false,
                "If you're not Feilong, you're a cheater.");

            changeGroundedBlinkVelocity = Config.BindAndOptionsSlider(
                section,
                "Grounded Blink Jump Height",
                0.25f,
                "When grounded, blink will go vertically by the inputted amount from 0 - 1 (0 degrees to 90 degrees)", 
                0, 
                1,
                false);
        }
    }
}
