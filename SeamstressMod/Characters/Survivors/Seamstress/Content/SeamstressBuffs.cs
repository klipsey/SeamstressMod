using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SeamstressMod.Survivors.Seamstress
{
    public static class SeamstressBuffs
    {
        public static BuffDef butchered;

        public static BuffDef needles;

        public static BuffDef needleCountDownBuff;

        public static BuffDef stitched;

        public static BuffDef stitchSetup;
        public static void Init(AssetBundle assetBundle)
        {
            butchered = Modules.Content.CreateAndAddBuff("ButcheredBuff", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Bandit2/texBuffSuperBleedingIcon.tif").WaitForCompletion(), 
                Color.red, false, false, false);
            needles = Modules.Content.CreateAndAddBuff("NeedlesBuff", assetBundle.LoadAsset<Sprite>("texNeedleBuffIcon"),
                Color.white, true, false, false);
            needleCountDownBuff = Modules.Content.CreateAndAddBuff("NeedlesCountdown", assetBundle.LoadAsset<Sprite>("texNeedleBuffIcon"),
                Color.gray, false, false, true);
            stitched = Modules.Content.CreateAndAddBuff("Stitched", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Bandit2/texBuffSuperBleedingIcon.tif").WaitForCompletion(),
                new Color(155f / 255f, 55f / 255f, 55f / 255f), true, false, false);
            stitchSetup = Modules.Content.CreateAndAddBuff("Stitched", Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/VoidSurvivor/texBuffVoidSurvivorCorruptionIcon.tif").WaitForCompletion(),
                new Color(155f / 255f, 55f / 255f, 55f / 255f), true, true, false);
        }
    }
}
