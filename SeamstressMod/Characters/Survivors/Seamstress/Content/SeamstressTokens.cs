﻿using System;
using SeamstressMod.Modules;
using SeamstressMod.Survivors.Seamstress.Achievements;

namespace SeamstressMod.Survivors.Seamstress
{
    public static class SeamstressTokens
    {
        public static void Init()
        {
            AddSeamstressTokens();

            ////uncomment this to spit out a lanuage file with all the above tokens that people can translate
            ////make sure you set Language.usingLanguageFolder and printingEnabled to true
            //Language.PrintOutput("Seamstress.txt");
            //todo guide
            ////refer to guide on how to build and distribute your mod with the proper folders
        }

        public static void AddSeamstressTokens()
        {
            #region Seamstress
            string prefix = SeamstressSurvivor.SEAMSTRESS_PREFIX;

            string desc = "Seamstress uses her health for damage. She doesn't gain base damage per level so stack health items to take advantage of her passive.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + "< ! > Trim is a consistent way to deal damage while full hp." + Environment.NewLine + Environment.NewLine
             + "< ! > Weave is a basic dash on its own but with butcher it can become devastating in groups." + Environment.NewLine + Environment.NewLine
             + "< ! > Reap can give you incredible buffs through butcher but use it carefully, you could end your run if used poorly." + Environment.NewLine + Environment.NewLine
             + "< ! > Reap what you.. Sew? Use Sew to regain the health lost from Reap especially during butcher." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so she left, wanting to stitch more than just fabric.";
            string outroFailure = "..and so she vanished, with seams unsewn.";

            Language.Add(prefix + "NAME", "Seamstress");
            Language.Add(prefix + "DESCRIPTION", desc);
            Language.Add(prefix + "SUBTITLE", "The Tattered Maiden");
            Language.Add(prefix + "LORE", "sample lore");
            Language.Add(prefix + "OUTRO_FLAVOR", outro);
            Language.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            Language.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            #region Passive
            Language.Add(prefix + "PASSIVE_NAME", "Tortured Heart");
            Language.Add(prefix + "PASSIVE_DESCRIPTION", "Gain .1 base damage for every 1 health missing. ");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_TRIM_NAME", "Trim");
            Language.Add(prefix + "PRIMARY_TRIM_DESCRIPTION", Tokens.agilePrefix + $". Slash 2 times for <style=cIsDamage>3x{100f * SeamstressStaticValues.scissorsDamageCoefficient}% damage</style>. Then snip for <style=cIsDamage>" +
                $"{100f * SeamstressStaticValues.scissorsDamageCoefficient}% damage</style> and " + Tokens.cutPrefix +". While butchered, also apply " + Tokens.bleedPrefix +".");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_WEAVE_NAME", "Weave");
            Language.Add(prefix + "SECONDARY_WEAVE_DESCRIPTION", Tokens.slayerPrefix + $". Dash forward, dealing <style=cIsDamage>{100f * SeamstressStaticValues.weaveDamageCoefficient}% damage</style>. While " + Tokens.butcheredPrefix + ", reset the cooldown on kill and apply " + Tokens.bleedPrefix + ".");
            #endregion

            #region Utility
            Language.Add(prefix + "UTILITY_REAP_NAME", "Reap");
            Language.Add(prefix + "UTILITY_REAP_DESCRIPTION", Tokens.halfHealthPrefix + $". Stab your heart to gain " + Tokens.armorPrefix + ", " + Tokens.frenzyPrefix + ", and " + Tokens.butcheredPrefix + ".");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_SEW_NAME", "Sew");
            Language.Add(prefix + "SPECIAL_SEW_DESCRIPTION", $"Consume enemies in front of you dealing <style=cIsDamage>{100f * SeamstressStaticValues.sewDamageCoefficient}% damage</style> <style=cIsHealing>healing for 10% of the damage dealt.</style> While " + 
                Tokens.butcheredPrefix + ", <style=cIsHealing>heal for 20% of the damage dealt</style> and apply " + Tokens.bleedPrefix + ".");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(SeamstressMasteryAchievement.identifier), "Seamstress: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(SeamstressMasteryAchievement.identifier), "As Seamstress, beat the game or obliterate on Monsoon.");
            #endregion
            #endregion
        }
    }
}