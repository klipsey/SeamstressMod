using SeamstressMod.Survivors.Seamstress;

namespace SeamstressMod.Modules
{
    internal static class Tokens
    {
        public const string agilePrefix = "<style=cIsUtility>Agile</style>";

        public const string sentiencePrefix = "<color=#9B3737>Sentient</color>";

        public const string needlePrefix = "<color=#9B3737>Needle</color>";

        public const string insatiablePrefix = "<style=cIsUtility>Insatiable</style>";

        public static string agileKeyword = KeywordText("Agile", "The skill can be used while sprinting.");

        public static string needleKeyword = KeywordText("Needle", $"Gain <color=#9B3737>Needles</color> by picking up your " 
            + sentiencePrefix + " weapon or when landing <color=#9B3737>Sentient</color> attacks without your weapon.");

        public static string manipulateKeyword = KeywordText("Manipulate", $"Deal bonus damage to an enemy based on their velocity and <style=cIsHealth>max health</style>.");

        public static string sentienceKeyword = KeywordText("Sentient", "Your <color=#9B3737>Sentient</color> weapon. Gain <style=cIsDamage>attackspeed</style> and " +
            "<style=cIsUtility>movespeed</style> for each stock missing.");

        public static string sentienceRangeKeyword = KeywordText("Sentient", "When your " + sentiencePrefix + " weapon is available increase the range of this " +
            "attack.");

        public static string sentienceAttackKeyword = KeywordText("Sentient", $"When your " + sentiencePrefix + $" weapon is available hit for an additional strike " +
            $"dealing <style=cIsDamage>{100f * SeamstressStaticValues.scissorSlashDamageCoefficient}%</style> damage.");

        public static string insatiableKeyword = KeywordText("Insatiable", $"Attacks <style=cIsHealing>lifesteal</style> and <style=cIsHealth>bleed</style> on hit.");
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