using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2.UI;
using UnityEngine.UI;
using RoR2;
using SeamstressMod.Survivors.Seamstress;

namespace SeamstressMod.Survivors.Seamstress
{
    public static class NeedleHUD
    {
        private static HUD hud;
        public static GameObject needleZero;
        public static GameObject needleOne;
        public static GameObject needleTwo;
        public static GameObject needleThree;
        public static GameObject needleFour;
        public static GameObject needleFive;
        public static GameObject needleSix;
        public static GameObject needleSeven;
        public static GameObject needleEight;
        public static GameObject needleNine;
        public static void Init()
        {
            On.RoR2.UI.HUD.Awake += MyFunc;
        }
        private static void MyFunc(On.RoR2.UI.HUD.orig_Awake orig, RoR2.UI.HUD self)
        {
            orig(self); // Don't forget to call this, or the vanilla / other mods' codes will not execute!
            hud = self;
            needleZero = new GameObject("Needle0");
            needleZero.transform.SetParent(hud.mainContainer.transform);
            RectTransform rectTransform = needleZero.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localPosition = new Vector3(0.8f, 0.3f, rectTransform.localPosition.z);
            rectTransform.localScale = new Vector3(0.1f, 0.1f, rectTransform.localScale.z);
            needleZero.AddComponent<Image>();
            needleZero.GetComponent<Image>().sprite = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("needleHudIcon");

            needleOne = new GameObject("Needle1");
            needleOne.transform.SetParent(hud.mainContainer.transform);
            rectTransform = needleOne.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localPosition = new Vector3(0.8f, 0.325f, rectTransform.localPosition.z);
            rectTransform.localScale = new Vector3(0.1f, 0.1f, rectTransform.localScale.z);
            needleOne.AddComponent<Image>();
            needleOne.GetComponent<Image>().sprite = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("needleHudIcon");

            needleTwo = new GameObject("Needle2");
            needleTwo.transform.SetParent(hud.mainContainer.transform);
            rectTransform = needleTwo.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localPosition = new Vector3(0.8f, 0.35f, rectTransform.localPosition.z);
            rectTransform.localScale = new Vector3(0.1f, 0.1f, rectTransform.localScale.z);
            needleTwo.AddComponent<Image>();
            needleTwo.GetComponent<Image>().sprite = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("needleHudIcon");

            needleThree = new GameObject("Needle3");
            needleThree.transform.SetParent(hud.mainContainer.transform);
            rectTransform = needleThree.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localPosition = new Vector3(0.8f, 0.375f, rectTransform.localPosition.z);
            rectTransform.localScale = new Vector3(0.1f, 0.1f, rectTransform.localScale.z);
            needleThree.AddComponent<Image>();
            needleThree.GetComponent<Image>().sprite = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("needleHudIcon");

            needleFour = new GameObject("Needle4");
            needleFour.transform.SetParent(hud.mainContainer.transform);
            rectTransform = needleFour.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localPosition = new Vector3(0.8f, 0.4f, rectTransform.localPosition.z);
            rectTransform.localScale = new Vector3(0.1f, 0.1f, rectTransform.localScale.z);
            needleFour.AddComponent<Image>();
            needleFour.GetComponent<Image>().sprite = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("needleHudIcon");

            needleFive = new GameObject("Needle5");
            needleFive.transform.SetParent(hud.mainContainer.transform);
            rectTransform = needleFive.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localPosition = new Vector3(0.8f, 0.425f, rectTransform.localPosition.z);
            rectTransform.localScale = new Vector3(0.1f, 0.1f, rectTransform.localScale.z);
            needleFive.AddComponent<Image>();
            needleFive.GetComponent<Image>().sprite = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("needleHudIcon");

            needleSix = new GameObject("Needle6");
            needleSix.transform.SetParent(hud.mainContainer.transform);
            rectTransform = needleSix.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localPosition = new Vector3(0.8f, 0.45f, rectTransform.localPosition.z);
            rectTransform.localScale = new Vector3(0.1f, 0.1f, rectTransform.localScale.z);
            needleSix.AddComponent<Image>();
            needleSix.GetComponent<Image>().sprite = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("needleHudIcon");

            needleSeven = new GameObject("Needle7");
            needleSeven.transform.SetParent(hud.mainContainer.transform);
            rectTransform = needleSeven.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localPosition = new Vector3(0.8f, 0.475f, rectTransform.localPosition.z);
            rectTransform.localScale = new Vector3(0.1f, 0.1f, rectTransform.localScale.z);
            needleSeven.AddComponent<Image>();
            needleSeven.GetComponent<Image>().sprite = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("needleHudIcon");

            needleEight = new GameObject("Needle8");
            needleEight.transform.SetParent(hud.mainContainer.transform);
            rectTransform = needleEight.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localPosition = new Vector3(0.8f, 0.5f, rectTransform.localPosition.z);
            rectTransform.localScale = new Vector3(0.1f, 0.1f, rectTransform.localScale.z);
            needleEight.AddComponent<Image>();
            needleEight.GetComponent<Image>().sprite = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("needleHudIcon");

            needleNine = new GameObject("Needle9");
            needleNine.transform.SetParent(hud.mainContainer.transform);
            rectTransform = needleNine.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localPosition = new Vector3(0.8f, 0.525f, rectTransform.localPosition.z);
            rectTransform.localScale = new Vector3(0.1f, 0.1f, rectTransform.localScale.z);
            needleNine.AddComponent<Image>();
            needleNine.GetComponent<Image>().sprite = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("needleHudIcon");
            // Utilize the ResourcesAPI from R2API to load your image!
        }
        private static void OnDestroy()
        {
            On.RoR2.UI.HUD.Awake -= MyFunc;
        }
    }
}
