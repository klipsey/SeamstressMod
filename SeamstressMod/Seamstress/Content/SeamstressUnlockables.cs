using RoR2;
using UnityEngine;
using SeamstressMod.Seamstress;
using SeamstressMod.Seamstress.Achievements;

namespace SeamstressMod.Seamstress.Content
{
    public static class SeamstressUnlockables
    {
        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef masterySkinUnlockableDef = null;

        public static void Init()
        {
            characterUnlockableDef = Modules.Content.CreateAndAddUnlockableDef(
                SeamstressUnlockAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(SeamstressUnlockAchievement.identifier),
                SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texGlimpseOfPurityIcon"));

            masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockableDef(
                SeamstressMasteryAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(SeamstressMasteryAchievement.identifier),
                SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texMasteryAchievement"));
        }
    }
}
