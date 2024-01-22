using SeamstressMod.Survivors.Seamstress;

namespace SeamstressMod.Modules
{
    internal static class Tokens
    {
        public const string agilePrefix = "<style=cIsUtility>Agile</style>";

        public const string cutPrefix = "<style=cIsDamage>Cut</style>";

        public const string armorPrefix = "<style=cIsUtility>20 armor</style>";

        public const string needlePrefix = "<style=cIsUtility>Needle</style>";

        public const string bleedPrefix = "<style=cIsDamage>Bleed</style>";

        public const string halfHealthPrefix = "<style=cIsHealth>50% HP</style>";

        public const string healthCostPrefix = "<style=cIsHealth>X% of your current health</style>";

        public const string healingPrefix = "<style=cIsHealing>heal a % of damage dealt</style>";

        public const string butcheredPrefix = "<style=cIsUtility>Butchered</style>";

        public static string cutKeyword = KeywordText("Cut", $"Lower cooldowns by {SeamstressStaticValues.cutCooldownReduction} seconds. Deal <style=cIsDamage>{100f * SeamstressStaticValues.cutDamageCoefficient}%</style> (<style=cIsDamage>{100f * SeamstressStaticValues.cutBossDamageCoefficient}%</style> against bosses) of the enemies current <style=cIsHealth>health</style>");

        public static string slayerKeyword = KeywordText("Slayer", "Deals up to <style=cIsDamage>3x</style> damage against low health enemies.");

        public static string agileKeyword = KeywordText("Agile", "The skill can be used while sprinting.");

        public static string needleKeyword = KeywordText("Needle", "Hold up to 10 <style=cIsUtility>Needles</style> before auto firing needles.");

        public static string bleedKeyword = KeywordText("Bleed", "Deal <style=cIsDamage>240% base damage</style> over time.");

        public static string healthCostKeyword = KeywordText("X% HP", "The skill costs " + healthCostPrefix + ".");

        public static string butcheredKeyword = KeywordText("Butchered", "Empower other abilities. Gain attack speed and movement speed. During " + Tokens.butcheredPrefix +  $" convert <style=cIsHealth>{100f * (1 - SeamstressStaticValues.healConversion)}%</style> into damage for " + Tokens.butcheredPrefix + " <style=cIsHealth>Sew</style>");
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