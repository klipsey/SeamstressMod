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
             + "< ! > Trimming Slashes is a consistent way to deal damage while full hp." + Environment.NewLine + Environment.NewLine
             + "< ! > Woven Fate is your main mobility. Apply Cuts to keep the cooldown low." + Environment.NewLine + Environment.NewLine
             + "< ! > Planar Shift can give you incredible damage through Butchered but use it carefully, you could preemptively end your run." + Environment.NewLine + Environment.NewLine
             + "< ! > Don't forget to heal during Butchered, it can give you incredible burst damage with Expunge." + Environment.NewLine + Environment.NewLine
             + "< ! > Use Threaded Volley to fire your needles for devestating group damage." + Environment.NewLine + Environment.NewLine;

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
            Language.Add(prefix + "PASSIVE_NAME", "Stitched Heart");
            Language.Add(prefix + "PASSIVE_DESCRIPTION", $"Gain <style=cIsDamage>{SeamstressStaticValues.passiveScaling}</style> base damage for every <style=cIsHealth>1</style> health missing. ");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_TRIM_NAME", "Trimming Slashes");
            Language.Add(prefix + "PRIMARY_TRIM_DESCRIPTION", Tokens.agilePrefix + $". Slash in front for <style=cIsDamage>{100f * SeamstressStaticValues.trimDamageCoefficient}% damage</style>. " +
              " Every 3rd hit " + Tokens.stitchPrefix + $" in an area and deals <style=cIsDamage>{100f * SeamstressStaticValues.trimThirdDamageCoefficient}% damage</style>.");

            Language.Add(prefix + "PRIMARY_FLURRY_NAME", "Unrelenting Flurry");
            Language.Add(prefix + "PRIMARY_FLURRY_DESCRIPTION", Tokens.agilePrefix + $". Slash in front for <style=cIsDamage>{100f * SeamstressStaticValues.flurryDamageCoefficient}% damage</style>. " +
                $"<style=cIsHealing>Heal for {100 * SeamstressStaticValues.flurryLifeSteal}% of damage dealt</style>");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_WEAVE_NAME", "Woven Fate");
            Language.Add(prefix + "SECONDARY_WEAVE_DESCRIPTION", $"Leap in a direction dealing <style=cIsDamage>{100f * SeamstressStaticValues.weaveLeapDamageCoefficient}% damage</style>. " +
                $"Recast in the air to dash dealing <style=cIsDamage>{100f * SeamstressStaticValues.weaveDamageCoefficient}% damage</style>. Both casts apply " + Tokens.stitchPrefix + " on hit enemies.");
            #endregion

            #region Utility
            Language.Add(prefix + "UTILITY_BLINK_NAME", "Planar Shift");
            Language.Add(prefix + "UTILITY_BLINK_DESCRIPTION", Tokens.halfHealthPrefix + $". Blink in a direction dealing <style=cIsDamage>{100f * SeamstressStaticValues.blinkDamageCoefficient}% damage</style> applying " + Tokens.stitchPrefix + " to enemies. Gain " 
                + Tokens.butcheredPrefix + ". While " + Tokens.butcheredPrefix + " <color=#9B3737>Planar Shift</color> becomes <color=#9B3737>Expunge</color>");

            Language.Add(prefix + "UTILITY_EXPUNGE_NAME", "Expunge");
            Language.Add(prefix + "UTILITY_EXPUNGE_DESCRIPTION", Tokens.cutPrefix + ". Release stored <style=cIsHealing>healing</style> that you converted during " + Tokens.butcheredPrefix + " as a razor sharp blast of <style=cIsDamage>damage</style>.");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_SEW_NAME", "Threaded Volley");
            Language.Add(prefix + "SPECIAL_SEW_DESCRIPTION", $"Expel <color=#9B3737>Needles</color>. Hold up to 10 <color=#9B3737>Needles</color> before auto firing. Gain <color=#9B3737>Needles</color> by tearing "
                + Tokens.stitchPrefix + " or killing enemies.");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(SeamstressMasteryAchievement.identifier), "Seamstress: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(SeamstressMasteryAchievement.identifier), "As Seamstress, beat the game or obliterate on Monsoon.");
            #endregion
            #endregion
        }
    }
}