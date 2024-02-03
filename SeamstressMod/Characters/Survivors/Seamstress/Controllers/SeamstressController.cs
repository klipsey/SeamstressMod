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
using UnityEngine.Networking;
using EntityStates;

namespace SeamstressMod.Survivors.Seamstress
{
    public class SeamstressController : MonoBehaviour
    {
        private CharacterBody characterBody;

        private HealthComponent healthComponent;

        private SkillLocator skillLocator;

        internal float bd = 0f;

        internal float butcheredDurationPercent;

        private float butcheredConversion = 0f;

        private int baseNeedleAmount = 0;

        private int needleCount = 0;

        private bool hasPlayed = false;

        private bool fuckYou = false;

        private bool butchered = false;
        public void Awake()
        {
            Hook();
        }
        public void Start()
        {
            characterBody = GetComponent<CharacterBody>();
            healthComponent = GetComponent<HealthComponent>();
            skillLocator = GetComponent<SkillLocator>();
            butcheredDurationPercent = bd / 10f;
        }
        public void FixedUpdate()
        {
            //stopwatch
            if (bd > 0f)
            {
                bd -= Time.fixedDeltaTime;
            }
            if (bd <= 0f && butchered)
            {
                ButcheredEnd();
            }
            NeedleTracking();
            CalculateBonusDamage();
            GetComponent<NeedleHUD>().NeedleDisplayCount(needleCount);
            IsButchered();
            GetComponent<NeedleHUD>().HudColor(bd, butcheredDurationPercent);
            ButcheredSound();
            PassiveNeedleRegen();
        }

        #region hooks
        private void Hook()
        {
            On.RoR2.HealthComponent.Heal += new On.RoR2.HealthComponent.hook_Heal(HealthComponent_Heal);
            On.RoR2.CharacterModel.UpdateOverlays += new On.RoR2.CharacterModel.hook_UpdateOverlays(CharacterModel_UpdateOverlays);
        }
        //butchered overlay
        private void CharacterModel_UpdateOverlays(On.RoR2.CharacterModel.orig_UpdateOverlays orig, CharacterModel self)
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
        private float HealthComponent_Heal(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen = true)
        {
            if (self.body.HasBuff(SeamstressBuffs.butchered) && self.body.baseNameToken == "KENKO_SEAMSTRESS_NAME")
            {
                amount *= SeamstressStaticValues.healConversion;
            }
            var res = orig(self, amount, procChainMask, nonRegen);
            SeamstressController s = self.body.GetComponent<SeamstressController>();
            if (self.body.TryGetComponent<SeamstressController>(out s) && self.body.HasBuff(SeamstressBuffs.butchered))
            {
                s.ButcheredConversionCalc((res / SeamstressStaticValues.healConversion) * 1 - SeamstressStaticValues.healConversion);
            }
            return res;
        }
        #endregion
        public float GetButcheredConversion()
        {
            float num = butcheredConversion;
            butcheredConversion = characterBody.damage;
            characterBody.GetComponent<NeedleHUD>().ActivateExpungeHud(false);
            return num;
        }
        public void ButcheredConversionCalc(float healDamage)
        {
            butcheredConversion += healDamage;
        }
        private void NeedleTracking()
        {
            baseNeedleAmount = skillLocator.special.maxStock - 1;
            needleCount = characterBody.GetBuffCount(SeamstressBuffs.needles) - baseNeedleAmount;
        }
        public int ReturnNeedle(bool baseOrCurrent)
        {
            if(baseOrCurrent)
            {
                return baseNeedleAmount;
            }
            else
            {
                return needleCount;
            }
        }
        //needle regen
        private void PassiveNeedleRegen()
        {
            if (characterBody.GetBuffCount(SeamstressBuffs.needles) < SeamstressStaticValues.maxNeedleAmount + baseNeedleAmount)
            {
                if (!characterBody.HasBuff(SeamstressBuffs.needleCountDownBuff))
                {
                    if(NetworkServer.active)
                    {
                        characterBody.AddTimedBuff(SeamstressBuffs.needleCountDownBuff, SeamstressStaticValues.needleGainInterval);
                        characterBody.AddBuff(SeamstressBuffs.needles);
                    }
                    Util.PlaySound("Play_treeBot_m1_hit_heal", characterBody.gameObject);
                }
            }
        }
        private void IsButchered()
        {
            if (characterBody.HasBuff(SeamstressBuffs.butchered) && !butchered)
            {
                bd = SeamstressStaticValues.butcheredDuration;
                butcheredDurationPercent = bd / 10f;
                characterBody.GetComponent<NeedleHUD>().ActivateExpungeHud(true);
                butchered = true;
                #region IconUpdate
                skillLocator.primary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texStingerIcon");
                skillLocator.secondary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texPistolIcon");
                skillLocator.special.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texScepterSpecialIcon");
                #endregion
                Transform modelTransform = characterBody.modelLocator.modelTransform;
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

            if (characterBody.HasBuff(SeamstressBuffs.butchered))
            {
                characterBody.GetComponent<NeedleHUD>().UpdateExpunge(butcheredConversion);
            }
        }
        private void ButcheredEnd()
        {
            if(characterBody && characterBody.master)
            {
                if (!characterBody.HasBuff(SeamstressBuffs.butchered) && butchered)
                {
                    butchered = false;
                    fuckYou = false;
                    UnityEngine.Object.Instantiate<GameObject>(SeamstressAssets.reapEndEffect, characterBody.modelLocator.transform);
                    Util.PlaySound("Play_voidman_transform_return", characterBody.gameObject);
                    #region iconUpdate
                    skillLocator.primary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texPrimaryIcon");
                    skillLocator.secondary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texSecondaryIcon");
                    skillLocator.special.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texSpecialIcon");
                    //fire expunge at end of butchered
                    if (skillLocator.utility.skillOverrides.Any())
                    {
                        skillLocator.utility.ExecuteIfReady();
                    }
                    #endregion
                }
            }
        }
        //butchered end sound
        private void ButcheredSound()
        {
            if (characterBody.HasBuff(SeamstressBuffs.butchered))
            {
                if (bd < 2f && !hasPlayed)
                {
                    Util.PlaySound("Play_nullifier_impact", characterBody.gameObject);
                    hasPlayed = true;
                }
            }
            hasPlayed = false;
        }
        //passive damage
        private void CalculateBonusDamage()
        {
            float healthMissing = (healthComponent.fullHealth + healthComponent.fullShield) - (healthComponent.health + healthComponent.shield);
            characterBody.baseDamage = 10f + (healthMissing * SeamstressStaticValues.passiveScaling);
        }
        private void Unhook()
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