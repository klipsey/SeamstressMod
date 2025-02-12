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

        public static ConfigEntry<float> baseDamage;

        public static ConfigEntry<float> basePickupCooldown;

        public static ConfigEntry<float> trimDamageCoefficient;

        public static ConfigEntry<float> trimThirdDamageCoefficient;

        public static ConfigEntry<float> flurryDamageCoefficient;

        public static ConfigEntry<float> scissorSlashDamageCoefficient;

        public static ConfigEntry<float> scissorPickupDamageCoefficient;

        public static ConfigEntry<float> blinkDamageCoefficient;

        public static ConfigEntry<float> bleedDuration;

        public static ConfigEntry<float> blinkCooldown;

        public static ConfigEntry<float> healConversion;

        public static ConfigEntry<int> maxNeedleAmount;

        public static ConfigEntry<float> insatiableDuration;

        public static ConfigEntry<float> passiveScaling;

        public static ConfigEntry<float> passiveLifeSteal;

        public static ConfigEntry<float> parryDamageCoefficient;

        public static ConfigEntry<float> needleDamageCoefficient;

        public static ConfigEntry<float> clipDamageCoefficient;

        public static ConfigEntry<float> needleProcCoefficient;

        public static ConfigEntry<float> parryWindow;

        public static ConfigEntry<float> scissorDamageCoefficient;

        public static ConfigEntry<float> telekinesisDamageCoefficient;

        public static ConfigEntry<float> telekinesisCooldown;

        public static ConfigEntry<float> explodeDamageCoefficient;


        public static void Init()
        {
            string section = "Seamstress - 01";
            string section2 = "Seamstress - 02";

            baseDamage = Config.BindAndOptions(section, "Change Base Damage Value", 12f);

            basePickupCooldown = Config.BindAndOptions(section, "Change Base Pickup Cooldown Value", 3f);

            trimDamageCoefficient = Config.BindAndOptions(section, "Change Trim Damage Coefficient", 1.2f);

            trimThirdDamageCoefficient = Config.BindAndOptions(section, "Change Trim Third Damage Coefficient", 1.5f);

            flurryDamageCoefficient = Config.BindAndOptions(section, "Change Flurry Damage Coefficient", 1.4f);

            scissorSlashDamageCoefficient = Config.BindAndOptions(section, "Change Scissor Slash Damage Coefficient", 2f);

            scissorPickupDamageCoefficient = Config.BindAndOptions(section, "Change Scissor Pickup Damage Coefficient", 2f);

            blinkDamageCoefficient = Config.BindAndOptions(section, "Change Blink Damage Coefficient", 2.5f);

            bleedDuration = Config.BindAndOptions(section, "Change Bleed Duration", 2f);

            blinkCooldown = Config.BindAndOptions(section, "Change Blink Cooldown", 0.4f);

            healConversion = Config.BindAndOptionsSlider(section, "Change Heal Conversion", 0.001f, "Changes the amount of healing converted to barrier during Insatiable. (0.001 == 99.999% of healing is converted)", 0.001f);

            maxNeedleAmount = Config.BindAndOptions(section, "Change Max Needle Amount", 5);

            insatiableDuration = Config.BindAndOptions(section, "Change Insatiable Duration", 7f);

            passiveScaling = Config.BindAndOptions(section, "Change Passive Scaling", 0.04f, "0.04 extra base damage per 1 missing health. EX: 0.04 * 200 missing health = 8 extra base damage.");

            passiveLifeSteal = Config.BindAndOptions(section, "Change Passive Lifesteal Scaling", 0.03f, "0.03 == up to 30% lifesteal based on missing health.");

            parryDamageCoefficient = Config.BindAndOptions(section, "Change Parry Damage Coefficient", 4f);

            needleDamageCoefficient = Config.BindAndOptions(section, "Change Needle Damage Coefficient", 0.4f);

            clipDamageCoefficient = Config.BindAndOptions(section, "Change Clip Damage Coefficient", 0.7f);

            needleProcCoefficient = Config.BindAndOptions(section, "Change Needle Proc Coefficient", 0.6f);

            parryWindow = Config.BindAndOptions(section, "Change Parry Window Duration", 1f);

            scissorDamageCoefficient = Config.BindAndOptions(section, "Change Scissor Damage Coefficient", 4f);

            telekinesisDamageCoefficient = Config.BindAndOptions(section, "Change Telekinesis Damage Coefficient", 0.6f);

            telekinesisCooldown = Config.BindAndOptions(section, "Change Telekinesis Cooldown", 6f);

            explodeDamageCoefficient = Config.BindAndOptions(section, "Change Explode Damage Coefficient", 4f);


            heavyEnemy = Config.BindAndOptions(
                section2,
                "Pickup Big Enemies",
                false,
                "Allows you to pickup bigger enemies for fun.");

            forceUnlockCharacter = Config.BindAndOptions(
                section2,
                "Force Unlock Seamstress",
                false,
                "Cheater...");

            forceUnlockRaven = Config.BindAndOptions(
                section2,
                "Force Unlock Raven",
                false,
                "If you're not Feilong, you're a cheater.");


            /*
            changeGroundedBlinkVelocity = Config.BindAndOptionsSlider(
                section,
                "Grounded Blink Jump Height",
                0.25f,
                "When grounded, blink will go vertically by the inputted amount from 0 - 1 (0 degrees to 90 degrees)", 
                0, 
                1,
                false);
            */
        }
    }
}
