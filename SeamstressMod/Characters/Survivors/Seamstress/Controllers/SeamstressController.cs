using RoR2;
using System.Collections.Generic;
using SeamstressMod.Survivors.Seamstress;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.AddressableAssets;
using RoR2.EntityLogic;

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

        private static IEnumerable<CharacterBody.TimedBuff> timedBuffs;

        public bool hasPlayed;

        public bool fuckYou;

        public bool butchered;

        public float butcheredDurationPercent;

        public int needleCount;

        public TemporaryOverlay component;
        public void Awake()
        {
            this.healthComponent = GetComponent<HealthComponent>();
            this.characterBody = GetComponent<CharacterBody>();
            this.skillLocator = GetComponent<SkillLocator>();
            butcheredConversion = 0f;
            Hook();
        }
        public void Start()
        {
            needleRegen = SeamstressStaticValues.needleGainInterval;
            hasPlayed = false;
            fuckYou = false;
            butchered = false;
            butcheredDurationPercent = Seamstress.SeamstressStaticValues.butcheredDuration / 10f;
        }
        public void FixedUpdate()
        {
            timedBuffs = this.characterBody.timedBuffs.Where((CharacterBody.TimedBuff b) => b.buffIndex == SeamstressBuffs.butchered.buffIndex);
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
                s.GetButcheredConversion((res/SeamstressStaticValues.healConversion)* 1 - SeamstressStaticValues.healConversion);
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
                    this.characterBody.AddTimedBuff(SeamstressBuffs.needleCountDownBuff, SeamstressStaticValues.needleGainInterval);
                    this.characterBody.AddBuff(SeamstressBuffs.needles);
                    Util.PlaySound("Play_treeBot_m1_hit_heal", this.characterBody.gameObject);
                }
            }
        }
        public void GetButcheredConversion(float healDamage)
        {
            butcheredConversion += healDamage;
        }
        //butchered controller
        public void IsButchered()
        {
            //run during butchered
            if (this.characterBody.HasBuff(SeamstressBuffs.butchered))
            {
                //run when butchered starts
                if (!butchered && timedBuffs.Any())
                {
                    butcheredDurationPercent = timedBuffs.FirstOrDefault().timer / 10f;
                    NeedleHUD.expungeHealing.GetComponent<Text>().enabled = true;
                    NeedleHUD.expungeHealing.GetComponent<Outline>().enabled = true;
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
                    butchered = true;
                    #region IconUpdate
                    this.skillLocator.primary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texStingerIcon");
                    this.skillLocator.secondary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texPistolIcon");
                    this.skillLocator.special.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texScepterSpecialIcon");
                    #endregion
                    //FIX CD recdution
                    //this.skillLocator.secondary.rechargeStopwatch += this.skillLocator.secondary.finalRechargeInterval * 0.5f;
                }
            }
            //run when butchered ends
            if(!this.characterBody.HasBuff(SeamstressBuffs.butchered) && butchered)
            {
                butchered = false;
                UnityEngine.Object.Instantiate<GameObject>(SeamstressAssets.reapEndEffect, this.characterBody.modelLocator.transform);
                Util.PlaySound("Play_voidman_transform_return", this.characterBody.gameObject);
                fuckYou = false;
                #region IconUpdate
                this.skillLocator.primary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texPrimaryIcon");
                this.skillLocator.secondary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texSecondaryIcon");
                this.skillLocator.special.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texSpecialIcon");
                #endregion
                //this.skillLocator.secondary.cooldownScale = this.skillLocator.secondary.cooldownScale / 0.5f;
            }
            //fire expunge at end of butchered
            if (!this.characterBody.HasBuff(SeamstressBuffs.butchered) && this.skillLocator.utility == this.skillLocator.FindSkill("reapRecast"))
            {
                this.skillLocator.utility.ExecuteIfReady();
            }
            //expunge hud update
            if (NeedleHUD.expungeHealing.GetComponent<Text>())
            {
                NeedleHUD.expungeHealing.GetComponent<Text>().text = Mathf.Round(butcheredConversion).ToString();
            }
            if(timedBuffs.Any())
            {
                HudColor();
            }
            else
            {
                #region fuck you
                Color newColor = Color.white;
                newColor.a = 1;
                NeedleHUD.needleImgZero.color = newColor;
                NeedleHUD.needleImgOne.color = newColor;
                NeedleHUD.needleImgTwo.color = newColor;
                NeedleHUD.needleImgThree.color = newColor;
                NeedleHUD.needleImgFour.color = newColor;
                NeedleHUD.needleImgFive.color = newColor;
                NeedleHUD.needleImgSix.color = newColor;
                NeedleHUD.needleImgSeven.color = newColor;
                NeedleHUD.needleImgEight.color = newColor;
                NeedleHUD.needleImgNine.color = newColor;
                #endregion
            }
        }
        //butchered end sound
        public void ButcheredSound()
        {
            if (timedBuffs.Any())
            {
                if (timedBuffs.FirstOrDefault().timer < 2f && !hasPlayed)
                {
                    Util.PlaySound("Play_nullifier_impact", this.characterBody.gameObject);
                    hasPlayed = true;
                }
            }
            hasPlayed = false;
        }
        //passive damage
        public void CalculateBonusDamage()
        {
            float healthMissing = (this.healthComponent.fullHealth + this.healthComponent.fullShield) - (this.healthComponent.health + this.healthComponent.shield);
            this.characterBody.baseDamage = 10f + (healthMissing * SeamstressStaticValues.passiveScaling);
        }
        //needle controller
        public void NeedleDisplayCount()
        {
            #region needlehud
            baseNeedleAmount = skillLocator.special.maxStock - 1;
            needleCount = characterBody.GetBuffCount(SeamstressBuffs.needles) - baseNeedleAmount;
            switch (needleCount)
            {
                case 0:
                    NeedleHUD.needleZero.SetActive(false);
                    NeedleHUD.needleOne.SetActive(false);
                    NeedleHUD.needleTwo.SetActive(false);
                    NeedleHUD.needleThree.SetActive(false);
                    NeedleHUD.needleFour.SetActive(false);
                    NeedleHUD.needleFive.SetActive(false);
                    NeedleHUD.needleSix.SetActive(false);
                    NeedleHUD.needleSeven.SetActive(false);
                    NeedleHUD.needleEight.SetActive(false);
                    NeedleHUD.needleNine.SetActive(false);
                    break;
                case 1:
                    NeedleHUD.needleZero.SetActive(true);
                    NeedleHUD.needleOne.SetActive(false);
                    NeedleHUD.needleTwo.SetActive(false);
                    NeedleHUD.needleThree.SetActive(false);
                    NeedleHUD.needleFour.SetActive(false);
                    NeedleHUD.needleFive.SetActive(false);
                    NeedleHUD.needleSix.SetActive(false);
                    NeedleHUD.needleSeven.SetActive(false);
                    NeedleHUD.needleEight.SetActive(false);
                    NeedleHUD.needleNine.SetActive(false);
                    break;
                case 2:
                    NeedleHUD.needleZero.SetActive(true);
                    NeedleHUD.needleOne.SetActive(true);
                    NeedleHUD.needleTwo.SetActive(false);
                    NeedleHUD.needleThree.SetActive(false);
                    NeedleHUD.needleFour.SetActive(false);
                    NeedleHUD.needleFive.SetActive(false);
                    NeedleHUD.needleSix.SetActive(false);
                    NeedleHUD.needleSeven.SetActive(false);
                    NeedleHUD.needleEight.SetActive(false);
                    NeedleHUD.needleNine.SetActive(false);
                    break;
                case 3:
                    NeedleHUD.needleZero.SetActive(true);
                    NeedleHUD.needleOne.SetActive(true);
                    NeedleHUD.needleTwo.SetActive(true);
                    NeedleHUD.needleThree.SetActive(false);
                    NeedleHUD.needleFour.SetActive(false);
                    NeedleHUD.needleFive.SetActive(false);
                    NeedleHUD.needleSix.SetActive(false);
                    NeedleHUD.needleSeven.SetActive(false);
                    NeedleHUD.needleEight.SetActive(false);
                    NeedleHUD.needleNine.SetActive(false);
                    break;
                case 4:
                    NeedleHUD.needleZero.SetActive(true);
                    NeedleHUD.needleOne.SetActive(true);
                    NeedleHUD.needleTwo.SetActive(true);
                    NeedleHUD.needleThree.SetActive(true);
                    NeedleHUD.needleFour.SetActive(false);
                    NeedleHUD.needleFive.SetActive(false);
                    NeedleHUD.needleSix.SetActive(false);
                    NeedleHUD.needleSeven.SetActive(false);
                    NeedleHUD.needleEight.SetActive(false);
                    NeedleHUD.needleNine.SetActive(false);
                    break;
                case 5:
                    NeedleHUD.needleZero.SetActive(true);
                    NeedleHUD.needleOne.SetActive(true);
                    NeedleHUD.needleTwo.SetActive(true);
                    NeedleHUD.needleThree.SetActive(true);
                    NeedleHUD.needleFour.SetActive(true);
                    NeedleHUD.needleFive.SetActive(false);
                    NeedleHUD.needleSix.SetActive(false);
                    NeedleHUD.needleSeven.SetActive(false);
                    NeedleHUD.needleEight.SetActive(false);
                    NeedleHUD.needleNine.SetActive(false);
                    break;
                case 6:
                    NeedleHUD.needleZero.SetActive(true);
                    NeedleHUD.needleOne.SetActive(true);
                    NeedleHUD.needleTwo.SetActive(true);
                    NeedleHUD.needleThree.SetActive(true);
                    NeedleHUD.needleFour.SetActive(true);
                    NeedleHUD.needleFive.SetActive(true);
                    NeedleHUD.needleSix.SetActive(false);
                    NeedleHUD.needleSeven.SetActive(false);
                    NeedleHUD.needleEight.SetActive(false);
                    NeedleHUD.needleNine.SetActive(false);
                    break;
                case 7:
                    NeedleHUD.needleZero.SetActive(true);
                    NeedleHUD.needleOne.SetActive(true);
                    NeedleHUD.needleTwo.SetActive(true);
                    NeedleHUD.needleThree.SetActive(true);
                    NeedleHUD.needleFour.SetActive(true);
                    NeedleHUD.needleFive.SetActive(true);
                    NeedleHUD.needleSix.SetActive(true);
                    NeedleHUD.needleSeven.SetActive(false);
                    NeedleHUD.needleEight.SetActive(false);
                    NeedleHUD.needleNine.SetActive(false);
                    break;
                case 8:
                    NeedleHUD.needleZero.SetActive(true);
                    NeedleHUD.needleOne.SetActive(true);
                    NeedleHUD.needleTwo.SetActive(true);
                    NeedleHUD.needleThree.SetActive(true);
                    NeedleHUD.needleFour.SetActive(true);
                    NeedleHUD.needleFive.SetActive(true);
                    NeedleHUD.needleSix.SetActive(true);
                    NeedleHUD.needleSeven.SetActive(true);
                    NeedleHUD.needleEight.SetActive(false);
                    NeedleHUD.needleNine.SetActive(false);
                    break;
                case 9:
                    NeedleHUD.needleZero.SetActive(true);
                    NeedleHUD.needleOne.SetActive(true);
                    NeedleHUD.needleTwo.SetActive(true);
                    NeedleHUD.needleThree.SetActive(true);
                    NeedleHUD.needleFour.SetActive(true);
                    NeedleHUD.needleFive.SetActive(true);
                    NeedleHUD.needleSix.SetActive(true);
                    NeedleHUD.needleSeven.SetActive(true);
                    NeedleHUD.needleEight.SetActive(true);
                    NeedleHUD.needleNine.SetActive(false);
                    break;
                case 10:
                    NeedleHUD.needleZero.SetActive(true);
                    NeedleHUD.needleOne.SetActive(true);
                    NeedleHUD.needleTwo.SetActive(true);
                    NeedleHUD.needleThree.SetActive(true);
                    NeedleHUD.needleFour.SetActive(true);
                    NeedleHUD.needleFive.SetActive(true);
                    NeedleHUD.needleSix.SetActive(true);
                    NeedleHUD.needleSeven.SetActive(true);
                    NeedleHUD.needleEight.SetActive(true);
                    NeedleHUD.needleNine.SetActive(true);
                    break;

            }
            #endregion
        }
        //needle color controller
        public void HudColor()
        {
            #region hudColorTracking
            if (timedBuffs.FirstOrDefault().timer > 9f * butcheredDurationPercent)
            {
                Color newColor = Color.white;
                NeedleHUD.needleImgZero.color = newColor;
                NeedleHUD.needleImgOne.color = newColor;
                NeedleHUD.needleImgTwo.color = newColor;
                NeedleHUD.needleImgThree.color = newColor;
                NeedleHUD.needleImgFour.color = newColor;
                NeedleHUD.needleImgFive.color = newColor;
                NeedleHUD.needleImgSix.color = newColor;
                NeedleHUD.needleImgSeven.color = newColor;
                NeedleHUD.needleImgEight.color = newColor;
                NeedleHUD.needleImgNine.color = newColor;
            }
            if (timedBuffs.FirstOrDefault().timer <= 9f * butcheredDurationPercent && timedBuffs.FirstOrDefault().timer > 8f * butcheredDurationPercent) 
            {
                Color newColor = new Color(245f / 255f, 232 / 255f, 231 / 255f);
                newColor.a = 1;
                NeedleHUD.needleImgZero.color = newColor;
                NeedleHUD.needleImgOne.color = newColor;
                NeedleHUD.needleImgTwo.color = newColor;
                NeedleHUD.needleImgThree.color = newColor;
                NeedleHUD.needleImgFour.color = newColor;
                NeedleHUD.needleImgFive.color = newColor;
                NeedleHUD.needleImgSix.color = newColor;
                NeedleHUD.needleImgSeven.color = newColor;
                NeedleHUD.needleImgEight.color = newColor;
                NeedleHUD.needleImgNine.color = newColor;
            }
            if (timedBuffs.FirstOrDefault().timer <= 8f * butcheredDurationPercent && timedBuffs.FirstOrDefault().timer > 7f * butcheredDurationPercent)
            {
                Color newColor = new Color(237f / 255f, 209f / 255f, 207f / 255f);
                newColor.a = 1;
                NeedleHUD.needleImgZero.color = newColor;
                NeedleHUD.needleImgOne.color = newColor;
                NeedleHUD.needleImgTwo.color = newColor;
                NeedleHUD.needleImgThree.color = newColor;
                NeedleHUD.needleImgFour.color = newColor;
                NeedleHUD.needleImgFive.color = newColor;
                NeedleHUD.needleImgSix.color = newColor;
                NeedleHUD.needleImgSeven.color = newColor;
                NeedleHUD.needleImgEight.color = newColor;
                NeedleHUD.needleImgNine.color = newColor;
            }
            if(timedBuffs.FirstOrDefault().timer <= 7f * butcheredDurationPercent && timedBuffs.FirstOrDefault().timer > 6f * butcheredDurationPercent)
            {
                Color newColor = new Color(227f / 255f, 187f / 255f, 183f / 255f);
                newColor.a = 1;
                NeedleHUD.needleImgZero.color = newColor;
                NeedleHUD.needleImgOne.color = newColor;
                NeedleHUD.needleImgTwo.color = newColor;
                NeedleHUD.needleImgThree.color = newColor;
                NeedleHUD.needleImgFour.color = newColor;
                NeedleHUD.needleImgFive.color = newColor;
                NeedleHUD.needleImgSix.color = newColor;
                NeedleHUD.needleImgSeven.color = newColor;
                NeedleHUD.needleImgEight.color = newColor;
                NeedleHUD.needleImgNine.color = newColor;
            }
            if (timedBuffs.FirstOrDefault().timer <= 6f * butcheredDurationPercent && timedBuffs.FirstOrDefault().timer > 5f * butcheredDurationPercent)
            {
                Color newColor = new Color(217f / 255f, 165f / 255f, 160f / 255f);
                newColor.a = 1;
                NeedleHUD.needleImgZero.color = newColor;
                NeedleHUD.needleImgOne.color = newColor;
                NeedleHUD.needleImgTwo.color = newColor;
                NeedleHUD.needleImgThree.color = newColor;
                NeedleHUD.needleImgFour.color = newColor;
                NeedleHUD.needleImgFive.color = newColor;
                NeedleHUD.needleImgSix.color = newColor;
                NeedleHUD.needleImgSeven.color = newColor;
                NeedleHUD.needleImgEight.color = newColor;
                NeedleHUD.needleImgNine.color = newColor;
            }
            if (timedBuffs.FirstOrDefault().timer <= 5f * butcheredDurationPercent && timedBuffs.FirstOrDefault().timer > 4f * butcheredDurationPercent)
            {
                Color newColor = new Color(205f / 255f, 143f / 255f, 138f / 255f);
                newColor.a = 1;
                NeedleHUD.needleImgZero.color = newColor;
                NeedleHUD.needleImgOne.color = newColor;
                NeedleHUD.needleImgTwo.color = newColor;
                NeedleHUD.needleImgThree.color = newColor;
                NeedleHUD.needleImgFour.color = newColor;
                NeedleHUD.needleImgFive.color = newColor;
                NeedleHUD.needleImgSix.color = newColor;
                NeedleHUD.needleImgSeven.color = newColor;
                NeedleHUD.needleImgEight.color = newColor;
                NeedleHUD.needleImgNine.color = newColor;
            }
            if (timedBuffs.FirstOrDefault().timer <= 4f * butcheredDurationPercent && timedBuffs.FirstOrDefault().timer > 3f * butcheredDurationPercent)
            {
                Color newColor = new Color(193f / 255f, 121f / 255f, 116f / 255f);
                newColor.a = 1;
                NeedleHUD.needleImgZero.color = newColor;
                NeedleHUD.needleImgOne.color = newColor;
                NeedleHUD.needleImgTwo.color = newColor;
                NeedleHUD.needleImgThree.color = newColor;
                NeedleHUD.needleImgFour.color = newColor;
                NeedleHUD.needleImgFive.color = newColor;
                NeedleHUD.needleImgSix.color = newColor;
                NeedleHUD.needleImgSeven.color = newColor;
                NeedleHUD.needleImgEight.color = newColor;
                NeedleHUD.needleImgNine.color = newColor;
            }
            if (timedBuffs.FirstOrDefault().timer <= 3f * butcheredDurationPercent && timedBuffs.FirstOrDefault().timer > 2f * butcheredDurationPercent)
            {
                Color newColor = new Color(181f / 255f, 100f / 255f, 95f / 255f);
                newColor.a = 1;
                NeedleHUD.needleImgZero.color = newColor;
                NeedleHUD.needleImgOne.color = newColor;
                NeedleHUD.needleImgTwo.color = newColor;
                NeedleHUD.needleImgThree.color = newColor;
                NeedleHUD.needleImgFour.color = newColor;
                NeedleHUD.needleImgFive.color = newColor;
                NeedleHUD.needleImgSix.color = newColor;
                NeedleHUD.needleImgSeven.color = newColor;
                NeedleHUD.needleImgEight.color = newColor;
                NeedleHUD.needleImgNine.color = newColor;
            }
            if (timedBuffs.FirstOrDefault().timer <= 2f * butcheredDurationPercent && timedBuffs.FirstOrDefault().timer > 1f * butcheredDurationPercent)
            {
                Color newColor = new Color(168f / 255f, 78f / 255f, 74f / 255f);
                newColor.a = 1;
                NeedleHUD.needleImgZero.color = newColor;
                NeedleHUD.needleImgOne.color = newColor;
                NeedleHUD.needleImgTwo.color = newColor;
                NeedleHUD.needleImgThree.color = newColor;
                NeedleHUD.needleImgFour.color = newColor;
                NeedleHUD.needleImgFive.color = newColor;
                NeedleHUD.needleImgSix.color = newColor;
                NeedleHUD.needleImgSeven.color = newColor;
                NeedleHUD.needleImgEight.color = newColor;
                NeedleHUD.needleImgNine.color = newColor;
            }
            if (timedBuffs.FirstOrDefault().timer <= 1f * butcheredDurationPercent && timedBuffs.FirstOrDefault().timer > 0f * butcheredDurationPercent)
            {
                Color newColor = new Color(154f / 255f, 55f / 255f, 55f / 255f);
                newColor.a = 1;
                NeedleHUD.needleImgZero.color = newColor;
                NeedleHUD.needleImgOne.color = newColor;
                NeedleHUD.needleImgTwo.color = newColor;
                NeedleHUD.needleImgThree.color = newColor;
                NeedleHUD.needleImgFour.color = newColor;
                NeedleHUD.needleImgFive.color = newColor;
                NeedleHUD.needleImgSix.color = newColor;
                NeedleHUD.needleImgSeven.color = newColor;
                NeedleHUD.needleImgEight.color = newColor;
                NeedleHUD.needleImgNine.color = newColor;
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
