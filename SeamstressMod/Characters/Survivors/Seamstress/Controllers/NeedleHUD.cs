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
    public class NeedleHUD
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
            rectTransform.anchoredPosition = new Vector2(0.5f, 0.5f);
            rectTransform.localPosition = new Vector3(12f, -70f, rectTransform.localPosition.z);
            rectTransform.localScale = new Vector3(0.025f, 0.025f, rectTransform.localScale.z);
            rectTransform.rotation = Quaternion.Euler(0f,0f,270f);
            needleZero.AddComponent<Image>();
            needleZero.GetComponent<Image>().sprite = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("needleHudIcon");

            needleOne = new GameObject("Needle1");
            needleOne.transform.SetParent(hud.mainContainer.transform);
            rectTransform = needleOne.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localPosition = new Vector3(-12f, -70f, rectTransform.localPosition.z);
            rectTransform.localScale = new Vector3(0.025f, 0.025f, rectTransform.localScale.z);
            rectTransform.rotation = Quaternion.Euler(0f,0f,270f);

            needleOne.AddComponent<Image>();
            needleOne.GetComponent<Image>().sprite = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("needleHudIcon");

            needleTwo = new GameObject("Needle2");
            needleTwo.transform.SetParent(hud.mainContainer.transform);
            rectTransform = needleTwo.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localPosition = new Vector3(24f, -70f, rectTransform.localPosition.z);
            rectTransform.localScale = new Vector3(0.025f, 0.025f, rectTransform.localScale.z);
            rectTransform.rotation = Quaternion.Euler(0f,0f,270f);

            needleTwo.AddComponent<Image>();
            needleTwo.GetComponent<Image>().sprite = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("needleHudIcon");

            needleThree = new GameObject("Needle3");
            needleThree.transform.SetParent(hud.mainContainer.transform);
            rectTransform = needleThree.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localPosition = new Vector3(-24f, -70f, rectTransform.localPosition.z);
            rectTransform.localScale = new Vector3(0.025f, 0.025f, rectTransform.localScale.z);
            rectTransform.rotation = Quaternion.Euler(0f,0f,270f);

            needleThree.AddComponent<Image>();
            needleThree.GetComponent<Image>().sprite = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("needleHudIcon");

            needleFour = new GameObject("Needle4");
            needleFour.transform.SetParent(hud.mainContainer.transform);
            rectTransform = needleFour.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localPosition = new Vector3(36f, -70f, rectTransform.localPosition.z);
            rectTransform.localScale = new Vector3(0.025f, 0.025f, rectTransform.localScale.z);
            rectTransform.rotation = Quaternion.Euler(0f,0f,270f);

            needleFour.AddComponent<Image>();
            needleFour.GetComponent<Image>().sprite = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("needleHudIcon");

            needleFive = new GameObject("Needle5");
            needleFive.transform.SetParent(hud.mainContainer.transform);
            rectTransform = needleFive.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localPosition = new Vector3(-36f, -70f, rectTransform.localPosition.z);
            rectTransform.localScale = new Vector3(0.025f, 0.025f, rectTransform.localScale.z);
            rectTransform.rotation = Quaternion.Euler(0f,0f,270f);

            needleFive.AddComponent<Image>();
            needleFive.GetComponent<Image>().sprite = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("needleHudIcon");

            needleSix = new GameObject("Needle6");
            needleSix.transform.SetParent(hud.mainContainer.transform);
            rectTransform = needleSix.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localPosition = new Vector3(48f, -70f, rectTransform.localPosition.z);
            rectTransform.localScale = new Vector3(0.025f, 0.025f, rectTransform.localScale.z);
            rectTransform.rotation = Quaternion.Euler(0f,0f,270f);

            needleSix.AddComponent<Image>();
            needleSix.GetComponent<Image>().sprite = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("needleHudIcon");

            needleSeven = new GameObject("Needle7");
            needleSeven.transform.SetParent(hud.mainContainer.transform);
            rectTransform = needleSeven.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localPosition = new Vector3(-48f, -70f, rectTransform.localPosition.z);
            rectTransform.localScale = new Vector3(0.025f, 0.025f, rectTransform.localScale.z);
            rectTransform.rotation = Quaternion.Euler(0f,0f,270f);

            needleSeven.AddComponent<Image>();
            needleSeven.GetComponent<Image>().sprite = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("needleHudIcon");

            needleEight = new GameObject("Needle8");
            needleEight.transform.SetParent(hud.mainContainer.transform);
            rectTransform = needleEight.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localPosition = new Vector3(60f, -70f, rectTransform.localPosition.z);
            rectTransform.localScale = new Vector3(0.025f, 0.025f, rectTransform.localScale.z);
            rectTransform.rotation = Quaternion.Euler(0f,0f,270f);

            needleEight.AddComponent<Image>();
            needleEight.GetComponent<Image>().sprite = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("needleHudIcon");

            needleNine = new GameObject("Needle9");
            needleNine.transform.SetParent(hud.mainContainer.transform);
            rectTransform = needleNine.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localPosition = new Vector3(-60f, -70f, rectTransform.localPosition.z);
            rectTransform.localScale = new Vector3(0.025f, 0.025f, rectTransform.localScale.z);
            rectTransform.rotation = Quaternion.Euler(0f,0f,270f);

            needleNine.AddComponent<Image>();
            needleNine.GetComponent<Image>().sprite = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("needleHudIcon");
            // Utilize the ResourcesAPI from R2API to load your image!
        }
        public static void DestroyHUD()
        {
            UnityEngine.Object.Destroy(needleZero);
            UnityEngine.Object.Destroy(needleOne);
            UnityEngine.Object.Destroy(needleTwo);
            UnityEngine.Object.Destroy(needleThree);
            UnityEngine.Object.Destroy(needleFour);
            UnityEngine.Object.Destroy(needleFive);
            UnityEngine.Object.Destroy(needleSix);
            UnityEngine.Object.Destroy(needleSeven);
            UnityEngine.Object.Destroy(needleEight);
            UnityEngine.Object.Destroy(needleNine);
            On.RoR2.UI.HUD.Awake -= MyFunc;
        }
    }
}
