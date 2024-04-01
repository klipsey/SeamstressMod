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

            string desc = "Seamstress is a mobile survivor that uses her health for damage. She doesn't gain base damage per level so stack health items to take advantage of her Imp Touched Heart.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + "< ! > Needles are a great source of extra damage. Use them wisely for damage and mobility." + Environment.NewLine + Environment.NewLine
             + "< ! > Make sure not to use Clip in a sticky situation. Having backup Needles might just save you." + Environment.NewLine + Environment.NewLine
             + "< ! > Sate your Hunger as soon as you can while Insatiable. The stats and health it provides can halt a quick death." + Environment.NewLine + Environment.NewLine
             + "< ! > Glimpse of Corruption is a great way to boost your damage but be wary of where you leave your Heart." + Environment.NewLine + Environment.NewLine
             + "< ! > Use and retrieve your Sentient weapon often to keep up a balance of damage and mobility." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so she left, wanting to stitch more than just fabric.";
            string outroFailure = "..and so she vanished, with seams unsewn.";

            Language.Add(prefix + "NAME", "Seamstress");
            Language.Add(prefix + "DESCRIPTION", desc);
            Language.Add(prefix + "SUBTITLE", "The Tattered Maiden");
            Language.Add(prefix + "LORE", "Durrrr");
            Language.Add(prefix + "OUTRO_FLAVOR", outro);
            Language.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            Language.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            #region Passive
            Language.Add(prefix + "PASSIVE_NAME", "Imp Touched Heart");
            Language.Add(prefix + "PASSIVE_DESCRIPTION", $"Gain <style=cIsDamage>base damage</style> for every <style=cIsHealh>1 missing health</style>. " +
                $"<color=#9B3737>Blink</color> instead of jumping. <color=#9B3737>Needles</color> can be consumed to <color=#9B3737>Blink</color> in the air.");

            Language.Add(prefix + "GAUGE_NAME", "It Hungers");
            Language.Add(prefix + "GAUGE_DESCRIPTION", $"During " + Tokens.insatiablePrefix + $", <style=cIsHealing>healing</style> sates your <style=cIsHealth>Hunger</style>. " +
                $"Filling your <style=cIsHealth>Hunger</style> gives bonus stats and acts as a <style=cIsHealth>barrier</style>.");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_TRIM_NAME", "Trim");
            Language.Add(prefix + "PRIMARY_TRIM_DESCRIPTION", Tokens.sentiencePrefix + $". Slash in front for <style=cIsDamage>{100f * SeamstressStaticValues.trimDamageCoefficient}% damage</style>." +
              $" Every 3rd hit deals <style=cIsDamage>{100f * SeamstressStaticValues.trimThirdDamageCoefficient}% damage</style>.");

            Language.Add(prefix + "PRIMARY_FLURRY_NAME", "Flurry");
            Language.Add(prefix + "PRIMARY_FLURRY_DESCRIPTION", Tokens.sentiencePrefix + $". Slash in front for <style=cIsDamage>{100f * SeamstressStaticValues.flurryDamageCoefficient}% damage</style>.");
            #endregion
            
            #region Secondary
            Language.Add(prefix + "SECONDARY_PLANARSHIFT_NAME", "Planar Shift");
            Language.Add(prefix + "SECONDARY_PLANARSHIFT_DESCRIPTION", $"Consume all <color=#9B3737>Needles</color>. Temporarily shift and reappear dealing <style=cIsDamage>{100f * SeamstressStaticValues.sewAltDamageCoefficient}% damage</style>" +
                $" per <color=#9B3737>Needle</color> to surrounding enemies. " +
                $"<style=cIsHealing>Heal for {100f * SeamstressStaticValues.insatiableLifesSteal}% of the damage dealt</style>.");

            Language.Add(prefix + "SECONDARY_CLIP_NAME", "Clip");
            Language.Add(prefix + "SECONDARY_CLIP_DESCRIPTION", Tokens.sentiencePrefix +  $". Consume all <color=#9B3737>Needles</color>. Snip for <style=cIsDamage>2x{100f * SeamstressStaticValues.clipDamageCoefficient}% damage</style>. Snip 5 additional times per <color=#9B3737>Needle</color>. " +
                $"<style=cIsHealing>Heal for {100f * SeamstressStaticValues.clipLifeSteal}% of the damage dealt</style>.");

            Language.Add(prefix + "SECONDARY_PLANMAN_NAME", "Planar Manipulation");
            Language.Add(prefix + "SECONDARY_PLANMAN_DESCRIPTION", $"<color=#9B3737>Manipulate</color>. Slamming enemies at high speeds deals <style=cIsDamage>{100f * SeamstressStaticValues.telekinesisDamageCoefficient}% damage</style> in an area. " +
                $"<style=cIsUtility>{SeamstressStaticValues.telekinesisCooldown}</style> second cooldown on the same target.");

            #endregion

            #region Utility 
            Language.Add(prefix + "UTILITY_HEARTDASH_NAME", "Glimpse of Corruption");
            Language.Add(prefix + "UTILITY_HEARTDASH_DESCRIPTION", $"Tear out your <color=#9B3737>Heart</color> gaining " + Tokens.insatiablePrefix + $". Dash forward dealing " +
                $"<style=cIsDamage>{100f * SeamstressStaticValues.blinkDamageCoefficient}% damage</style>. Recasting or losing sated <style=cIsHealth>Hunger</style> returns you to your <color=#9B3737>Heart</color>.");

            Language.Add(prefix + "UTILITY_PARRY_NAME", "Glimpse of Purity");
            Language.Add(prefix + "UTILITY_PARRY_DESCRIPTION", $"Prepare a <style=cIsUtility>parry</style>. If successful, dash forward dealing <style=cIsDamage>{100f * SeamstressStaticValues.parryDamageCoefficient}% damage</style> and gain " +
               Tokens.insatiablePrefix + ".");

            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_FIRE_NAME", "Skewer");
            Language.Add(prefix + "SPECIAL_FIRE_DESCRIPTION", $"Fire a " + Tokens.sentiencePrefix + $" weapon dealing <style=cIsDamage>{100f * SeamstressStaticValues.scissorDamageCoefficient}% damage</style>. " +
                $"Pick up " + Tokens.sentiencePrefix + $" weapons to deal <style=cIsDamage>{100f * SeamstressStaticValues.scissorSlashDamageCoefficient}% damage</style> in an area.");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(SeamstressMasteryAchievement.identifier), "Seamstress: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(SeamstressMasteryAchievement.identifier), "As Seamstress, beat the game or obliterate on Monsoon.");

            Language.Add(Tokens.GetAchievementNameToken(SeamstressUnlockAchievement.identifier), "Just A Trim");
            Language.Add(Tokens.GetAchievementDescriptionToken(SeamstressUnlockAchievement.identifier), "Defeat a teleporter boss with bleed.");

            #endregion

            #endregion
        }
    }
}