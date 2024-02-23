using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SeamstressMod.Survivors.Seamstress
{
    public static class SeamstressBuffs
    {
        public static BuffDef butchered;

        public static BuffDef cutBleed;

        public static BuffDef scissorCount;

        public static BuffDef needles;

        public static BuffDef parryStart;

        public static BuffDef parrySuccess;
        public static void Init(AssetBundle assetBundle)
        {
            butchered = Modules.Content.CreateAndAddBuff("ButcheredBuff", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Bandit2/texBuffSuperBleedingIcon.tif").WaitForCompletion(), 
                Color.red, false, false, false);
            cutBleed = Modules.Content.CreateAndAddBuff("CutBleed", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Bandit2/texBuffSuperBleedingIcon.tif").WaitForCompletion(),
                new Color(155f / 255f, 55f / 255f, 55f / 255f), true, false, false);
            scissorCount = Modules.Content.CreateAndAddBuff("ScissorCount", Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/VoidSurvivor/texBuffVoidSurvivorCorruptionIcon.tif").WaitForCompletion(),
                new Color(155f / 255f, 55f / 255f, 55f / 255f), true, false, false);
            needles = Modules.Content.CreateAndAddBuff("NeedlesBuff", assetBundle.LoadAsset<Sprite>("texNeedleBuffIcon"),
                Color.white, true, false, false);
            parryStart = Modules.Content.CreateAndAddBuff("ParryBuff", LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                new Color(155f / 255f, 55f / 255f, 55f / 255f), false, false, false);
            parrySuccess = Modules.Content.CreateAndAddBuff("ParryEndBuff", LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.red, false, false, false);

        }
    }
}
