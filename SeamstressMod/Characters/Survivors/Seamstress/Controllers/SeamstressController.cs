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

        private float butcheredConversion = 0f;

        public float savedConverstion = 0f;

        private bool hasPlayed = false;

        private bool butchered = false;

        private bool hasFired = false;

        private float leapLength = 0f;

        private float bd = 0f;

        public bool fuckYou = false;

        private bool fuck = false;

        public void Awake()
        {
            characterBody = base.GetComponent<CharacterBody>();
            characterMotor = base.GetComponent<CharacterMotor>();
            healthComponent = base.GetComponent<HealthComponent>();
            skillLocator = base.GetComponent<SkillLocator>();
        }
        public void FixedUpdate()
        {
            if(leapLength > 0f) 
            {
                leapLength -= Time.fixedDeltaTime;
            }
            if(bd > 0f)
            {
                bd -= Time.fixedDeltaTime;
            }
            LeapEnd();
            CalculateBonusDamage();
            ButcheredSound();
            IsButchered();      
        }
        private void LeapEnd()
        {
            if(skillLocator.utility.skillOverrides.Any() && leapLength <= 0f && !fuck)
            {
                fuck = true;
                leapLength = 2f;
            }
            else if(leapLength < 0f && (characterMotor.isGrounded || !skillLocator.utility.skillOverrides.Any()) && fuck)
            {
                fuck = false;
                leapLength = 0f;
                if (skillLocator.utility.skillOverrides.Any())
                {
                    skillLocator.utility.UnsetSkillOverride(gameObject, SeamstressAssets.weaveRecastSkillDef, GenericSkill.SkillOverridePriority.Contextual);
                }
            }
        }
        public float GetButcheredConversion()
        {
            return savedConverstion;
        }
        public void ButcheredConversionCalc(float healDamage)
        {
            butcheredConversion += healDamage;
            savedConverstion = butcheredConversion;
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
                UnityEngine.Object.Instantiate<GameObject>(endReap, characterBody.modelLocator.transform);
                Util.PlaySound("Play_voidman_transform_return", characterBody.gameObject);
                //fire expunge at end of butchered
            }
            if(bd < 0f && !hasFired)
            {
                if (skillLocator.special == skillLocator.FindSkill("reapRecast"))
                {
                    if(skillLocator.special.ExecuteIfReady())
                    {
                        hasFired = true;
                    }
                }
                else if(!fuckYou && skillLocator.special != skillLocator.FindSkill("reapRecast"))
                {
                    hasFired = true;
                }
                butcheredConversion = characterBody.damage;
            }

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
            float fakeHealthMissing = (healthComponent.fullHealth + healthComponent.fullShield) * 0.5f;
            if(fuckYou && skillLocator.special.skillNameToken == SeamstressSurvivor.SEAMSTRESS_PREFIX + "SPECIAL_PARRY_NAME") characterBody.baseDamage = 10f + (fakeHealthMissing * SeamstressStaticValues.passiveScaling) + (healthMissing * SeamstressStaticValues.passiveScaling);
            else characterBody.baseDamage = 10f + (healthMissing * SeamstressStaticValues.passiveScaling);
        }
    }
}