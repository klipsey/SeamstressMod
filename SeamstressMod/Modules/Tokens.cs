using SeamstressMod.Survivors.Seamstress;

namespace SeamstressMod.Modules
{
    internal static class Tokens
    {
        public const string agilePrefix = "<style=cIsUtility>Agile</style>";

        public const string cutPrefix = "<color=#9B3737>Cut</color>";

        public const string stitchPrefix = "<color=#9B3737>Stitch</color>";

        public const string armorPrefix = "<style=cIsUtility>20 armor</style>";

        public const string needlePrefix = "<color=#9B3737>Needle</color>";

        public const string halfHealthPrefix = "<style=cIsHealth>50% HP</style>";

        public const string healthCostPrefix = "<style=cIsHealth>X% of your current health</style>";

        public const string butcheredPrefix = "<style=cIsUtility>Butchered</style>";

        public static string stitchKeyword = KeywordText("Stitch", $"Apply <color=#9B3737>Stitches</color>. Applying <color=#9B3737>Stitches</color> or " +
            $"tearing open <color=#9B3737>Stitches</color> reduce cooldowns by {SeamstressStaticValues.stitchCooldownReduction}.");

        public static string cutKeyword = KeywordText("Cut", $"Tear open <color=#9B3737>Stitches</color> " +
            $"dealing <style=cIsDamage>2.5%</style> (<style=cIsDamage>1.25%</style> against bosses) of the enemies missing health and apply <color=#9B3737>Cuts</color>. " +
            $"<color=#9B3737>Cuts</color> deal <style=cIsDamage>0.5%</style> (<style=cIsDamage>0.25%</style> against bosses) of the " +
            $"enemies missing <style=cIsHealth>health</style> per second for {SeamstressStaticValues.cutDuration} seconds. ");

        public static string agileKeyword = KeywordText("Agile", "The skill can be used while sprinting.");

        public static string needleKeyword = KeywordText("Needle", Tokens.stitchPrefix + $". <color=#9B3737>Needles</color> pierce for <style=cIsDamage>{100f * SeamstressStaticValues.sewNeedleDamageCoefficient}% damage</style> each. " +
            $"<style=cIsHealing>Heal for {100f * SeamstressStaticValues.needleHealAmount}% of the damage dealt</style>. While " + Tokens.butcheredPrefix + ", <color=#9B3737>Needles</color> <style=cIsUtility>slow</style> enemies. ");
        
        public static string passiveKeywords = needleKeyword + "\n\n" + stitchKeyword + "\n\n" + cutKeyword;

        public static string sewHelp = KeywordText("Sew", "Always fire 1 " + needlePrefix + ".");

        public static string healthCostKeyword = KeywordText("X% HP", "The skill costs " + healthCostPrefix + ".");

        public static string butcheredKeyword = KeywordText("Butchered", "<style=cIsUtility>Empower</style> abilities. Gain movement and attack speed. " +
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