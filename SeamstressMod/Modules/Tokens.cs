using SeamstressMod.Seamstress.Content;

namespace SeamstressMod.Modules
{
    internal static class Tokens
    {
        public const string agilePrefix = "<style=cIsUtility>Agile</style>";

        public const string reachPrefix = "<color=#9B3737>Symbiotic Reach</color>";

        public const string symbioticPrefix = "<color=#9B3737>Symbiotic</color>";

        public const string symbioticSlashPrefix = "<color=#9B3737>Symbiotic Slash</color>";

        public const string needlePrefix = "<color=#9B3737>Needle</color>";

        public const string insatiablePrefix = "<color=#9B3737>Insatiable</color>";

        public const string crushPrefix = "<color=#9B3737>Crush</color>";

        public static string agileKeyword = KeywordText("Agile", "The skill can be used while sprinting.");

        public static string crushKeyword = KeywordText("Crush", $"Deal bonus damage to an enemy based on their velocity and <style=cIsHealth>max health</style>.");

        public static string symbioticKeyword = KeywordText("Symbiotic", "Gain <style=cIsDamage>attack speed</style> and " +
            "<style=cIsUtility>move speed</style> for each stock missing.");

        public static string reachKeyword = KeywordText("Symbiotic Reach", "When your " + symbioticPrefix + " weapon is available increase the range of this " +
            "attack.");

        public static string symbioticSlashKeyword = KeywordText("Symbiotic Slash", $"When your " + symbioticPrefix + $" weapon is available hit for an additional strike " +
            $"dealing <style=cIsDamage>{100f * SeamstressStaticValues.scissorPickupDamageCoefficient}% damage</style>.");

        public static string insatiableKeyword = KeywordText("Insatiable", $"Become <style=cIsDamage>frenzied</style> causing your attacks to <style=cIsHealth>bleed</style>. " +
            $"<style=cIsHealing>Healing</style> is converted into extra layer of <style=cIsHealth>health</style> that <style=cIsHealing>heals</style> you after <color=#9B3737>Insatiable</color> ends.");

        public static string detailsKeyword = KeywordText("Details", $"Gain <style=cIsDamage>{SeamstressStaticValues.passiveScaling} base damage</style> per <style=cIsHealth>1 missing health</style>. " +
            $"Gain up to <style=cIsHealing>{100f * SeamstressStaticValues.passiveHealingScaling}% lifsteal</style> based on <style=cIsHealth>missing health</style>.");
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