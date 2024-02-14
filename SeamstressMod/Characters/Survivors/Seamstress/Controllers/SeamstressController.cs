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

        private CharacterMotor characterMotor;

        private HealthComponent healthComponent;

        private SkillLocator skillLocator;

        private GameObject endReap = SeamstressAssets.reapEndEffect;

        private float butcheredConversion = 0f;

        private bool hasPlayed = false;

        private bool butchered = false;

        public float bd = 0f;

        public float leapLength = 0f;

        public float butcheredDurationPercent = 0f;

        public bool fuckYou = false;

        public bool fuck = false;

        private void Awake()
        {
            characterBody = base.GetComponent<CharacterBody>();
            characterMotor = base.GetComponent<CharacterMotor>();
            healthComponent = base.GetComponent<HealthComponent>();
            skillLocator = base.GetComponent<SkillLocator>();
            butcheredDurationPercent = bd / 10f;
        }
        private void FixedUpdate()
        {
            //stopwatch
            if (bd > 0f)
            {
                bd -= Time.fixedDeltaTime;
            }
            else if (bd <= 0f && butchered || bd <= 0f && skillLocator.special == skillLocator.FindSkill("reapRecast"))
            {
                ButcheredEnd();
            }
            if(leapLength > 0f) 
            {
                leapLength -= Time.fixedDeltaTime;
                Log.Debug(leapLength + " uuh is this working?");
            }
            RecalcNeedles();
            LeapEnd();
            CalculateBonusDamage();
            IsButchered();
            ButcheredSound();        
        }
        private void RecalcNeedles()
        {
            if(characterBody.GetBuffCount(SeamstressBuffs.needles) < skillLocator.utility.stock)
            {
                characterBody.AddBuff(SeamstressBuffs.needles);
            }
            if(characterBody.GetBuffCount(SeamstressBuffs.needles) > skillLocator.utility.stock)
            {
                characterBody.RemoveBuff(SeamstressBuffs.needles);
            }
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
                if (base.hasAuthority && skillLocator.utility.skillOverrides.Any())
                {
                    skillLocator.utility.UnsetSkillOverride(gameObject, SeamstressAssets.weaveRecastSkillDef, GenericSkill.SkillOverridePriority.Contextual);
                }
            }
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
        //needle regen
        private void IsButchered()
        {
            if (characterBody.HasBuff(SeamstressBuffs.butchered) && !butchered)
            {
                bd = SeamstressStaticValues.butcheredDuration;
                butcheredDurationPercent = bd / 10f;
                butchered = true;
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
                    //fire expunge at end of butchered

                }
                if (skillLocator.special == skillLocator.FindSkill("reapRecast") && !characterBody.HasBuff(SeamstressBuffs.butchered))
                {
                    if (base.hasAuthority)
                    {
                        skillLocator.special.ExecuteIfReady();
                    }
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
            float fakeHealthMissing = (healthComponent.fullHealth + healthComponent.fullShield) * 0.5f;
            if(characterBody.HasBuff(SeamstressBuffs.butchered) && skillLocator.special.skillNameToken == "") characterBody.baseDamage = 12f + (fakeHealthMissing * SeamstressStaticValues.passiveScaling) + (healthMissing * SeamstressStaticValues.passiveScaling);
            else characterBody.baseDamage = 12f + (healthMissing * SeamstressStaticValues.passiveScaling);
        }
    }
}