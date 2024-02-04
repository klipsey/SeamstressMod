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
using TMPro;

namespace SeamstressMod.Survivors.Seamstress
{
    public class SeamstressController : NetworkBehaviour
    {
        private CharacterBody characterBody;

        private HealthComponent healthComponent;

        private SkillLocator skillLocator;

        private GameObject endReap = SeamstressAssets.reapEndEffect;

        public int needleCount = 0;

        public float bd = 0f;

        public float butcheredDurationPercent = 0f;

        private float butcheredConversion = 0f;

        private int baseNeedleAmount = 0;

        private bool hasPlayed = false;

        public bool fuckYou = false;

        private bool butchered = false;

        public void Awake()
        {
            characterBody = base.GetComponent<CharacterBody>();
            healthComponent = base.GetComponent<HealthComponent>();
            skillLocator = base.GetComponent<SkillLocator>();
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
            IsButchered();
            ButcheredSound();
            PassiveNeedleRegen();
        }
        public float getButcheredValues(bool percentOrStopwatch)
        {
            if(percentOrStopwatch)
            {
                return butcheredDurationPercent;
            }
            else
            {
                return bd;
            }
        }
        public float GetButcheredConversion()
        {
            float num = butcheredConversion;
            butcheredConversion = characterBody.damage;
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
                butchered = true;
                #region IconUpdate
                skillLocator.primary.skillDef.icon = SeamstressAssets.primaryEmp;
                skillLocator.secondary.skillDef.icon = SeamstressAssets.secondaryEmp;
                skillLocator.special.skillDef.icon = SeamstressAssets.specialEmp;
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
        }
        private void ButcheredEnd()
        {
            if(characterBody && characterBody.master)
            {
                if (!characterBody.HasBuff(SeamstressBuffs.butchered) && butchered)
                {
                    butchered = false;
                    fuckYou = false;
                    UnityEngine.Object.Instantiate<GameObject>(endReap, characterBody.modelLocator.transform);
                    Util.PlaySound("Play_voidman_transform_return", characterBody.gameObject);
                    #region iconUpdate
                    skillLocator.primary.skillDef.icon = SeamstressAssets.primary;
                    skillLocator.secondary.skillDef.icon = SeamstressAssets.secondary;
                    skillLocator.special.skillDef.icon = SeamstressAssets.special;
                    //fire expunge at end of butchered
                    if (skillLocator.utility.skillOverrides.Any())
                    {
                        if(base.hasAuthority)
                        {
                            skillLocator.utility.ExecuteIfReady();
                        }
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
    }
}