using RoR2;
using System.Collections.Generic;
using SeamstressMod.Survivors.Seamstress;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.AddressableAssets;
using RoR2.EntityLogic;
using Rewired.HID;

namespace SeamstressMod.Survivors.Seamstress
{
    public class SeamstressController : MonoBehaviour
    {
        public CharacterBody characterBody;

        public float butcheredConversion;

        public float needleRegen;

        public int baseNeedleAmount;
        private bool hasEffectiveAuthority => characterBody.hasEffectiveAuthority;

        public HealthComponent healthComponent;

        public SkillLocator skillLocator;

        public NeedleHUD needleHUD;

        public bool hasPlayed;

        public bool fuckYou;

        public bool butchered;

        public float bd;

        public float butcheredDurationPercent;

        public int needleCount;

        public TemporaryOverlay component;
        public void Awake()
        {
            this.healthComponent = GetComponent<HealthComponent>();
            this.characterBody = GetComponent<CharacterBody>();
            this.skillLocator = GetComponent<SkillLocator>();
            this.needleHUD = GetComponent<NeedleHUD>();
            butcheredConversion = 0f;
            Hook();
        }
        public void Start()
        {
            needleRegen = SeamstressStaticValues.needleGainInterval;
            hasPlayed = false;
            fuckYou = false;
            butchered = false;
            bd = 0f;
            butcheredDurationPercent = bd / 10f;
        }
        public void FixedUpdate()
        {
            if (bd > 0f)
            {
                bd -= Time.fixedDeltaTime;
            }
            passiveNeedleRegen();
            IsButchered();
            ButcheredSound();
            CalculateBonusDamage();
            NeedleDisplayCount();
        }
        private static void Hook()
        {
            On.RoR2.HealthComponent.Heal += new On.RoR2.HealthComponent.hook_Heal(HealthComponent_Heal);
            On.RoR2.CharacterModel.UpdateOverlays += new On.RoR2.CharacterModel.hook_UpdateOverlays(CharacterModel_UpdateOverlays);
        }
        //butchered overlay
        private static void CharacterModel_UpdateOverlays(On.RoR2.CharacterModel.orig_UpdateOverlays orig, CharacterModel self)
        {
            orig.Invoke(self);
            if (!self || !self.body)
            {
                return;
            }
            if (self.body.HasBuff(SeamstressBuffs.butchered) && self.body.GetComponent<SeamstressController>().fuckYou == false)
            {
                TemporaryOverlay temporaryOverlay = self.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = SeamstressStaticValues.butcheredDuration;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0.4f);
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = SeamstressAssets.butcheredOverlayMat;
                temporaryOverlay.AddToCharacerModel(self);
                self.body.GetComponent<SeamstressController>().fuckYou = true;
            }
        }
        //calculate expunge healing
        private static float HealthComponent_Heal(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen = true)
        {
            if (self.body.HasBuff(SeamstressBuffs.butchered) && self.body.baseNameToken == "KENKO_SEAMSTRESS_NAME")
            {
                amount *= SeamstressStaticValues.healConversion;
            }
            var res = orig(self, amount, procChainMask, nonRegen);
            SeamstressController s = self.body.GetComponent<SeamstressController>();
            if (self.body.TryGetComponent<SeamstressController>(out s) && self.body.HasBuff(SeamstressBuffs.butchered))
            {
                s.GetButcheredConversion((res / SeamstressStaticValues.healConversion) * 1 - SeamstressStaticValues.healConversion);
            }
            return res;
        }
        //needle regen
        private void passiveNeedleRegen()
        {
            if (this.characterBody.GetBuffCount(SeamstressBuffs.needles) < SeamstressStaticValues.maxNeedleAmount + skillLocator.special.maxStock - 1)
            {
                if (!this.characterBody.HasBuff(SeamstressBuffs.needleCountDownBuff))
                {
                    if(this.hasEffectiveAuthority)
                    {
                        this.characterBody.AddTimedBuff(SeamstressBuffs.needleCountDownBuff, SeamstressStaticValues.needleGainInterval);
                        this.characterBody.AddBuff(SeamstressBuffs.needles);
                        Util.PlaySound("Play_treeBot_m1_hit_heal", this.characterBody.gameObject);
                    }
                }
            }
        }
        private void GetButcheredConversion(float healDamage)
        {
            butcheredConversion += healDamage;
        }
        //butchered controller
        private void IsButchered()
        {
            //run during butchered
            if (this.characterBody.HasBuff(SeamstressBuffs.butchered))
            {
                bd = SeamstressStaticValues.butcheredDuration;
                //run when butchered starts
                if (!butchered && bd > 0f)
                {
                    butcheredDurationPercent = bd / 10f;
                    if(this.hasEffectiveAuthority)
                    {
                        needleHUD.expungeHealing.GetComponent<Text>().enabled = true;
                        needleHUD.expungeHealing.GetComponent<Outline>().enabled = true;
                        butchered = true;
                        #region IconUpdate
                        this.skillLocator.primary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texStingerIcon");
                        this.skillLocator.secondary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texPistolIcon");
                        this.skillLocator.special.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texScepterSpecialIcon");
                        #endregion
                    }
                    Transform modelTransform = this.characterBody.modelLocator.modelTransform;
                    if (modelTransform)
                    {
                        #region overlay
                        TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                        temporaryOverlay.duration = 1f;
                        temporaryOverlay.destroyComponentOnEnd = true;
                        temporaryOverlay.originalMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/matOnFire.mat").WaitForCompletion();
                        temporaryOverlay.inspectorCharacterModel = modelTransform.GetComponent<CharacterModel>();
                        temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                        temporaryOverlay.animateShaderAlpha = true;
                        #endregion
                    }
                }
            }
            if (!this.characterBody.HasBuff(SeamstressBuffs.butchered) && butchered)
            {
                butchered = false;
                fuckYou = false;
                if(this.hasEffectiveAuthority)
                {
                    UnityEngine.Object.Instantiate<GameObject>(SeamstressAssets.reapEndEffect, this.characterBody.modelLocator.transform);
                    Util.PlaySound("Play_voidman_transform_return", this.characterBody.gameObject);
                    #region IconUpdate
                    this.skillLocator.primary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texPrimaryIcon");
                    this.skillLocator.secondary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texSecondaryIcon");
                    this.skillLocator.special.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texSpecialIcon");
                    #endregion
                }
            }
            //fire expunge at end of butchered
            if(this.hasEffectiveAuthority)
            {
                if (!this.characterBody.HasBuff(SeamstressBuffs.butchered) && this.skillLocator.utility == this.skillLocator.FindSkill("reapRecast"))
                {
                    this.skillLocator.utility.ExecuteIfReady();
                }
                if (needleHUD.expungeHealing.GetComponent<Text>())
                {
                    needleHUD.expungeHealing.GetComponent<Text>().text = Mathf.Round(butcheredConversion).ToString();
                }
                if (this.characterBody.HasBuff(SeamstressBuffs.butchered))
                {
                    HudColor();
                }
                else
                {
                    #region fuck you
                    Color newColor = Color.white;
                    newColor.a = 1;
                    needleHUD.needleImgZero.color = newColor;
                    needleHUD.needleImgOne.color = newColor;
                    needleHUD.needleImgTwo.color = newColor;
                    needleHUD.needleImgThree.color = newColor;
                    needleHUD.needleImgFour.color = newColor;
                    needleHUD.needleImgFive.color = newColor;
                    needleHUD.needleImgSix.color = newColor;
                    needleHUD.needleImgSeven.color = newColor;
                    needleHUD.needleImgEight.color = newColor;
                    needleHUD.needleImgNine.color = newColor;
                    #endregion
                }
            }
        }
        //butchered end sound
        private void ButcheredSound()
        {
            if (this.characterBody.HasBuff(SeamstressBuffs.butchered))
            {
                if (bd < 2f && !hasPlayed)
                {
                    Util.PlaySound("Play_nullifier_impact", this.characterBody.gameObject);
                    hasPlayed = true;
                }
            }
            hasPlayed = false;
        }
        //passive damage
        private void CalculateBonusDamage()
        {
            float healthMissing = (this.healthComponent.fullHealth + this.healthComponent.fullShield) - (this.healthComponent.health + this.healthComponent.shield);
            this.characterBody.baseDamage = 10f + (healthMissing * SeamstressStaticValues.passiveScaling);
        }
        //needle controller
        private void NeedleDisplayCount()
        {
            #region needle
            baseNeedleAmount = skillLocator.special.maxStock - 1;
            needleCount = characterBody.GetBuffCount(SeamstressBuffs.needles) - baseNeedleAmount;
            switch (needleCount)
            {
                case 0:
                    needleHUD.needleZero.SetActive(false);
                    needleHUD.needleOne.SetActive(false);
                    needleHUD.needleTwo.SetActive(false);
                    needleHUD.needleThree.SetActive(false);
                    needleHUD.needleFour.SetActive(false);
                    needleHUD.needleFive.SetActive(false);
                    needleHUD.needleSix.SetActive(false);
                    needleHUD.needleSeven.SetActive(false);
                    needleHUD.needleEight.SetActive(false);
                    needleHUD.needleNine.SetActive(false);
                    break;
                case 1:
                    needleHUD.needleZero.SetActive(true);
                    needleHUD.needleOne.SetActive(false);
                    needleHUD.needleTwo.SetActive(false);
                    needleHUD.needleThree.SetActive(false);
                    needleHUD.needleFour.SetActive(false);
                    needleHUD.needleFive.SetActive(false);
                    needleHUD.needleSix.SetActive(false);
                    needleHUD.needleSeven.SetActive(false);
                    needleHUD.needleEight.SetActive(false);
                    needleHUD.needleNine.SetActive(false);
                    break;
                case 2:
                    needleHUD.needleZero.SetActive(true);
                    needleHUD.needleOne.SetActive(true);
                    needleHUD.needleTwo.SetActive(false);
                    needleHUD.needleThree.SetActive(false);
                    needleHUD.needleFour.SetActive(false);
                    needleHUD.needleFive.SetActive(false);
                    needleHUD.needleSix.SetActive(false);
                    needleHUD.needleSeven.SetActive(false);
                    needleHUD.needleEight.SetActive(false);
                    needleHUD.needleNine.SetActive(false);
                    break;
                case 3:
                    needleHUD.needleZero.SetActive(true);
                    needleHUD.needleOne.SetActive(true);
                    needleHUD.needleTwo.SetActive(true);
                    needleHUD.needleThree.SetActive(false);
                    needleHUD.needleFour.SetActive(false);
                    needleHUD.needleFive.SetActive(false);
                    needleHUD.needleSix.SetActive(false);
                    needleHUD.needleSeven.SetActive(false);
                    needleHUD.needleEight.SetActive(false);
                    needleHUD.needleNine.SetActive(false);
                    break;
                case 4:
                    needleHUD.needleZero.SetActive(true);
                    needleHUD.needleOne.SetActive(true);
                    needleHUD.needleTwo.SetActive(true);
                    needleHUD.needleThree.SetActive(true);
                    needleHUD.needleFour.SetActive(false);
                    needleHUD.needleFive.SetActive(false);
                    needleHUD.needleSix.SetActive(false);
                    needleHUD.needleSeven.SetActive(false);
                    needleHUD.needleEight.SetActive(false);
                    needleHUD.needleNine.SetActive(false);
                    break;
                case 5:
                    needleHUD.needleZero.SetActive(true);
                    needleHUD.needleOne.SetActive(true);
                    needleHUD.needleTwo.SetActive(true);
                    needleHUD.needleThree.SetActive(true);
                    needleHUD.needleFour.SetActive(true);
                    needleHUD.needleFive.SetActive(false);
                    needleHUD.needleSix.SetActive(false);
                    needleHUD.needleSeven.SetActive(false);
                    needleHUD.needleEight.SetActive(false);
                    needleHUD.needleNine.SetActive(false);
                    break;
                case 6:
                    needleHUD.needleZero.SetActive(true);
                    needleHUD.needleOne.SetActive(true);
                    needleHUD.needleTwo.SetActive(true);
                    needleHUD.needleThree.SetActive(true);
                    needleHUD.needleFour.SetActive(true);
                    needleHUD.needleFive.SetActive(true);
                    needleHUD.needleSix.SetActive(false);
                    needleHUD.needleSeven.SetActive(false);
                    needleHUD.needleEight.SetActive(false);
                    needleHUD.needleNine.SetActive(false);
                    break;
                case 7:
                    needleHUD.needleZero.SetActive(true);
                    needleHUD.needleOne.SetActive(true);
                    needleHUD.needleTwo.SetActive(true);
                    needleHUD.needleThree.SetActive(true);
                    needleHUD.needleFour.SetActive(true);
                    needleHUD.needleFive.SetActive(true);
                    needleHUD.needleSix.SetActive(true);
                    needleHUD.needleSeven.SetActive(false);
                    needleHUD.needleEight.SetActive(false);
                    needleHUD.needleNine.SetActive(false);
                    break;
                case 8:
                    needleHUD.needleZero.SetActive(true);
                    needleHUD.needleOne.SetActive(true);
                    needleHUD.needleTwo.SetActive(true);
                    needleHUD.needleThree.SetActive(true);
                    needleHUD.needleFour.SetActive(true);
                    needleHUD.needleFive.SetActive(true);
                    needleHUD.needleSix.SetActive(true);
                    needleHUD.needleSeven.SetActive(true);
                    needleHUD.needleEight.SetActive(false);
                    needleHUD.needleNine.SetActive(false);
                    break;
                case 9:
                    needleHUD.needleZero.SetActive(true);
                    needleHUD.needleOne.SetActive(true);
                    needleHUD.needleTwo.SetActive(true);
                    needleHUD.needleThree.SetActive(true);
                    needleHUD.needleFour.SetActive(true);
                    needleHUD.needleFive.SetActive(true);
                    needleHUD.needleSix.SetActive(true);
                    needleHUD.needleSeven.SetActive(true);
                    needleHUD.needleEight.SetActive(true);
                    needleHUD.needleNine.SetActive(false);
                    break;
                case 10:
                    needleHUD.needleZero.SetActive(true);
                    needleHUD.needleOne.SetActive(true);
                    needleHUD.needleTwo.SetActive(true);
                    needleHUD.needleThree.SetActive(true);
                    needleHUD.needleFour.SetActive(true);
                    needleHUD.needleFive.SetActive(true);
                    needleHUD.needleSix.SetActive(true);
                    needleHUD.needleSeven.SetActive(true);
                    needleHUD.needleEight.SetActive(true);
                    needleHUD.needleNine.SetActive(true);
                    break;

            }
            #endregion
        }
        //needle color controller
        private void HudColor()
        {
            #region hudColorTracking
            if (bd > 9f * butcheredDurationPercent)
            {
                Color newColor = Color.white;
                needleHUD.needleImgZero.color = newColor;
                needleHUD.needleImgOne.color = newColor;
                needleHUD.needleImgTwo.color = newColor;
                needleHUD.needleImgThree.color = newColor;
                needleHUD.needleImgFour.color = newColor;
                needleHUD.needleImgFive.color = newColor;
                needleHUD.needleImgSix.color = newColor;
                needleHUD.needleImgSeven.color = newColor;
                needleHUD.needleImgEight.color = newColor;
                needleHUD.needleImgNine.color = newColor;
            }
            if (bd <= 9f * butcheredDurationPercent && bd > 8f * butcheredDurationPercent)
            {
                Color newColor = new Color(245f / 255f, 232 / 255f, 231 / 255f);
                newColor.a = 1;
                needleHUD.needleImgZero.color = newColor;
                needleHUD.needleImgOne.color = newColor;
                needleHUD.needleImgTwo.color = newColor;
                needleHUD.needleImgThree.color = newColor;
                needleHUD.needleImgFour.color = newColor;
                needleHUD.needleImgFive.color = newColor;
                needleHUD.needleImgSix.color = newColor;
                needleHUD.needleImgSeven.color = newColor;
                needleHUD.needleImgEight.color = newColor;
                needleHUD.needleImgNine.color = newColor;
            }
            if (bd <= 8f * butcheredDurationPercent && bd > 7f * butcheredDurationPercent)
            {
                Color newColor = new Color(237f / 255f, 209f / 255f, 207f / 255f);
                newColor.a = 1;
                needleHUD.needleImgZero.color = newColor;
                needleHUD.needleImgOne.color = newColor;
                needleHUD.needleImgTwo.color = newColor;
                needleHUD.needleImgThree.color = newColor;
                needleHUD.needleImgFour.color = newColor;
                needleHUD.needleImgFive.color = newColor;
                needleHUD.needleImgSix.color = newColor;
                needleHUD.needleImgSeven.color = newColor;
                needleHUD.needleImgEight.color = newColor;
                needleHUD.needleImgNine.color = newColor;
            }
            if (bd <= 7f * butcheredDurationPercent && bd > 6f * butcheredDurationPercent)
            {
                Color newColor = new Color(227f / 255f, 187f / 255f, 183f / 255f);
                newColor.a = 1;
                needleHUD.needleImgZero.color = newColor;
                needleHUD.needleImgOne.color = newColor;
                needleHUD.needleImgTwo.color = newColor;
                needleHUD.needleImgThree.color = newColor;
                needleHUD.needleImgFour.color = newColor;
                needleHUD.needleImgFive.color = newColor;
                needleHUD.needleImgSix.color = newColor;
                needleHUD.needleImgSeven.color = newColor;
                needleHUD.needleImgEight.color = newColor;
                needleHUD.needleImgNine.color = newColor;
            }
            if (bd <= 6f * butcheredDurationPercent && bd > 5f * butcheredDurationPercent)
            {
                Color newColor = new Color(217f / 255f, 165f / 255f, 160f / 255f);
                newColor.a = 1;
                needleHUD.needleImgZero.color = newColor;
                needleHUD.needleImgOne.color = newColor;
                needleHUD.needleImgTwo.color = newColor;
                needleHUD.needleImgThree.color = newColor;
                needleHUD.needleImgFour.color = newColor;
                needleHUD.needleImgFive.color = newColor;
                needleHUD.needleImgSix.color = newColor;
                needleHUD.needleImgSeven.color = newColor;
                needleHUD.needleImgEight.color = newColor;
                needleHUD.needleImgNine.color = newColor;
            }
            if (bd <= 5f * butcheredDurationPercent && bd > 4f * butcheredDurationPercent)
            {
                Color newColor = new Color(205f / 255f, 143f / 255f, 138f / 255f);
                newColor.a = 1;
                needleHUD.needleImgZero.color = newColor;
                needleHUD.needleImgOne.color = newColor;
                needleHUD.needleImgTwo.color = newColor;
                needleHUD.needleImgThree.color = newColor;
                needleHUD.needleImgFour.color = newColor;
                needleHUD.needleImgFive.color = newColor;
                needleHUD.needleImgSix.color = newColor;
                needleHUD.needleImgSeven.color = newColor;
                needleHUD.needleImgEight.color = newColor;
                needleHUD.needleImgNine.color = newColor;
            }
            if (bd <= 4f * butcheredDurationPercent && bd > 3f * butcheredDurationPercent)
            {
                Color newColor = new Color(193f / 255f, 121f / 255f, 116f / 255f);
                newColor.a = 1;
                needleHUD.needleImgZero.color = newColor;
                needleHUD.needleImgOne.color = newColor;
                needleHUD.needleImgTwo.color = newColor;
                needleHUD.needleImgThree.color = newColor;
                needleHUD.needleImgFour.color = newColor;
                needleHUD.needleImgFive.color = newColor;
                needleHUD.needleImgSix.color = newColor;
                needleHUD.needleImgSeven.color = newColor;
                needleHUD.needleImgEight.color = newColor;
                needleHUD.needleImgNine.color = newColor;
            }
            if (bd <= 3f * butcheredDurationPercent && bd > 2f * butcheredDurationPercent)
            {
                Color newColor = new Color(181f / 255f, 100f / 255f, 95f / 255f);
                newColor.a = 1;
                needleHUD.needleImgZero.color = newColor;
                needleHUD.needleImgOne.color = newColor;
                needleHUD.needleImgTwo.color = newColor;
                needleHUD.needleImgThree.color = newColor;
                needleHUD.needleImgFour.color = newColor;
                needleHUD.needleImgFive.color = newColor;
                needleHUD.needleImgSix.color = newColor;
                needleHUD.needleImgSeven.color = newColor;
                needleHUD.needleImgEight.color = newColor;
                needleHUD.needleImgNine.color = newColor;
            }
            if (bd <= 2f * butcheredDurationPercent && bd > 1f * butcheredDurationPercent)
            {
                Color newColor = new Color(168f / 255f, 78f / 255f, 74f / 255f);
                newColor.a = 1;
                needleHUD.needleImgZero.color = newColor;
                needleHUD.needleImgOne.color = newColor;
                needleHUD.needleImgTwo.color = newColor;
                needleHUD.needleImgThree.color = newColor;
                needleHUD.needleImgFour.color = newColor;
                needleHUD.needleImgFive.color = newColor;
                needleHUD.needleImgSix.color = newColor;
                needleHUD.needleImgSeven.color = newColor;
                needleHUD.needleImgEight.color = newColor;
                needleHUD.needleImgNine.color = newColor;
            }
            if (bd <= 1f * butcheredDurationPercent && bd > 0f * butcheredDurationPercent)
            {
                Color newColor = new Color(154f / 255f, 55f / 255f, 55f / 255f);
                newColor.a = 1;
                needleHUD.needleImgZero.color = newColor;
                needleHUD.needleImgOne.color = newColor;
                needleHUD.needleImgTwo.color = newColor;
                needleHUD.needleImgThree.color = newColor;
                needleHUD.needleImgFour.color = newColor;
                needleHUD.needleImgFive.color = newColor;
                needleHUD.needleImgSix.color = newColor;
                needleHUD.needleImgSeven.color = newColor;
                needleHUD.needleImgEight.color = newColor;
                needleHUD.needleImgNine.color = newColor;
            }
            #endregion
        }
        private static void Unhook()
        {
            On.RoR2.HealthComponent.Heal -= new On.RoR2.HealthComponent.hook_Heal(HealthComponent_Heal);
            On.RoR2.CharacterModel.UpdateOverlays -= new On.RoR2.CharacterModel.hook_UpdateOverlays(CharacterModel_UpdateOverlays);
        }
        public void OnDestroy()
        {
            Unhook();
        }

    }
}