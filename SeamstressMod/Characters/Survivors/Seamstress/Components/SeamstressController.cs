using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.EntityLogic;
using UnityEngine.Networking;
using System.Linq;

namespace SeamstressMod.Survivors.Seamstress
{
    public class SeamstressController : NetworkBehaviour
    {
        private CharacterBody characterBody;

        private CharacterMotor characterMotor;

        private HealthComponent healthComponent;

        private SkillLocator skillLocator;

        private GameObject endReap = SeamstressAssets.reapEndEffect;

        public static GameObject supaPrefab = SeamstressAssets.blinkPrefab;

        private float fiendGauge = 0f;

        private float drainAmount;

        private float dashStopwatch;

        public float hopoopFeatherTimer;

        public float blinkCd;

        public bool blinkReady;

        private bool hasPlayed = false;

        private bool hasPlayedEffect = true;

        public bool isDashing;

        public Vector3 heldDashVector;

        public Vector3 heldOrigin;

        public Vector3 snapBackPosition;
        //private float leapLength = 0f;

        //public float lockOutLength = 0f;

        private float butcheredDuration = 0f;

        private float cooldownRefund;

        public bool inButchered = false;

        public bool runButchered;

        public bool drainGauge;

        private bool hasRefunded;
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
            blinkCd -= Time.fixedDeltaTime;
            if (butcheredDuration > 0f) butcheredDuration -= Time.fixedDeltaTime;
            if (dashStopwatch > 0 && !hasPlayedEffect) dashStopwatch -= Time.fixedDeltaTime;
            RefundUtil();
            DrainGauge();
            CreateBlinkEffect(heldOrigin);
            CalculateBonusDamage();
            ButcheredSound();
            IsButchered();
        }
        private void RefundUtil()
        {
            if (!hasRefunded && skillLocator.utility.skillOverrides.Any() && skillLocator.utility.skillDef == SeamstressAssets.snapBackSkillDef)
            {
                cooldownRefund += Time.fixedDeltaTime;
                hasRefunded = true;
            }
            else if (hasRefunded && !skillLocator.utility.skillOverrides.Any() && skillLocator.utility.skillName == "HeartDashSeamstress")
            {
                skillLocator.utility.rechargeStopwatch += cooldownRefund;
                cooldownRefund = 0f;
                hasRefunded = false;
            }
        }
        private void DrainGauge()
        {
            if (drainGauge)
            {
                if (fiendGauge > 0f)
                {
                    fiendGauge -= drainAmount;
                    if (healthComponent.health < healthComponent.fullHealth) healthComponent.Heal(drainAmount / 8, default, false);
                }
                else if (fiendGauge < 0f)
                {
                    drainGauge = false;
                    fiendGauge = 0f;
                }
            }
        }
        private void CreateBlinkEffect(Vector3 origin)
        {
            if (supaPrefab && !hasPlayedEffect && dashStopwatch < 0)
            {
                EffectData effectData = new EffectData();
                effectData.rotation = Util.QuaternionSafeLookRotation(heldDashVector);
                effectData.origin = origin;
                effectData.scale = 1f;
                EffectManager.SpawnEffect(supaPrefab, effectData, transmit: true);
                hasPlayedEffect = true;
            }
        }
        public void StartDashEffectTimer()
        {
            dashStopwatch = 0.75f;
            hasPlayedEffect = false;
        }
        public void RefreshBlink()
        {
            this.blinkReady = true;
        }
        public void ImpGaugeCalc(float healDamage)
        {
            if((fiendGauge + healDamage) < (healthComponent.fullHealth * SeamstressStaticValues.maxFiendGaugeCoefficient))
            {
                fiendGauge += healDamage;
            }
            else
            {
                fiendGauge = healthComponent.fullHealth * SeamstressStaticValues.maxFiendGaugeCoefficient;
            }
        }
        public float ImpGaugeAmount()
        {
            return fiendGauge;
        }
        public float ImpGaugeAmountPercent()
        {
            return (fiendGauge / (healthComponent.fullHealth * SeamstressStaticValues.maxFiendGaugeCoefficient));
        }
        private void IsButchered()
        {
            if (inButchered && !runButchered)
            {
                drainGauge = false;
                butcheredDuration = SeamstressStaticValues.butcheredDuration;
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
                runButchered = true;
            }
            else if (!inButchered && runButchered)
            {
                drainAmount = healthComponent.fullHealth / 125;
                drainGauge = true;
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
                if(skillLocator.utility.skillDef == SeamstressAssets.snapBackSkillDef)
                {
                    skillLocator.utility.ExecuteIfReady();
                }
                UnityEngine.Object.Instantiate<GameObject>(endReap, characterBody.modelLocator.transform);
                Util.PlaySound("Play_voidman_transform_return", characterBody.gameObject);
                runButchered = false;
            }
        }
        //butchered end sound
        private void ButcheredSound()
        {
            if (inButchered)
            {
                if (butcheredDuration < 2f && !hasPlayed)
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
            if(inButchered && skillLocator.utility.skillNameToken == SeamstressSurvivor.SEAMSTRESS_PREFIX + "UTILITY_PARRY_NAME") characterBody.baseDamage = 8f + (fakeHealthMissing * SeamstressStaticValues.passiveScaling) + (healthMissing * SeamstressStaticValues.passiveScaling);
            else characterBody.baseDamage = 8f + (healthMissing * SeamstressStaticValues.passiveScaling);
        }
    }
}