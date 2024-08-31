using RoR2;
using SeamstressMod.Modules.Achievements;
using SeamstressMod.Seamstress;

namespace SeamstressMod.Seamstress.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, 0)]
    public class SeamstressMasteryAchievement : BaseMasteryAchievement
    {
        public const string identifier = SeamstressSurvivor.SEAMSTRESS_PREFIX + "masteryAchievement";
        public const string unlockableIdentifier = SeamstressSurvivor.SEAMSTRESS_PREFIX + "masteryUnlockable";

        public override string RequiredCharacterBody => SeamstressSurvivor.instance.bodyName;

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}