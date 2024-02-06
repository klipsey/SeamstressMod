using SeamstressMod.Survivors.Seamstress;

namespace SeamstressMod.Modules
{
    internal static class Tokens
    {
        public const string agilePrefix = "<style=cIsUtility>Agile</style>";

        public const string cutPrefix = "<color=#9B3737>Cut</color>";

        public const string stitchPrefix = "<color=#9B3737>Stitches</color>";

        public const string needlePrefix = "<color=#9B3737>Needle</color>";

        public const string halfHealthPrefix = "<style=cIsHealth>50% HP</style>";

        public const string healthCostPrefix = "<style=cIsHealth>X% of your current health</style>";

        public const string butcheredPrefix = "<style=cIsUtility>Butchered</style>";

        public static string stitchKeyword = KeywordText("Stitch", $"Deal damage to tear open Stitches dealing <style=cIsDamage>{100 * SeamstressStaticValues.stitchBaseDamage}% damage</style> and applying " + cutPrefix + ". Also gain a " + needlePrefix + ".");

        public static string cutKeyword = KeywordText("Cut", $"Bleed enemies for <style=cIsDamage>1%</style> (<style=cIsDamage>0.5%</style> against bosses) of the " +
            $"enemies missing <style=cIsHealth>health</style> per second for {SeamstressStaticValues.cutDuration} seconds.");

        public static string agileKeyword = KeywordText("Agile", "The skill can be used while sprinting.");

        public static string needleKeyword = KeywordText("Needle", $"<color=#9B3737>Needles</color> pierce for <style=cIsDamage>{100f * SeamstressStaticValues.needleDamageCoefficient}% damage</style> each. " +
            $"<style=cIsHealing>Heal for {100f * SeamstressStaticValues.needleHealAmount}% of the damage dealt</style>.");

        public static string healthCostKeyword = KeywordText("X% HP", "The skill costs " + healthCostPrefix + ".");

        public static string butcheredKeyword = KeywordText("Butchered", "Gain movement speed and grant every ability " + cutPrefix +
            ". During " + Tokens.butcheredPrefix +  $" convert <style=cIsHealing>{100f * (1 - SeamstressStaticValues.healConversion)}% of healing</style> into damage for <color=#9B3737>Expunge</color>.");
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