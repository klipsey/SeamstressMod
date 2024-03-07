using RoR2;
using UnityEngine;
using SeamstressMod.Modules;
using RoR2.Projectile;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using R2API;
using RoR2.UI;

namespace SeamstressMod.Survivors.Seamstress
{
    public static class SeamstressCrosshair
    {
        internal static GameObject seamstressCrosshair;

        private static AssetBundle _assetBundle;
        public static void Init(AssetBundle assetBundle)
        {
            _assetBundle = assetBundle;
            CreateCrosshair();
        }

        private static void CreateCrosshair()
        {
            seamstressCrosshair = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageCrosshair.prefab").WaitForCompletion().InstantiateClone("SeamCrosshair");
            seamstressCrosshair.transform.GetChild(5).GetChild(0).gameObject.GetComponent<RectTransform>().position = new Vector2(-24f, -20f);
            seamstressCrosshair.transform.GetChild(5).GetChild(0).gameObject.GetComponent<RectTransform>().localScale *= 0.5f;
            seamstressCrosshair.transform.GetChild(5).GetChild(1).gameObject.GetComponent<RectTransform>().position = new Vector2(-12f, -20f);
            seamstressCrosshair.transform.GetChild(5).GetChild(1).gameObject.GetComponent<RectTransform>().localScale *= 0.5f;
            seamstressCrosshair.transform.GetChild(5).GetChild(2).gameObject.GetComponent<RectTransform>().position = new Vector2(0f, -20f);
            seamstressCrosshair.transform.GetChild(5).GetChild(2).gameObject.GetComponent<RectTransform>().localScale *= 0.5f;
            seamstressCrosshair.transform.GetChild(5).GetChild(3).gameObject.GetComponent<RectTransform>().position = new Vector2(12f, -20f);
            seamstressCrosshair.transform.GetChild(5).GetChild(3).gameObject.GetComponent<RectTransform>().localScale *= 0.5f;
            GameObject stock5 = seamstressCrosshair.transform.GetChild(5).GetChild(3).gameObject.InstantiateClone("Stock, 5");
            stock5.transform.SetParent(seamstressCrosshair.transform.GetChild(5));
            seamstressCrosshair.transform.GetChild(5).GetChild(4).gameObject.GetComponent<RectTransform>().position = new Vector2(24f, -20f);
            NeedleCrosshairController cross = seamstressCrosshair.gameObject.AddComponent<NeedleCrosshairController>();
            NeedleCrosshairController.NeedleSpriteDisplay one = new NeedleCrosshairController.NeedleSpriteDisplay();
            cross.needleSpriteDisplays = new NeedleCrosshairController.NeedleSpriteDisplay[5];
            one.target = seamstressCrosshair.transform.GetChild(5).GetChild(0).gameObject;
            one.minimumStockCountToBeValid = 1;
            one.maximumStockCountToBeValid = 5;
            cross.needleSpriteDisplays[0] = one;
            one.target = seamstressCrosshair.transform.GetChild(5).GetChild(1).gameObject;
            one.minimumStockCountToBeValid = 2;
            one.maximumStockCountToBeValid = 5;
            cross.needleSpriteDisplays[1] = one;
            one.target = seamstressCrosshair.transform.GetChild(5).GetChild(2).gameObject;
            one.minimumStockCountToBeValid = 3;
            one.maximumStockCountToBeValid = 5;
            cross.needleSpriteDisplays[2] = one;
            one.target = seamstressCrosshair.transform.GetChild(5).GetChild(3).gameObject;
            one.minimumStockCountToBeValid = 4;
            one.maximumStockCountToBeValid = 5;
            cross.needleSpriteDisplays[3] = one;
            one.target = seamstressCrosshair.transform.GetChild(5).GetChild(4).gameObject;
            one.minimumStockCountToBeValid = 5;
            one.maximumStockCountToBeValid = 5;
            cross.needleSpriteDisplays[4] = one;
            Object.Destroy(seamstressCrosshair.gameObject.GetComponent<CrosshairController>());
            Object.Destroy(seamstressCrosshair.transform.GetChild(0).gameObject);
            Object.Destroy(seamstressCrosshair.transform.GetChild(1).gameObject);
            Object.Destroy(seamstressCrosshair.transform.GetChild(2).gameObject);
            Object.Destroy(seamstressCrosshair.transform.GetChild(3).gameObject);
            Object.Destroy(seamstressCrosshair.transform.GetChild(4).gameObject);
            Object.Destroy(seamstressCrosshair.transform.GetChild(6).gameObject);
        }
    }
}