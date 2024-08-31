using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SeamstressMod.Seamstress.Content
{
    public static class SeamstressBuffs
    {
        public static BuffDef SeamstressInsatiableBuff;

        public static BuffDef SeamstressBleedBuff;

        public static BuffDef ScissorRightBuff;

        public static BuffDef ScissorLeftBuff;

        public static BuffDef Needles;

        public static BuffDef ParryStart;

        public static BuffDef ParrySuccess;

        public static BuffDef Manipulated;

        public static BuffDef ManipulatedCd;
        public static void Init(AssetBundle assetBundle)
        {
            SeamstressInsatiableBuff = Modules.Content.CreateAndAddBuff("SeamstressInsatiableBuff", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Bandit2/texBuffSuperBleedingIcon.tif").WaitForCompletion(),
                Color.red, false, false, false);
            SeamstressBleedBuff = Modules.Content.CreateAndAddBuff("SeamstressCutBleed", assetBundle.LoadAsset<Sprite>("texBuffSuperBleedIcon"),
                new Color(155f / 255f, 55f / 255f, 55f / 255f), true, false, false);
            ScissorRightBuff = Modules.Content.CreateAndAddBuff("SeamstressScissorRightBuff", Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/VoidSurvivor/texBuffVoidSurvivorCorruptionIcon.tif").WaitForCompletion(),
                new Color(155f / 255f, 55f / 255f, 55f / 255f), true, false, false);
            ScissorLeftBuff = Modules.Content.CreateAndAddBuff("SeamstressScissorLeftBuff", Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/VoidSurvivor/texBuffVoidSurvivorCorruptionIcon.tif").WaitForCompletion(),
                new Color(155f / 255f, 55f / 255f, 55f / 255f), true, false, false);
            Needles = Modules.Content.CreateAndAddBuff("SeamstressNeedlesBuff", assetBundle.LoadAsset<Sprite>("texNeedleBuffIcon"),
                Color.white, true, false, false);
            ParryStart = Modules.Content.CreateAndAddBuff("SeamstressParryBuff", LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                new Color(155f / 255f, 55f / 255f, 55f / 255f), false, false, false);
            ParrySuccess = Modules.Content.CreateAndAddBuff("SeamstressParryEndBuff", LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.red, false, false, false);
            Manipulated = Modules.Content.CreateAndAddBuff("SeamstressManipulated", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/EliteLunar/texBuffAffixLunar.tif").WaitForCompletion(),
                new Color(155f / 255f, 55f / 255f, 55f / 255f), false, false, false);
            ManipulatedCd = Modules.Content.CreateAndAddBuff("SeamstressManipulatedCd", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/EliteLunar/texBuffAffixLunar.tif").WaitForCompletion(),
                Color.gray, false, false, true);
        }
    }
}
