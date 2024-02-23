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
    public class SeamstressController : MonoBehaviour
    {
        private CharacterBody characterBody;

        private CharacterMotor characterMotor;

        private HealthComponent healthComponent;

        private SkillLocator skillLocator;

        private GameObject endReap = SeamstressAssets.reapEndEffect;

        public float fiendGauge = 0f;

        public float hopoopFeatherTimer;

        private bool hasPlayed = false;

        private bool butchered = false;

        private bool hasFired = false;

        //private float leapLength = 0f;

        //public float lockOutLength = 0f;

        private float bd = 0f;

        public bool fuckYou = false;

        public bool blinkReady = true;

        public void Awake()
        {
            characterBody = base.GetComponent<CharacterBody>();
            characterMotor = base.GetComponent<CharacterMotor>();
            healthComponent = base.GetComponent<HealthComponent>();
            skillLocator = base.GetComponent<SkillLocator>();
        }
        public void FixedUpdate()
        {
            hopoopFeatherTimer -= Time.fixedDeltaTime;
            #region Scrapped
            /*
            if (leapLength > 0f) 
            {
                leapLength -= Time.fixedDeltaTime;
            }
            if(lockOutLength > 0f)
            {
                lockOutLength -= Time.fixedDeltaTime;
            }
            */
            //LeapEnd();
            //ExhaustEnd();
            #endregion
            if (bd > 0f)
            {
                bd -= Time.fixedDeltaTime;
            }
            CalculateBonusDamage();
            ButcheredSound();
            IsButchered();      
        }
        #region scrapped
        /*
        private void LeapEnd()
        {
            if(skillLocator.utility.skillOverrides.Any() && leapLength <= 0f && !fuck && skillLocator.utility.skillDef == SeamstressAssets.weaveRecastSkillDef)
            {
                fuck = true;
                leapLength = 2f;
            }
            else if(leapLength < 0f && (characterMotor.isGrounded || !skillLocator.utility.skillOverrides.Any()) && fuck)
            {
                fuck = false;
                leapLength = 0f;
                if (skillLocator.utility.skillOverrides.Any() && skillLocator.utility.skillDef == SeamstressAssets.weaveRecastSkillDef) skillLocator.utility.UnsetSkillOverride(gameObject, SeamstressAssets.weaveRecastSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            }
        }


        private void ExhaustEnd()
        {
            if (skillLocator.secondary.skillOverrides.Any() && lockOutLength < 0f && skillLocator.secondary.skillDef == SeamstressAssets.lockOutSkillDef)
            {
                int holdStocks = skillLocator.secondary.stock;
                Log.Debug("held stocks" + holdStocks);
                skillLocator.secondary.UnsetSkillOverride(gameObject, SeamstressAssets.lockOutSkillDef, GenericSkill.SkillOverridePriority.Contextual);
                if(holdStocks + skillLocator.secondary.stock > skillLocator.secondary.maxStock)
                {
                    skillLocator.secondary.stock = skillLocator.secondary.maxStock;
                }
                else
                {
                    skillLocator.secondary.stock += holdStocks;
                }
                lockOutLength = 0f;
            }
        }
        */
        #endregion
        public void ButcheredConversionCalc(float healDamage)
        {
            if((fiendGauge + healDamage) < (5 * healthComponent.fullHealth))
            {
                fiendGauge += healDamage;
            }
            else if((fiendGauge + healDamage) >= (5 * healthComponent.fullHealth) && fiendGauge != (5 * healthComponent.fullHealth))
            {
                fiendGauge += (5 * healthComponent.fullHealth) - fiendGauge; 
            }
        }
        private void IsButchered()
        {
            if (fuckYou && !butchered)
            {
                bd = SeamstressStaticValues.butcheredDuration;
                butchered = true;
                hasFired = false;
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
            else if (!fuckYou && butchered)
            {
                butchered = false;
                Transform modelTransform = characterBody.modelLocator.modelTransform;
                if (modelTransform)
                {
                    TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = 1f;
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = SeamstressAssets.destealthMaterial;
                temporaryOverlay.inspectorCharacterModel = modelTransform.GetComponent<CharacterModel>();
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.animateShaderAlpha = true;
                }
                UnityEngine.Object.Instantiate<GameObject>(endReap, characterBody.modelLocator.transform);
                Util.PlaySound("Play_voidman_transform_return", characterBody.gameObject);
                //fire expunge at end of butchered
            }
            /*
            if(bd < 0f && !hasFired)
            {
                if (skillLocator.special == skillLocator.FindSkill("reapRecast"))
                {
                    if(skillLocator.special.ExecuteIfReady())
                    {
                        hasFired = true;
                        butcheredConversion = characterBody.damage;
                    }
                }
                else if(!fuckYou && skillLocator.special != skillLocator.FindSkill("reapRecast"))
                {
                    hasFired = true;
                }
            }
            */
        }
        //butchered end sound
        private void ButcheredSound()
        {
            if (fuckYou)
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
            float fakeHealthMissing = (healthComponent.fullHealth) * 0.5f;
            if(fuckYou && skillLocator.special.skillNameToken == SeamstressSurvivor.SEAMSTRESS_PREFIX + "UTILITY_PARRY_NAME") characterBody.baseDamage = 8f + (fakeHealthMissing * SeamstressStaticValues.passiveScaling) + (healthMissing * SeamstressStaticValues.passiveScaling);
            else characterBody.baseDamage = 8f + (healthMissing * SeamstressStaticValues.passiveScaling);
        }
    }
}