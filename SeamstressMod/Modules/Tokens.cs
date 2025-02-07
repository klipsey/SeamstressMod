using SeamstressMod.Seamstress.Content;

namespace SeamstressMod.Modules
{
    internal static class Tokens
    {
        public const string agilePrefix = "<style=cIsUtility>Agile</style>";

        public const string symbioticPrefix = "<color=#9B3737>Symbiotic</color>";

        public const string bonusStrike = "<color=#9B3737>Double Hit</color>";

        public const string needlePrefix = "<color=#9B3737>Needle</color>";

        public const string insatiablePrefix = "<color=#9B3737>Insatiable</color>";

        public const string crushPrefix = "<color=#9B3737>Crush</color>";

        public static string agileKeyword = KeywordText("Agile", "The skill can be used while sprinting.");

        public static string crushKeyword = KeywordText("Crush", $"Deal bonus damage to an enemy based on their velocity and <style=cIsHealth>max health</style>.");

        public static string symbioticKeyword = KeywordText("Symbiotic", "Gain <style=cIsDamage>attack speed</style> and " +
            "<style=cIsUtility>move speed</style> for each stock missing. Additional stocks are converted into missing stock.");

        public static string symbioticSlashKeyword = KeywordText("Double Hit", "<style=cIsUtility>Stunning</style>" + $". When your " + 
            symbioticPrefix + $" scissor is available hit for an additional strike.");

        public static string insatiableKeyword = KeywordText("Insatiable", $"Attacks <style=cIsHealth>bleed</style> and grant <color=#9B3737>Needles</color>. " +
            $"<style=cIsHealing>Healing</style> is converted into <style=cIsHealing>Barrier</style>.");

        public static string detailsKeyword = KeywordText("Details", $"Gain <style=cIsDamage>{SeamstressConfig.passiveScaling} base damage</style> per <style=cIsHealth>1 missing health</style>. " +
            $"Heal up to <style=cIsHealing>{100f * SeamstressConfig.passiveHealingScaling.Value}%</style> of your <style=cIsHealth>missing health</style> on hit.");
        
        public static string needlesKeyword = KeywordText("Needles", $"Gain <color=#9B3737>Needles</color> by picking up your <color=#9B3737>Symbiotic</color> weapons " +
            $"or hitting enemies during <color=#9B3737>Insatiable</color>.");
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