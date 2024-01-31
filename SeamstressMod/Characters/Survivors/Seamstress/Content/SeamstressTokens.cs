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
             + "< ! > Woven Fate is your main mobility. Build up stitches on enemies to keep the cooldown low." + Environment.NewLine + Environment.NewLine
             + "< ! > Bloodsoaked Path can give you incredible buffs through butcher but use it carefully, you could preemptively end your run." + Environment.NewLine + Environment.NewLine
             + "< ! > Don't forget to heal during butchered, it can give you incredible burst damage with Expunge." + Environment.NewLine + Environment.NewLine
             + "< ! > Use Threaded Volley to fire your well needles for devestating group damage." + Environment.NewLine + Environment.NewLine;

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
            Language.Add(prefix + "PASSIVE_DESCRIPTION", $"Gain <style=cIsDamage>{SeamstressStaticValues.passiveScaling}</style> base damage for every <style=cIsHealth>1</style> health missing. " +
                $"Regenerate <color=#9B3737>Needles</color> overtime. Gain additional <color=#9B3737>Needles</color> when <color=#9B3737>Stitch</color> is torn.");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_TRIM_NAME", "Trimming Slashes");
            Language.Add(prefix + "PRIMARY_TRIM_DESCRIPTION", Tokens.agilePrefix + $". Slash in front for <style=cIsDamage>{100f * SeamstressStaticValues.scissorsDamageCoefficient}% damage</style>. " +
              "Every 2nd hit applies " + Tokens.stitchPrefix + ". Every 3rd hit applies " + Tokens.cutPrefix + ". While " + Tokens.butcheredPrefix + 
              $", every slash additionally applies " + Tokens.stitchPrefix + ".");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_WEAVE_NAME", "Woven Fate");
            Language.Add(prefix + "SECONDARY_WEAVE_DESCRIPTION", Tokens.cutPrefix + $". Dash forward, dealing <style=cIsDamage>{100f * SeamstressStaticValues.weaveDamageCoefficient}% damage</style>." +
                " While " + Tokens.butcheredPrefix + $", the <style=cIsUtility>cooldown</style> is halved and <style=cIsHealing>heal for {100 * SeamstressStaticValues.weaveLifeSteal}% of damage dealt</style>.");
            #endregion

            #region Utility
            Language.Add(prefix + "UTILITY_REAP_NAME", "Bloodsoaked Path");
            Language.Add(prefix + "UTILITY_REAP_DESCRIPTION", Tokens.halfHealthPrefix + $". Stab your heart gaining " + Tokens.butcheredPrefix + 
                ". During " + Tokens.butcheredPrefix + ", <color=#9B3737>Bloodsoaked Path</color> becomes <color=#9B3737>Expunge</color>.");

            Language.Add(prefix + "UTILITY_EXPUNGE_NAME", "Expunge");
            Language.Add(prefix + "UTILITY_EXPUNGE_DESCRIPTION", Tokens.cutPrefix + ". Release stored <style=cIsHealing>healing</style> that you converted during " + Tokens.butcheredPrefix + " as a razor sharp blast of <style=cIsDamage>damage</style>.");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_SEW_NAME", "Threaded Volley");
            Language.Add(prefix + "SPECIAL_SEW_DESCRIPTION", $"Expel <color=#9B3737>Needles</color>. Hold up to 10 <color=#9B3737>Needles</color> before auto firing. " );
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(SeamstressMasteryAchievement.identifier), "Seamstress: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(SeamstressMasteryAchievement.identifier), "As Seamstress, beat the game or obliterate on Monsoon.");
            #endregion
            #endregion
        }
    }
}