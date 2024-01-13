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
        public static void Init(AssetBundle assetBundle)
        {
            armorBuff = Modules.Content.CreateAndAddBuff("SeamstressArmorBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.white,
                false,
                false);
            butchered = Modules.Content.CreateAndAddBuff("ButcheredBuff", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Bandit2/texBuffSuperBleedingIcon.tif").WaitForCompletion(), 
                Color.red, false, false);
            needles = Modules.Content.CreateAndAddBuff("Needles", Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/MoveSpeedOnKill/texBuffKillMoveSpeed.tif").WaitForCompletion(),
                Color.white, true, false);
        }
    }
}
