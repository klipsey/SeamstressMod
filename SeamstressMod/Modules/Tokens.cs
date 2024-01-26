using SeamstressMod.Survivors.Seamstress;

namespace SeamstressMod.Modules
{
    internal static class Tokens
    {
        public const string agilePrefix = "<style=cIsUtility>Agile</style>";

        public const string cutPrefix = "<color=#9B3737>Cut</color>";

        public const string stitchPrefix = "<color=#9B3737>Stitch</color>";

        public const string armorPrefix = "<style=cIsUtility>20 armor</style>";

        public const string needlePrefix = "<style=cIsUtility>Needle</style>";

        public const string halfHealthPrefix = "<style=cIsHealth>50% HP</style>";

        public const string healthCostPrefix = "<style=cIsHealth>X% of your current health</style>";

        public const string butcheredPrefix = "<style=cIsUtility>Butchered</style>";

        public static string cutKeyword = KeywordText("Cut", $"Gain a <style=cIsUtility>Needle</style> on hit. If the enemy is <color=#9B3737>Stitched</color>, lower cooldowns by {SeamstressStaticValues.cutCooldownReduction}" +
            $"Also, consume <color=#9B3737>Stitched</color> dealing <style=cIsDamage>{100 * SeamstressStaticValues.cutsumeMissingHpDamage}%</style> of the enemies missing <style=cIsHealth>health</style> per <color=#9B3737>Stitched</color>.");

        public static string stitchKeyword = KeywordText("Stitch", $"Deal <style=cIsDamage>{100f * SeamstressStaticValues.stitchDamageCoefficient}</style>% damage. Apply Stitched which " +
            $"deals <style=cIsDamage>{1000f * (4 * SeamstressStaticValues.stitchedDotDamage)}%</style> (<style=cIsDamage>{1000f * (4 * SeamstressStaticValues.stitchedDotBossDamage)}%</style> against bosses) of the enemies missing <style=cIsHealth>health</style> over 4 seconds.");
       
        public static string agileKeyword = KeywordText("Agile", "The skill can be used while sprinting.");

        public static string needleKeyword = KeywordText("Needle", "Hold up to 10 <style=cIsUtility>Needles</style> before auto firing needles.");

        public static string sewHelp = KeywordText("Sew", "Always fire 1 Needle.");

        public static string healthCostKeyword = KeywordText("X% HP", "The skill costs " + healthCostPrefix + ".");

        public static string butcheredKeyword = KeywordText("Butchered", "Empower abilities. Gain movement and attack speed. " +
            "During " + Tokens.butcheredPrefix +  $" convert <style=cIsHealing>{100f * (1 - SeamstressStaticValues.healConversion)}% of healing</style> into damage for <color=#9B3737>Expunge</color>.");
        public static string DamageText(string text)
        {
            return $"<style=cIsDamage>{text}</style>";
        }
        public static string DamageValueText(float value)
        {
            return $"<style=cIsDamage>{value * 100}% damage</style>";
        }
        public static string UtilityText(string text)
        {
            return $"<style=cIsUtility>{text}</style>";
        }
        public static string RedText(string text) => HealthText(text);
        public static string HealthText(string text)
        {
            return $"<style=cIsHealth>{text}</style>";
        }
        public static string KeywordText(string keyword, string sub)
        {
            return $"<style=cKeywordName>{keyword}</style><style=cSub>{sub}</style>";
        }
        public static string ScepterDescription(string desc)
        {
            return $"\n<color=#d299ff>SCEPTER: {desc}</color>";
        }

        public static string GetAchievementNameToken(string identifier)
        {
            return $"ACHIEVEMENT_{identifier.ToUpperInvariant()}_NAME";
        }
        public static string GetAchievementDescriptionToken(string identifier)
        {
            return $"ACHIEVEMENT_{identifier.ToUpperInvariant()}_DESCRIPTION";
        }
    }
}