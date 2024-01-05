using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SeamstressMod.Survivors.Seamstress
{
    public static class SeamstressBuffs
    {
        // armor buff gained during butcher
        public static BuffDef armorBuff;

        public static BuffDef bloodBath;

        public static BuffDef weaveDur;
        public static void Init(AssetBundle assetBundle)
        {
            armorBuff = Modules.Content.CreateAndAddBuff("SeamstressArmorBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.white,
                false,
                false);
            bloodBath = Modules.Content.CreateAndAddBuff("BloodBath", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Bandit2/texBuffSuperBleedingIcon.tif").WaitForCompletion(), 
                Color.red, false, false);
            weaveDur = Modules.Content.CreateAndAddBuff("WeaveDuration", LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite, Color.white, false, false);
        }
    }
}
