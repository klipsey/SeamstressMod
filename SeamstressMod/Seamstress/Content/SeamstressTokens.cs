﻿using System;
using SeamstressMod.Modules;
using SeamstressMod.Seamstress;
using SeamstressMod.Seamstress.Achievements;

namespace SeamstressMod.Seamstress.Content
{
    public static class SeamstressTokens
    {
        public static void Init()
        {
            AddSeamstressTokens();

            ////uncomment this to spit out a lanuage file with all the above tokens that people can translate
            ////make sure you set Language.usingLanguageFolder and printingEnabled to true
            Language.PrintOutput("Seamstress.txt");
            //todo guide
            ////refer to guide on how to build and distribute your mod with the proper folders
        }

        public static void AddSeamstressTokens()
        {
            #region Seamstress
            string prefix = SeamstressSurvivor.SEAMSTRESS_PREFIX;

            string desc = "The Seamstress is a mobile survivor that uses her health for damage. She gains less base damage per level so stack health items to take advantage her health to damage conversion.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + "< ! > Needles are a great source of extra damage. Use them wisely for damage and mobility." + Environment.NewLine + Environment.NewLine
             + "< ! > Make sure to use Clip with as many needles as you can. It can rapidly apply bleed during Insatiable." + Environment.NewLine + Environment.NewLine
             + "< ! > Heal as much as you can during Insatiable. The barrier it provides could be life or death." + Environment.NewLine + Environment.NewLine
             + "< ! > Brutalize is a great way to boost your damage but be wary of your health." + Environment.NewLine + Environment.NewLine
             + "< ! > Use and retrieve your Symbiotic scissors to gain needles outside of Insatiable." + Environment.NewLine + Environment.NewLine;

            /*
             * its not really shown yet in the anims but the panel on the top of the robe/heart area opens up to her artificial heart. 
             * the lore is something along the lines of her being a human who was plagued with illness during her life forcing her to have an artificial heart. 
             * while traveling on a civilian ship, the ship was intercepted by imps attempting to escape petrichor V causing it 
             * to be sent into the red plane where everyone in the ship either died immediately to the dimension or killed off by imps. 
             * Due to her heart being artificial, she didnt immediately die but instead got corrupted by the plane instead. I was thinking that 
             * maybe she IS killed in the plane by another survivor with her heart corrupting her but idk about that yet. For the scissors all 
             * i know is that they are sentient but i dont really have anything for why they would be 
            */
            string lore = "Pale skin and fatigue. Signs of a disease spreading within children of the high court. How unfortunate they are." +
                Environment.NewLine +
                Environment.NewLine +
                "A sickly infant, frail and weak, cries as her parents do too. A tragic fate for these aging nobles as she is their only heir. With a shaky future, her <style=cIsHealth>heart</style> still beats." +
                Environment.NewLine +
                Environment.NewLine +
                "The disease is curable with an artificial heart but the operation cannot be done on those who haven't reached adulthood. Only few are lucky enough to fall within that timeframe." +
                Environment.NewLine +
                Environment.NewLine +
                "A bedridden teen, her health quickly fading. She holds on for dear life. She needs to survive one more week. Hanging on by a thread, her <color=#292525>heart</color> still beats." +
                Environment.NewLine +
                Environment.NewLine +
                "Although an artificial heart cures the disease, one's day-to-day remains the same. With frequent maintenance and risks of malfunctioning, venturing far from home could spell death. " +
                "Many take up slower hobbies like baking and sewing but not everyone is content with their lives." +
                Environment.NewLine +
                Environment.NewLine +
                "A restless seamstress, with synthetic vitality, despises the walls she resides in. She wants nothing to do with her nobility. It ties her down to a monotonous life. " +
                "As darkness settles, she stows away in a civilian ship. Filled with adrenaline her <style=cIsUtility>heart</style> still beats. " +
                Environment.NewLine +
                Environment.NewLine +
                "Many who run away are given a reality check. The galaxy outside of the high court isn't the wonderland depicted in books. It's more akin to a tragic fairytale. " +
                Environment.NewLine +
                Environment.NewLine +
                "A corrupted creature, on borrowed time. How did it go so wrong? The ship she chose -swallowed by a rift made by imps who desired the same escape as her. " +
                "Suddenly, a wave of blood blended with the crimson sky, every human and imp alike was cut clean. As her vision began to fade, a figure lept away as another one quietly approached. " +
                "A whisper, a target, a new life. Her <color=#9B3737>heart</color> still beats.";

            string outro = "..and so she left, wanting to stitch more than just fabric.";
            string outroFailure = "..and so she vanished, with seams unsewn.";
            
            Language.Add(prefix + "NAME", "Seamstress");
            Language.Add(prefix + "DESCRIPTION", desc);
            Language.Add(prefix + "SUBTITLE", "The Tattered Maiden");
            Language.Add(prefix + "LORE", lore);
            Language.Add(prefix + "OUTRO_FLAVOR", outro);
            Language.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            Language.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            Language.Add(prefix + "MASTERY_SKIN_NAME2", "Alternate2");
            Language.Add(prefix + "MASTERY_SKIN_NAME3", "Raven");
            Language.Add(prefix + "MASTERY_SKIN_NAME4", "Raven Alternate");
            #endregion

            #region Passive
            Language.Add(prefix + "PASSIVE_NAME", "It Hungers");
            Language.Add(prefix + "PASSIVE_DESCRIPTION", $"Gain <style=cIsDamage>base damage</style> and <style=cIsHealing>healing</style> based on <style=cIsHealth>missing health</style>.");

            Language.Add(prefix + "NEEDLE_NAME", "Imp Touched Heart");
            Language.Add(prefix + "NEEDLE_DESCRIPTION", $"Gain <color=#9B3737>Needles</color> from picking up " + Tokens.symbioticPrefix + $" scissors or attacking with your primary skill while " + 
                Tokens.insatiablePrefix + $". <color=#9B3737>Needles</color> let you <color=#9B3737>Blink</color> mid air and will fire out dealing " +
                $"<style=cIsDamage>{100f * SeamstressConfig.needleDamageCoefficient.Value}% damage</style>.");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_TRIM_NAME", "Trim");
            Language.Add(prefix + "PRIMARY_TRIM_DESCRIPTION", Tokens.bonusStrike + $". Slash in front for <style=cIsDamage>" +
                $"{100f * SeamstressConfig.trimDamageCoefficient.Value}% + {100f * SeamstressConfig.scissorSlashDamageCoefficient.Value}% damage</style>." +
              $" Every 3rd hit deals <style=cIsDamage>{100f * SeamstressConfig.trimThirdDamageCoefficient.Value}% + " +
              $"{100f * SeamstressConfig.scissorSlashDamageCoefficient.Value}% damage</style>.");

            Language.Add(prefix + "PRIMARY_FLURRY_NAME", "Flurry");
            Language.Add(prefix + "PRIMARY_FLURRY_DESCRIPTION", Tokens.bonusStrike + $". Slash in front for <style=cIsDamage>" +
                $"{100f * SeamstressConfig.flurryDamageCoefficient.Value}% + {100f * SeamstressConfig.scissorSlashDamageCoefficient.Value}% damage</style>.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_CLIP_NAME", "Clip");
            Language.Add(prefix + "SECONDARY_CLIP_DESCRIPTION", $"Snip for <style=cIsDamage>2x{100f * SeamstressConfig.clipDamageCoefficient.Value}% damage</style>. " +
                $"Snip an additional time per <color=#9B3737>Needle</color>.");

            Language.Add(prefix + "SECONDARY_PLANMAN_NAME", "Planar Manipulation");
            Language.Add(prefix + "SECONDARY_PLANMAN_DESCRIPTION", Tokens.crushPrefix + $". Hold to grab enemies. Slamming the enemy deals <style=cIsDamage>{100f * SeamstressConfig.telekinesisDamageCoefficient.Value} - {100f * SeamstressConfig.telekinesisDamageCoefficient.Value * 5}% damage</style> " +
                $"in an area based on <color=#9B3737>Needle</color> count. " +
                $"<style=cIsUtility>{SeamstressConfig.telekinesisCooldown} second cooldown</style> on the same target.");

            #endregion

            #region Utility 
            Language.Add(prefix + "UTILITY_HEARTDASH_NAME", "Brutalize");
            Language.Add(prefix + "UTILITY_HEARTDASH_DESCRIPTION", $"Tear out your <color=#9B3737>heart</color> and become " + Tokens.insatiablePrefix + $". Dash forward dealing " +
                $"<style=cIsDamage>{100f * SeamstressConfig.blinkDamageCoefficient.Value}% damage</style>. Recast to explode for <style=cIsDamage>{100f * SeamstressConfig.explodeDamageCoefficient.Value}% damage</style>.");

            Language.Add(prefix + "UTILITY_PARRY_NAME", "Retaliate");
            Language.Add(prefix + "UTILITY_PARRY_DESCRIPTION", $"Prepare a <style=cIsUtility>parry</style>. If successful, dash forward dealing <style=cIsDamage>{100f * SeamstressConfig.parryDamageCoefficient.Value}% damage</style> and gain " +
               Tokens.insatiablePrefix + ".");

            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_FIRE_NAME", "Skewer");
            Language.Add(prefix + "SPECIAL_FIRE_DESCRIPTION", $"<style=cIsHealth>15% HP.</style>. Fire a " + Tokens.symbioticPrefix + $" scissor dealing <style=cIsDamage>{100f * SeamstressConfig.scissorDamageCoefficient.Value}% damage</style>. " +
                $"Pick up " + Tokens.symbioticPrefix + $" scissors to slash for <style=cIsDamage>{100f * SeamstressConfig.scissorPickupDamageCoefficient.Value}% damage</style>.");

            Language.Add(prefix + "SPECIAL_SCEPTER_NAME", "Rampage");
            Language.Add(prefix + "SPECIAL_SCEPTER_DESCRIPTION", $"<style=cIsHealth>15% HP.</style>. Fire a " + Tokens.symbioticPrefix + $" scissor dealing <style=cIsDamage>{100f * SeamstressConfig.scissorDamageCoefficient.Value}% damage</style>. " +
                $"Pick up " + Tokens.symbioticPrefix + $" scissors to slash for <style=cIsDamage>{100f * SeamstressConfig.scissorPickupDamageCoefficient.Value}% damage</style>." + Tokens.ScepterDescription("Your " + Tokens.symbioticPrefix + $" weapon only has a <style=cIsUtility>1.5 second pickup time</style>."));

            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(SeamstressMasteryAchievement.identifier), "Seamstress: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(SeamstressMasteryAchievement.identifier), "As Seamstress, beat the game or obliterate on Monsoon.");

            Language.Add(Tokens.GetAchievementNameToken(SeamstressRavenUnlockAchievement.identifier), "Raven: Unlocked");
            Language.Add(Tokens.GetAchievementDescriptionToken(SeamstressRavenUnlockAchievement.identifier), "As Seamstress, beat the game or obliterate on Eclipse or higher.");

            //Language.Add(Tokens.GetAchievementNameToken(SeamstressUnlockAchievement.identifier), "Just A Trim");
            //Language.Add(Tokens.GetAchievementDescriptionToken(SeamstressUnlockAchievement.identifier), "Defeat a teleporter boss with bleed.");

            #endregion

            #endregion
        }
    }
}