using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SeamstressMod.Survivors.Seamstress
{
    public static class SeamstressBuffs
    {
        // armor buff gained during butcher
        public static BuffDef armorBuff;

        public static BuffDef butchered;

        public static BuffDef needles;

        public static BuffDef stitched;
        public static void Init(AssetBundle assetBundle)
        {
            armorBuff = Modules.Content.CreateAndAddBuff("SeamstressArmorBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.white,
                false,
                false); 
            butchered = Modules.Content.CreateAndAddBuff("ButcheredBuff", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Bandit2/texBuffSuperBleedingIcon.tif").WaitForCompletion(), 
                Color.red, false, false);
            needles = Modules.Content.CreateAndAddBuff("NeedlesBuff", assetBundle.LoadAsset<Sprite>("texNeedleBuffIcon"),
                Color.white, true, false);
            stitched = Modules.Content.CreateAndAddBuff("Stitched", Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/PermanentDebuffOnHit/texBuffPermanentDebuffIcon.tif").WaitForCompletion(),
                new Color(155f / 255f, 55f / 255f, 55f / 255f), true, true);
        }
    }
}
