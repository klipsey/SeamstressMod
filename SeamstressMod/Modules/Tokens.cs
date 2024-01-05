namespace SeamstressMod.Modules
{
    internal static class Tokens
    {
        public const string agilePrefix = "<style=cIsUtility>Agile</style>";

        public const string cutPrefix = "<style=cIsDamage>Cut</style>";

        public const string armorPrefix = "<style=cIsUtility>20 armor</style>";

        public const string frenzyPrefix = "<style=cIsUtility>Frenzy</style>";

        public const string bleedPrefix = "<style=cIsDamage>Bleed</style>";

        public const string halfHealthPrefix = "<style=cIsHealth>50% HP</style>";

        public const string healthCostPrefix = "<style=cIsHealth>X% of your current health</style>";

        public const string healingPrefix = "<style=cIsHealing>heal a % of damage dealt</style>";

        public const string butcheredPrefix = "<style=cIsUtility>Butchered</style>";

        public const string slayerPrefix = "<style=cIsDamage>Slayer</style>";

        public static string cutKeyword = KeywordText("Cut", "Deal 5% of the enemies current health.");

        public static string slayerKeyword = KeywordText("Slayer", "Deals up to <style=cIsDamage>3x</style> damage against low health enemies.");

        public static string agileKeyword = KeywordText("Agile", "The skill can be used while sprinting.");

        public static string bleedKeyword = KeywordText("Bleed", "Deal <style=cIsDamage>240% base damage</style> over time.");

        public static string healthCostKeyword = KeywordText("X% HP", "The skill costs " + healthCostPrefix + ".");

        public static string frenzyKeyword = KeywordText("Frenzy", "Gain attack speed and movement speed.");

        public static string butcheredKeyword = KeywordText("Butchered", "Empower other abilities.");
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