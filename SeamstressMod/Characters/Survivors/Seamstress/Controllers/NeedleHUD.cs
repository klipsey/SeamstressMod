using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2.UI;
using UnityEngine.UI;
using RoR2;
using SeamstressMod.Survivors.Seamstress;
using UnityEngine.AddressableAssets;

namespace SeamstressMod.Survivors.Seamstress
{
    public static class NeedleHud
    {
        internal static HUD hud;
        internal static GameObject needleZero;
        internal static GameObject needleOne;
        internal static GameObject needleTwo;
        internal static GameObject needleThree;
        internal static GameObject needleFour;
        internal static GameObject needleFive;
        internal static GameObject needleSix;
        internal static GameObject needleSeven;
        internal static GameObject needleEight;
        internal static GameObject needleNine;
        internal static Image needleImgZero;
        internal static Image needleImgOne;
        internal static Image needleImgTwo;
        internal static Image needleImgThree;
        internal static Image needleImgFour;
        internal static Image needleImgFive;
        internal static Image needleImgSix;
        internal static Image needleImgSeven;
        internal static Image needleImgEight;
        internal static Image needleImgNine;
        internal static GameObject expungeHealing;

        public static void Init()
        {

        }
        public static void Awake()
        {
            On.RoR2.UI.HUD.Awake += MyFunc;
        }
        private static void MyFunc(On.RoR2.UI.HUD.orig_Awake orig, RoR2.UI.HUD self)
        {
            //WARNING ALL THE LOCAL POSITION SCALE ROTATION IS FUCKED FOR THE NEEDLES. TO FIX SET SCALE TO 1 AND THEN MANUALLY MOVE TO MIDDLE
            orig(self); // Don't forget to call this, or the vanilla / other mods' codes will not execute!
            hud = self;
            needleZero = new GameObject("Needle0");
            needleZero.transform.SetParent(hud.mainContainer.transform);
            RectTransform rectTransform = needleZero.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = new Vector2(0.5f, 0.5f);
            rectTransform.localPosition = new Vector3(12f, -60f, rectTransform.localPosition.z);
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
            rectTransform.localPosition = new Vector3(-12f, -60f, rectTransform.localPosition.z);
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
            rectTransform.localPosition = new Vector3(24f, -60f, rectTransform.localPosition.z);
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
            rectTransform.localPosition = new Vector3(-24f, -60f, rectTransform.localPosition.z);
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
            rectTransform.localPosition = new Vector3(36f, -60f, rectTransform.localPosition.z);
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
            rectTransform.localPosition = new Vector3(-36f, -60f, rectTransform.localPosition.z);
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
            rectTransform.localPosition = new Vector3(48f, -60f, rectTransform.localPosition.z);
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
            rectTransform.localPosition = new Vector3(-48f, -60f, rectTransform.localPosition.z);
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
            rectTransform.localPosition = new Vector3(60f, -60f, rectTransform.localPosition.z);
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
            rectTransform.localPosition = new Vector3(-60f, -60f, rectTransform.localPosition.z);
            rectTransform.localScale = new Vector3(0.025f, 0.025f, rectTransform.localScale.z);
            rectTransform.rotation = Quaternion.Euler(0f,0f,270f);

            needleNine.AddComponent<Image>();
            needleNine.GetComponent<Image>().sprite = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("needleHudIcon");

            expungeHealing = new GameObject("ExpungeHealing");
            expungeHealing.transform.SetParent(hud.mainContainer.transform);
            rectTransform = expungeHealing.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero; 
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localPosition = new Vector3(0, 30, rectTransform.localPosition.z);
            rectTransform.localScale = new Vector3(1f, 1f, rectTransform.localScale.z);
            rectTransform.rotation = Quaternion.Euler(0f, 0f, 0f);

            expungeHealing.AddComponent<Text>();
            expungeHealing.AddComponent<Outline>();
            expungeHealing.GetComponent<Outline>().effectColor = Color.black;
            expungeHealing.GetComponent<Outline>().effectDistance = Vector2.one;
            expungeHealing.GetComponent<Text>().font = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Font>("RoR2/Base/Common/Fonts/Bombardier/BOMBARD_.ttf").WaitForCompletion());
            expungeHealing.GetComponent<Text>().fontSize = 20;
            expungeHealing.GetComponent<Text>().fontStyle = FontStyle.Normal;
            expungeHealing.GetComponent<Text>().material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/Fonts/Bombardier/BOMBARD_.ttf").WaitForCompletion());           
            expungeHealing.GetComponent<Text>().color = new Color(155f / 255f, 55f / 255f, 55f / 255f, 1f);
            expungeHealing.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            expungeHealing.GetComponent<Text>().text = 0.ToString();
            expungeHealing.GetComponent<Text>().enabled = false;
            expungeHealing.GetComponent<Outline>().enabled = true;

            needleImgZero = needleZero.gameObject.GetComponent<Image>();
            needleImgOne = needleOne.gameObject.GetComponent<Image>();
            needleImgTwo = needleTwo.gameObject.GetComponent<Image>();
            needleImgThree = needleThree.gameObject.GetComponent<Image>();
            needleImgFour = needleFour.gameObject.GetComponent<Image>();
            needleImgFive = needleFive.gameObject.GetComponent<Image>();
            needleImgSix = needleSix.gameObject.GetComponent<Image>();
            needleImgSeven = needleSeven.gameObject.GetComponent<Image>();
            needleImgEight = needleEight.gameObject.GetComponent<Image>();
            needleImgNine = needleNine.gameObject.GetComponent<Image>();

            needleZero.SetActive(false);
            needleOne.SetActive(false);
            needleTwo.SetActive(false);
            needleThree.SetActive(false);
            needleFour.SetActive(false);
            needleFive.SetActive(false);
            needleSix.SetActive(false);
            needleSeven.SetActive(false);
            needleEight.SetActive(false);
            needleNine.SetActive(false);
        }
        public static void OnDestroy()
        {
            On.RoR2.UI.HUD.Awake -= MyFunc;
        }
    }
}
