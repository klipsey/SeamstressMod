﻿using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SeamstressMod.Seamstress.Content
{
    public static class SeamstressBuffs
    {
        public static BuffDef instatiable;

        public static BuffDef cutBleed;

        public static BuffDef scissorRightBuff;

        public static BuffDef scissorLeftBuff;

        public static BuffDef needles;

        public static BuffDef parryStart;

        public static BuffDef parrySuccess;

        public static BuffDef manipulated;

        public static BuffDef manipulatedCd;
        public static void Init(AssetBundle assetBundle)
        {
            instatiable = Modules.Content.CreateAndAddBuff("ButcheredBuff", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Bandit2/texBuffSuperBleedingIcon.tif").WaitForCompletion(),
                Color.red, false, false, false);
            cutBleed = Modules.Content.CreateAndAddBuff("CutBleed", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Bandit2/texBuffSuperBleedingIcon.tif").WaitForCompletion(),
                new Color(155f / 255f, 55f / 255f, 55f / 255f), true, false, false);
            scissorRightBuff = Modules.Content.CreateAndAddBuff("ScissorRightBuff", Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/VoidSurvivor/texBuffVoidSurvivorCorruptionIcon.tif").WaitForCompletion(),
                new Color(155f / 255f, 55f / 255f, 55f / 255f), true, false, false);
            scissorLeftBuff = Modules.Content.CreateAndAddBuff("ScissorLeftBuff", Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/VoidSurvivor/texBuffVoidSurvivorCorruptionIcon.tif").WaitForCompletion(),
                new Color(155f / 255f, 55f / 255f, 55f / 255f), true, false, false);
            needles = Modules.Content.CreateAndAddBuff("NeedlesBuff", assetBundle.LoadAsset<Sprite>("texNeedleBuffIcon"),
                Color.white, true, false, false);
            parryStart = Modules.Content.CreateAndAddBuff("ParryBuff", LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                new Color(155f / 255f, 55f / 255f, 55f / 255f), false, false, false);
            parrySuccess = Modules.Content.CreateAndAddBuff("ParryEndBuff", LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.red, false, false, false);
            manipulated = Modules.Content.CreateAndAddBuff("Manipulated", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/EliteLunar/texBuffAffixLunar.tif").WaitForCompletion(),
                new Color(155f / 255f, 55f / 255f, 55f / 255f), false, false, false);
            manipulatedCd = Modules.Content.CreateAndAddBuff("ManipulatedCd", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/EliteLunar/texBuffAffixLunar.tif").WaitForCompletion(),
                Color.gray, false, false, true);
        }
    }
}