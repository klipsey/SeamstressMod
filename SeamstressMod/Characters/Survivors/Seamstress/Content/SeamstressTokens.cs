using System;
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

            string desc = "Seamstress is a mobile survivor that uses her health for damage. She doesn't gain base damage per level so stack health items to take advantage of her passive.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + "< ! > Trim is a consistent way to deal damage while full hp." + Environment.NewLine + Environment.NewLine
             + "< ! > Weave is a basic dash on its own but with butcher it becomes devastating in groups." + Environment.NewLine + Environment.NewLine
             + "< ! > Threads Of Fate can give you incredible buffs through butcher but use it carefully, you could preemptively end your run." + Environment.NewLine + Environment.NewLine
             + "< ! > Use Sew to fire your needles to hit enemies from afar." + Environment.NewLine + Environment.NewLine;

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
            Language.Add(prefix + "PASSIVE_NAME", "Corrupted Heart");
            Language.Add(prefix + "PASSIVE_DESCRIPTION", $"Gain <style=cIsDamage>{100f * SeamstressStaticValues.passiveScaling}% damage</style> base damage for every <style=cIsHealth>1</style> health missing. Certain ability conditions grant <style=cIsUtility>Needles</style>.");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_TRIM_NAME", "Trim");
            Language.Add(prefix + "PRIMARY_TRIM_DESCRIPTION", Tokens.agilePrefix + $". Slash in front for <style=cIsDamage>{100f * SeamstressStaticValues.scissorsDamageCoefficient}% damage</style>. " +
              "Every 3rd hit applies " + Tokens.stitchPrefix + ". While " + Tokens.butcheredPrefix + 
              $", the first two slashes apply " + Tokens.stitchPrefix + " and the third hit applies " + Tokens.cutPrefix);
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_WEAVE_NAME", "Weave");
            Language.Add(prefix + "SECONDARY_WEAVE_DESCRIPTION", Tokens.cutPrefix + $". Dash forward, dealing <style=cIsDamage>{100f * SeamstressStaticValues.weaveDamageCoefficient}% damage</style>." +
                " While " + Tokens.butcheredPrefix + ", applying <style=cIsHealing>heal for 20% of damage dealt</style>.");
            #endregion

            #region Utility
            Language.Add(prefix + "UTILITY_REAP_NAME", "Threads Of Fate");
            Language.Add(prefix + "UTILITY_REAP_DESCRIPTION", Tokens.halfHealthPrefix + $". Stab your heart gaining " + Tokens.butcheredPrefix + 
                ". During " + Tokens.butcheredPrefix + ", <color=#9B3737>Threads Of Fate</color> becomes <color=#9B3737>Expunge</color>.");

            Language.Add(prefix + "UTILITY_EXPUNGE_NAME", "Expunge");
            Language.Add(prefix + "UTILITY_EXPUNGE_DESCRIPTION", Tokens.cutPrefix + ". Release stored <style=cIsHealing>healing</style> that you converted during " + Tokens.butcheredPrefix + " as a razor sharp blast of <style=cIsDamage>damage</style>.");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_SEW_NAME", "Sew");
            Language.Add(prefix + "SPECIAL_SEW_DESCRIPTION", Tokens.stitchPrefix + $". Expel <style=cIsUtility>Needles</style> that pierce for <style=cIsDamage>{100f * SeamstressStaticValues.sewNeedleDamageCoefficient}% damage</style>  each. " +
                $"<style=cIsHealing>Heal you for {100f * SeamstressStaticValues.needleHealAmount} of the damage dealt</style>. While " + 
                Tokens.butcheredPrefix + ", <style=cIsUtility>Needles</style> <style=cIsUtility>Slow</style> enemies.");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(SeamstressMasteryAchievement.identifier), "Seamstress: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(SeamstressMasteryAchievement.identifier), "As Seamstress, beat the game or obliterate on Monsoon.");
            #endregion
            #endregion
        }
    }
}