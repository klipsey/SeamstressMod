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
             + "< ! > Trimming Slashes is a consistent way to apply Cut while Butchered is inactive." + Environment.NewLine + Environment.NewLine
             + "< ! > Woven Fate is your main mobility. Use the recast to reposition your self after you leap." + Environment.NewLine + Environment.NewLine
             + "< ! > Glimpse of Corruption can give you incredible damage through Butchered but use it carefully, you could preemptively end your run." + Environment.NewLine + Environment.NewLine
             + "< ! > Don't forget to heal during Butchered, it can give you incredible burst damage with Expunge." + Environment.NewLine + Environment.NewLine
             + "< ! > Use Threaded Volley to fire your needles for devestating group damage and healing." + Environment.NewLine + Environment.NewLine;

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
            Language.Add(prefix + "PASSIVE_DESCRIPTION", $"Gain <style=cIsDamage>{SeamstressStaticValues.passiveScaling}</style> base damage for every <style=cIsHealth>1</style> health or shield missing. ");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_TRIM_NAME", "Trimming Slashes");
            Language.Add(prefix + "PRIMARY_TRIM_DESCRIPTION", Tokens.agilePrefix + $". Slash in front for <style=cIsDamage>{100f * SeamstressStaticValues.trimDamageCoefficient}% damage</style>. " +
              $" Every 3rd hit deals <style=cIsDamage>{100f * SeamstressStaticValues.trimThirdDamageCoefficient}% damage</style> and " + Tokens.stitchPrefix + $" enemies.");

            Language.Add(prefix + "PRIMARY_FLURRY_NAME", "Unrelenting Flurry");
            Language.Add(prefix + "PRIMARY_FLURRY_DESCRIPTION", Tokens.agilePrefix + $". Slash in front for <style=cIsDamage>{100f * SeamstressStaticValues.flurryDamageCoefficient}% damage</style> applying " 
                + Tokens.stitchPrefix +" to enemies.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_SEW_NAME", "Threaded Volley");
            Language.Add(prefix + "SECONDARY_SEW_DESCRIPTION", $"Expel <color=#9B3737>Needles</color>. <color=#9B3737>Needles</color> pierce for <style=cIsDamage>{100f * SeamstressStaticValues.needleDamageCoefficient}% damage</style> each. " +
            $"<style=cIsHealing>Heal for {100f * SeamstressStaticValues.needleHealAmount}% of the damage dealt</style>.");

            Language.Add(prefix + "SECONDARY_ALTSEW_NAME", "Planar Shift");
            Language.Add(prefix + "SECONDARY_ALTSEW_DESCRIPTION", $"Consume all <color=#9B3737>Needles</color>. Temporarily shift and reappear dealing <style=cIsDamage>{100f * SeamstressStaticValues.sewAltDamageCoefficient}% damage</style>" +
                $" per <color=#9B3737>Needle</color> to surrounding enemies. " +
                $"<style=cIsHealing>Heal for {100f * SeamstressStaticValues.planarLifeSteal}% of the damage dealt</style>.");

            Language.Add(prefix + "SECONDARY_CLIP_NAME", "Clip");
            Language.Add(prefix + "SECONDARY_CLIP_DESCRIPTION", $"Consume all <color=#9B3737>Needles</color>. Snip up to 5 times based on <color=#9B3737>Needles</color> consumed dealing " +
                $"<style=cIsDamage>{100f * SeamstressStaticValues.clipDamageCoefficient}% damage</style> per <color=#9B3737>Needle</color> consumed. <style=cIsHealing>Heal for {100f * SeamstressStaticValues.planarLifeSteal}% of the damage dealt</style>.");
            #endregion

            #region Utility 
            Language.Add(prefix + "UTILITY_WEAVE_NAME", "Woven Fate");
            Language.Add(prefix + "UTILITY_WEAVE_DESCRIPTION", $"Leap in a direction dealing <style=cIsDamage>{100f * SeamstressStaticValues.weaveLeapDamageCoefficient}% damage</style>. " +
                $"Recast in the air to dash dealing <style=cIsDamage>{100f * SeamstressStaticValues.weaveDamageCoefficient}% damage</style>. Apply " + Tokens.stitchPrefix + " to enemies hit.");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_BLINK_NAME", "Glimpse of Corruption");
            Language.Add(prefix + "SPECIAL_BLINK_DESCRIPTION", Tokens.halfHealthPrefix + $". Blink in a direction dealing <style=cIsDamage>{100f * SeamstressStaticValues.blinkDamageCoefficient}% damage</style> applying " + Tokens.stitchPrefix + " to enemies. Gain "
                + Tokens.butcheredPrefix + ". While " + Tokens.butcheredPrefix + " <color=#9B3737>Glimpse of Corruption</color> becomes <color=#9B3737>Expunge</color>.");

            Language.Add(prefix + "SPECIAL_EXPUNGE_NAME", "Expunge");
            Language.Add(prefix + "SPECIAL_EXPUNGE_DESCRIPTION", Tokens.cutPrefix + ".  During " + Tokens.butcheredPrefix +  $" convert <style=cIsHealing>{100f * (1 - SeamstressStaticValues.healConversion)}% of healing</style> into damage for <color=#9B3737>Expunge</color>. Release stored <style=cIsHealing>healing</style> as a deadly blast of <style=cIsDamage>damage</style>.");

            Language.Add(prefix + "SPECIAL_PARRY_NAME", "Glimpse of Purity");
            Language.Add(prefix + "SPECIAL_PARRY_DESCRIPTION", $"Briefly become <style=cIsUtility>Invulnerable</style>. If hit, dash forward dealing <style=cIsDamage>2x{100f * SeamstressStaticValues.parryDamage}% damage</style> applying " + Tokens.stitchPrefix + ". Gain " +
               Tokens.butcheredPrefix + $". During " + Tokens.butcheredPrefix + $", gain base damage as if your health was at <style=cIsHealth>50%</style>. Missing refunds <style=cIsUtility>50%</style> of the cooldown.");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(SeamstressMasteryAchievement.identifier), "Seamstress: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(SeamstressMasteryAchievement.identifier), "As Seamstress, beat the game or obliterate on Monsoon.");
            #endregion
            #endregion
        }
    }
}