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
             + "< ! > Reap can give you incredible buffs through butcher but use it carefully, you could preemptively end your run." + Environment.NewLine + Environment.NewLine
             + "< ! > Reap what you.. Sew? Use Sew to fire your needles to hit enemies from afar." + Environment.NewLine + Environment.NewLine;

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
            Language.Add(prefix + "PASSIVE_NAME", "<style=cIsHealth>Stitched Heart</style>");
            Language.Add(prefix + "PASSIVE_DESCRIPTION", "Gain <style=cIsDamage>0.1</style> base damage for every <style=cIsHealth>1</style> health missing. Abilities grant <style=cIsUtility>Needles</style>.");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_TRIM_NAME", "<style=cIsHealth>Trim</style>");
            Language.Add(prefix + "PRIMARY_TRIM_DESCRIPTION", Tokens.agilePrefix + $". Slash for <style=cIsDamage>{100f * SeamstressStaticValues.scissorsDamageCoefficient}% damage</style>. On every third hit, " +
               Tokens.cutPrefix + " and gain a " + Tokens.needlePrefix + " for each enemy hit. " + "While " + Tokens.butcheredPrefix + $" gain <style=cIsHealing>Barrier</style> on hit and increase damage of the third hit to <style=cIsDamage>{100f * SeamstressStaticValues.scissorsBonusDamageCoefficient}% damage</style>.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_WEAVE_NAME", "<style=cIsHealth>Weave</style>");
            Language.Add(prefix + "SECONDARY_WEAVE_DESCRIPTION", $"Dash forward, dealing <style=cIsDamage>{100f * SeamstressStaticValues.weaveDamageCoefficient}% damage</style>. On kill, return a stock to <style=cIsHealth>Weave</style> and gain a " + Tokens.needlePrefix + ". While " + Tokens.butcheredPrefix +", gain " + Tokens.slayerPrefix + " and fully reset cooldown on kill.");
            #endregion

            #region Utility
            Language.Add(prefix + "UTILITY_REAP_NAME", "<style=cIsHealth>Reap</style>");
            Language.Add(prefix + "UTILITY_REAP_DESCRIPTION", Tokens.halfHealthPrefix + $". Stab your heart to gain " + Tokens.butcheredPrefix + " and a "+ Tokens.needlePrefix + ".");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_SEW_NAME", "<style=cIsHealth>Sew</style>");
            Language.Add(prefix + "SPECIAL_SEW_DESCRIPTION", $"Detonate your heart dealing <style=cIsDamage>{100f * SeamstressStaticValues.sewDamageCoefficient}% damage</style>." +
                " Expels <style=cIsUtility>Needles</style> that " + Tokens.cutPrefix + $" and deal <style=cIsDamage>{100f * SeamstressStaticValues.sewNeedleDamageCoefficient}% damage each</style>. While " + 
                Tokens.butcheredPrefix + ", your heart <style=cIsUtility>Cleanses</style> you.");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(SeamstressMasteryAchievement.identifier), "Seamstress: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(SeamstressMasteryAchievement.identifier), "As Seamstress, beat the game or obliterate on Monsoon.");
            #endregion
            #endregion
        }
    }
}