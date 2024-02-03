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
    public class NeedleHUD : MonoBehaviour
    {
        private HUD hud;
        private GameObject needleZero;
        private GameObject needleOne;
        private GameObject needleTwo;
        private GameObject needleThree;
        private GameObject needleFour;
        private GameObject needleFive;
        private GameObject needleSix;
        private GameObject needleSeven;
        private GameObject needleEight;
        private GameObject needleNine;
        private GameObject expungeHealing;

        public void Awake()
        {
            On.RoR2.UI.HUD.Awake += MyFunc;
        }
        private void MyFunc(On.RoR2.UI.HUD.orig_Awake orig, RoR2.UI.HUD self)
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
            rectTransform.rotation = Quaternion.Euler(0f, 0f, 270f);
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
            rectTransform.rotation = Quaternion.Euler(0f, 0f, 270f);

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
            rectTransform.rotation = Quaternion.Euler(0f, 0f, 270f);

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
            rectTransform.rotation = Quaternion.Euler(0f, 0f, 270f);

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
            rectTransform.rotation = Quaternion.Euler(0f, 0f, 270f);

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
            rectTransform.rotation = Quaternion.Euler(0f, 0f, 270f);

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
            rectTransform.rotation = Quaternion.Euler(0f, 0f, 270f);

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
            rectTransform.rotation = Quaternion.Euler(0f, 0f, 270f);

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
            rectTransform.rotation = Quaternion.Euler(0f, 0f, 270f);

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
            rectTransform.rotation = Quaternion.Euler(0f, 0f, 270f);

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
        }
        public void ActivateExpungeHud(bool activate)
        {
            if(activate) 
            {
                expungeHealing.GetComponent<Text>().enabled = true;
                expungeHealing.GetComponent<Outline>().enabled = true;
            }
            else
            {
                expungeHealing.GetComponent<Text>().enabled = false;
                expungeHealing.GetComponent<Outline>().enabled = false;
            }
        }
        public void UpdateExpunge(float num)
        {
            expungeHealing.GetComponent<Text>().text = Mathf.Round(num).ToString();
        }
        public void HudColor(float time, float percent)
        {
            #region hudColorTracking
            if (time > 9f * percent)
            {
                Color newColor = Color.white;
                needleZero.GetComponent<Image>().color = newColor;
                needleOne.GetComponent<Image>().color = newColor;
                needleTwo.GetComponent<Image>().color = newColor;
                needleThree.GetComponent<Image>().color = newColor;
                needleFour.GetComponent<Image>().color = newColor;
                needleFive.GetComponent<Image>().color = newColor;
                needleSix.GetComponent<Image>().color = newColor;
                needleSeven.GetComponent<Image>().color = newColor;
                needleEight.GetComponent<Image>().color = newColor;
                needleNine.GetComponent<Image>().color = newColor;
            }
            if (time <= 9f * percent && time > 8f * percent)
            {
                Color newColor = new Color(245f / 255f, 232 / 255f, 231 / 255f);
                newColor.a = 1;
                needleZero.GetComponent<Image>().color = newColor;
                needleOne.GetComponent<Image>().color = newColor;
                needleTwo.GetComponent<Image>().color = newColor;
                needleThree.GetComponent<Image>().color = newColor;
                needleFour.GetComponent<Image>().color = newColor;
                needleFive.GetComponent<Image>().color = newColor;
                needleSix.GetComponent<Image>().color = newColor;
                needleSeven.GetComponent<Image>().color = newColor;
                needleEight.GetComponent<Image>().color = newColor;
                needleNine.GetComponent<Image>().color = newColor;
            }
            if (time <= 8f * percent && time > 7f * percent)
            {
                Color newColor = new Color(237f / 255f, 209f / 255f, 207f / 255f);
                newColor.a = 1;
                needleZero.GetComponent<Image>().color = newColor;
                needleOne.GetComponent<Image>().color = newColor;
                needleTwo.GetComponent<Image>().color = newColor;
                needleThree.GetComponent<Image>().color = newColor;
                needleFour.GetComponent<Image>().color = newColor;
                needleFive.GetComponent<Image>().color = newColor;
                needleSix.GetComponent<Image>().color = newColor;
                needleSeven.GetComponent<Image>().color = newColor;
                needleEight.GetComponent<Image>().color = newColor;
                needleNine.GetComponent<Image>().color = newColor;
            }
            if (time <= 7f * percent && time > 6f * percent)
            {
                Color newColor = new Color(227f / 255f, 187f / 255f, 183f / 255f);
                newColor.a = 1;
                needleZero.GetComponent<Image>().color = newColor;
                needleOne.GetComponent<Image>().color = newColor;
                needleTwo.GetComponent<Image>().color = newColor;
                needleThree.GetComponent<Image>().color = newColor;
                needleFour.GetComponent<Image>().color = newColor;
                needleFive.GetComponent<Image>().color = newColor;
                needleSix.GetComponent<Image>().color = newColor;
                needleSeven.GetComponent<Image>().color = newColor;
                needleEight.GetComponent<Image>().color = newColor;
                needleNine.GetComponent<Image>().color = newColor;
            }
            if (time <= 6f * percent && time > 5f * percent)
            {
                Color newColor = new Color(217f / 255f, 165f / 255f, 160f / 255f);
                newColor.a = 1;
                needleZero.GetComponent<Image>().color = newColor;
                needleOne.GetComponent<Image>().color = newColor;
                needleTwo.GetComponent<Image>().color = newColor;
                needleThree.GetComponent<Image>().color = newColor;
                needleFour.GetComponent<Image>().color = newColor;
                needleFive.GetComponent<Image>().color = newColor;
                needleSix.GetComponent<Image>().color = newColor;
                needleSeven.GetComponent<Image>().color = newColor;
                needleEight.GetComponent<Image>().color = newColor;
                needleNine.GetComponent<Image>().color = newColor;
            }
            if (time <= 5f * percent && time > 4f * percent)
            {
                Color newColor = new Color(205f / 255f, 143f / 255f, 138f / 255f);
                newColor.a = 1;
                needleZero.GetComponent<Image>().color = newColor;
                needleOne.GetComponent<Image>().color = newColor;
                needleTwo.GetComponent<Image>().color = newColor;
                needleThree.GetComponent<Image>().color = newColor;
                needleFour.GetComponent<Image>().color = newColor;
                needleFive.GetComponent<Image>().color = newColor;
                needleSix.GetComponent<Image>().color = newColor;
                needleSeven.GetComponent<Image>().color = newColor;
                needleEight.GetComponent<Image>().color = newColor;
                needleNine.GetComponent<Image>().color = newColor;
            }
            if (time <= 4f * percent && time > 3f * percent)
            {
                Color newColor = new Color(193f / 255f, 121f / 255f, 116f / 255f);
                newColor.a = 1;
                needleZero.GetComponent<Image>().color = newColor;
                needleOne.GetComponent<Image>().color = newColor;
                needleTwo.GetComponent<Image>().color = newColor;
                needleThree.GetComponent<Image>().color = newColor;
                needleFour.GetComponent<Image>().color = newColor;
                needleFive.GetComponent<Image>().color = newColor;
                needleSix.GetComponent<Image>().color = newColor;
                needleSeven.GetComponent<Image>().color = newColor;
                needleEight.GetComponent<Image>().color = newColor;
                needleNine.GetComponent<Image>().color = newColor;
            }
            if (time <= 3f * percent && time > 2f * percent)
            {
                Color newColor = new Color(181f / 255f, 100f / 255f, 95f / 255f);
                newColor.a = 1;
                needleZero.GetComponent<Image>().color = newColor;
                needleOne.GetComponent<Image>().color = newColor;
                needleTwo.GetComponent<Image>().color = newColor;
                needleThree.GetComponent<Image>().color = newColor;
                needleFour.GetComponent<Image>().color = newColor;
                needleFive.GetComponent<Image>().color = newColor;
                needleSix.GetComponent<Image>().color = newColor;
                needleSeven.GetComponent<Image>().color = newColor;
                needleEight.GetComponent<Image>().color = newColor;
                needleNine.GetComponent<Image>().color = newColor;
            }
            if (time <= 2f * percent && time > 1f * percent)
            {
                Color newColor = new Color(168f / 255f, 78f / 255f, 74f / 255f);
                newColor.a = 1;
                needleZero.GetComponent<Image>().color = newColor;
                needleOne.GetComponent<Image>().color = newColor;
                needleTwo.GetComponent<Image>().color = newColor;
                needleThree.GetComponent<Image>().color = newColor;
                needleFour.GetComponent<Image>().color = newColor;
                needleFive.GetComponent<Image>().color = newColor;
                needleSix.GetComponent<Image>().color = newColor;
                needleSeven.GetComponent<Image>().color = newColor;
                needleEight.GetComponent<Image>().color = newColor;
                needleNine.GetComponent<Image>().color = newColor;
            }
            if (time <= 1f * percent && time > 0f * percent)
            {
                Color newColor = new Color(154f / 255f, 55f / 255f, 55f / 255f);
                newColor.a = 1;
                needleZero.GetComponent<Image>().color = newColor;
                needleOne.GetComponent<Image>().color = newColor;
                needleTwo.GetComponent<Image>().color = newColor;
                needleThree.GetComponent<Image>().color = newColor;
                needleFour.GetComponent<Image>().color = newColor;
                needleFive.GetComponent<Image>().color = newColor;
                needleSix.GetComponent<Image>().color = newColor;
                needleSeven.GetComponent<Image>().color = newColor;
                needleEight.GetComponent<Image>().color = newColor;
                needleNine.GetComponent<Image>().color = newColor;
            }
            if (time <= 0f)
            {
                Color newColor = Color.white;
                needleZero.GetComponent<Image>().color = newColor;
                needleOne.GetComponent<Image>().color = newColor;
                needleTwo.GetComponent<Image>().color = newColor;
                needleThree.GetComponent<Image>().color = newColor;
                needleFour.GetComponent<Image>().color = newColor;
                needleFive.GetComponent<Image>().color = newColor;
                needleSix.GetComponent<Image>().color = newColor;
                needleSeven.GetComponent<Image>().color = newColor;
                needleEight.GetComponent<Image>().color = newColor;
                needleNine.GetComponent<Image>().color = newColor;
            }
            #endregion
        }
        public void NeedleDisplayCount(int num)
        {
            #region needlehud
            switch (num)
            {
                case 0:
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
                    break;
                case 1:
                    needleZero.SetActive(true);
                    needleOne.SetActive(false);
                    needleTwo.SetActive(false);
                    needleThree.SetActive(false);
                    needleFour.SetActive(false);
                    needleFive.SetActive(false);
                    needleSix.SetActive(false);
                    needleSeven.SetActive(false);
                    needleEight.SetActive(false);
                    needleNine.SetActive(false);
                    break;
                case 2:
                    needleZero.SetActive(true);
                    needleOne.SetActive(true);
                    needleTwo.SetActive(false);
                    needleThree.SetActive(false);
                    needleFour.SetActive(false);
                    needleFive.SetActive(false);
                    needleSix.SetActive(false);
                    needleSeven.SetActive(false);
                    needleEight.SetActive(false);
                    needleNine.SetActive(false);
                    break;
                case 3:
                    needleZero.SetActive(true);
                    needleOne.SetActive(true);
                    needleTwo.SetActive(true);
                    needleThree.SetActive(false);
                    needleFour.SetActive(false);
                    needleFive.SetActive(false);
                    needleSix.SetActive(false);
                    needleSeven.SetActive(false);
                    needleEight.SetActive(false);
                    needleNine.SetActive(false);
                    break;
                case 4:
                    needleZero.SetActive(true);
                    needleOne.SetActive(true);
                    needleTwo.SetActive(true);
                    needleThree.SetActive(true);
                    needleFour.SetActive(false);
                    needleFive.SetActive(false);
                    needleSix.SetActive(false);
                    needleSeven.SetActive(false);
                    needleEight.SetActive(false);
                    needleNine.SetActive(false);
                    break;
                case 5:
                    needleZero.SetActive(true);
                    needleOne.SetActive(true);
                    needleTwo.SetActive(true);
                    needleThree.SetActive(true);
                    needleFour.SetActive(true);
                    needleFive.SetActive(false);
                    needleSix.SetActive(false);
                    needleSeven.SetActive(false);
                    needleEight.SetActive(false);
                    needleNine.SetActive(false);
                    break;
                case 6:
                    needleZero.SetActive(true);
                    needleOne.SetActive(true);
                    needleTwo.SetActive(true);
                    needleThree.SetActive(true);
                    needleFour.SetActive(true);
                    needleFive.SetActive(true);
                    needleSix.SetActive(false);
                    needleSeven.SetActive(false);
                    needleEight.SetActive(false);
                    needleNine.SetActive(false);
                    break;
                case 7:
                    needleZero.SetActive(true);
                    needleOne.SetActive(true);
                    needleTwo.SetActive(true);
                    needleThree.SetActive(true);
                    needleFour.SetActive(true);
                    needleFive.SetActive(true);
                    needleSix.SetActive(true);
                    needleSeven.SetActive(false);
                    needleEight.SetActive(false);
                    needleNine.SetActive(false);
                    break;
                case 8:
                    needleZero.SetActive(true);
                    needleOne.SetActive(true);
                    needleTwo.SetActive(true);
                    needleThree.SetActive(true);
                    needleFour.SetActive(true);
                    needleFive.SetActive(true);
                    needleSix.SetActive(true);
                    needleSeven.SetActive(true);
                    needleEight.SetActive(false);
                    needleNine.SetActive(false);
                    break;
                case 9:
                    needleZero.SetActive(true);
                    needleOne.SetActive(true);
                    needleTwo.SetActive(true);
                    needleThree.SetActive(true);
                    needleFour.SetActive(true);
                    needleFive.SetActive(true);
                    needleSix.SetActive(true);
                    needleSeven.SetActive(true);
                    needleEight.SetActive(true);
                    needleNine.SetActive(false);
                    break;
                case 10:
                    needleZero.SetActive(true);
                    needleOne.SetActive(true);
                    needleTwo.SetActive(true);
                    needleThree.SetActive(true);
                    needleFour.SetActive(true);
                    needleFive.SetActive(true);
                    needleSix.SetActive(true);
                    needleSeven.SetActive(true);
                    needleEight.SetActive(true);
                    needleNine.SetActive(true);
                    break;

            }
            #endregion
        }
        public void OnDestroy()
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
            UnityEngine.Object.Destroy(expungeHealing);
            On.RoR2.UI.HUD.Awake -= MyFunc;
        }
    }
}