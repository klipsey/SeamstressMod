using System;
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
            //Language.PrintOutput("Seamstress.txt");
            //todo guide
            ////refer to guide on how to build and distribute your mod with the proper folders
        }

        public static void AddSeamstressTokens()
        {
            #region Seamstress
            string prefix = SeamstressSurvivor.SEAMSTRESS_PREFIX;

            string desc = "The Seamstress is a mobile survivor that uses her health for damage. She doesn't gain base damage per level so stack health items to take advantage of her Imp Touched Heart.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + "< ! > Needles are a great source of extra damage. Use them wisely for damage and mobility." + Environment.NewLine + Environment.NewLine
             + "< ! > Make sure not to use Clip in a sticky situation. Having backup Needles might just save you." + Environment.NewLine + Environment.NewLine
             + "< ! > Sate your Hunger as soon as you can while Insatiable. The stats and health it provides can halt a quick death." + Environment.NewLine + Environment.NewLine
             + "< ! > Glimpse of Corruption is a great way to boost your damage but be wary of where you leave your Heart." + Environment.NewLine + Environment.NewLine
             + "< ! > Use and retrieve your Sentient weapon often to keep up a balance of damage and mobility." + Environment.NewLine + Environment.NewLine;

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
                "A sickly infant, frail and weak, cry as her parents do too. A tragic fate for these aging nobles as she is their only heir. With a shaky future, her <style=cIsHealth>heart</style> still beats." +
                Environment.NewLine +
                Environment.NewLine +
                "The disease is curable with an artificial heart but the operation cannot be done on those who haven't reached adulthood. Only few are lucky enough to fall within that timeframe." +
                Environment.NewLine +
                Environment.NewLine +
                "A bedridden princess, her health is fading. She holds on for dear life. She needs to survive one more week. Hanging on by a thread, her <color=#292525>heart</color> still beats." +
                Environment.NewLine +
                Environment.NewLine +
                "Although an artifical heart extends their lifespan, their day to day remains the same. Requiring constant maintenance with risks of malfunctioning, venturing far from home could spell death. " +
                "Many take up slower hobbies like baking and sewing but not everyone is content with their lives." +
                Environment.NewLine +
                Environment.NewLine +
                "A restless noble, with robotic vitality. Her <style=cIsUtility>heart</style> still beats. " +
                Environment.NewLine +
                Environment.NewLine +
                "Many who run away are given a reality check. The galaxy outside of the high court isn't the wonderland depicted in books. It's more akin to a tragic fairytale. " +
                "A corrupted creature, on borrowed time. Her <color=#9B3737>heart</color> still beats.";
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
            #endregion

            #region Passive
            Language.Add(prefix + "PASSIVE_NAME", "It Hungers");
            Language.Add(prefix + "PASSIVE_DESCRIPTION", $"Gain <style=cIsDamage>base damage</style> for every <style=cIsHealth>1 missing health</style>. " +
                $"Filling <style=cIsHealth>Hunger</style> during " + Tokens.insatiablePrefix + " grants <style=cIsUtility>move speed</style> and an additional layer of <style=cIsHealth>health</style>.");

            Language.Add(prefix + "NEEDLE_NAME", "Imp Touched Heart");
            Language.Add(prefix + "NEEDLE_DESCRIPTION", $"The Seamstress <color=#9B3737>Blinks</color> instead of jumping. <color=#9B3737>Needles</color> can be fired to <color=#9B3737>Blink</color> in the air dealing " +
                $"<style=cIsDamage>{100f * SeamstressStaticValues.needleDamageCoefficient}% damage</style>.");
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
            Language.Add(prefix + "SECONDARY_CLIP_DESCRIPTION", Tokens.sentiencePrefix + $". Consume all <color=#9B3737>Needles</color>. Snip for <style=cIsDamage>2x{100f * SeamstressStaticValues.clipDamageCoefficient}% damage</style>. Snip 5 additional times per <color=#9B3737>Needle</color>. " +
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