using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.EntityLogic;
using UnityEngine.Networking;
using System.Linq;
using SeamstressMod.Seamstress.Content;
using R2API.Networking.Interfaces;

namespace SeamstressMod.Seamstress.Components
{
    public class SeamstressController : NetworkBehaviour
    {
        private CharacterBody characterBody;

        private CharacterMotor characterMotor;

        private HealthComponent healthComponent;

        private SkillLocator skillLocator;

        private GameObject endReap = SeamstressAssets.reapEndEffect;

        public static GameObject supaPrefab = SeamstressAssets.blinkPrefab;

        public float fiendMeter = 0f;

        public float maxHunger = 0f;

        private float drainAmount;

        private float dashStopwatch;

        private float checkStatsStopwatch;

        public float hopoopFeatherTimer;

        public float blinkCd = 0f;

        public bool blinkReady;

        private bool hasPlayed = false;

        private bool hasPlayedEffect = true;

        public bool isDashing;

        public Vector3 heldDashVector;

        public Vector3 heldOrigin;

        public Vector3 snapBackPosition;
        //private float leapLength = 0f;

        //public float lockOutLength = 0f;

        private float insatiableDuration = 0f;

        private float cooldownRefund;

        public bool inInsatiable = false;

        public bool hasStartedInsatiable;

        public bool draining;

        private bool hasRefunded;
        public void Awake()
        {
            characterBody = GetComponent<CharacterBody>();
            characterMotor = GetComponent<CharacterMotor>();
            healthComponent = GetComponent<HealthComponent>();
            skillLocator = GetComponent<SkillLocator>();
        }
        public void FixedUpdate()
        {
            hopoopFeatherTimer -= Time.fixedDeltaTime;
            blinkCd += Time.fixedDeltaTime;
            if (insatiableDuration > 0f) insatiableDuration -= Time.fixedDeltaTime;
            if (dashStopwatch > 0 && !hasPlayedEffect) dashStopwatch -= Time.fixedDeltaTime;
            if (checkStatsStopwatch >= 0.5)
            {
                characterBody.MarkAllStatsDirty();
                checkStatsStopwatch = 0f;
            }
            else checkStatsStopwatch += Time.fixedDeltaTime;
            RefundUtil();
            CheckToDrainGauge();
            CreateBlinkEffect(heldOrigin);
            ButcheredSound();
            IsInsatiable();
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
        private void CheckToDrainGauge()
        {
            if (draining)
            {
                if (fiendMeter > 0f)
                {
                    fiendMeter -= drainAmount;
                    if (healthComponent.health < healthComponent.fullHealth) healthComponent.Heal(drainAmount / 8, default, false);
                }
                else if (fiendMeter <= 0f)
                {
                    draining = false;
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
                EffectManager.SpawnEffect(supaPrefab, effectData, transmit: false);
                hasPlayedEffect = true;
            }
        }
        public void StartDashEffectTimer()
        {
            dashStopwatch = 0.75f;
            hasPlayedEffect = false;
        }
        public void FillHunger(float healDamage)
        {
            maxHunger = healthComponent.fullHealth * SeamstressStaticValues.maxFiendGaugeCoefficient;

            if (fiendMeter + healDamage < maxHunger)
            {
                fiendMeter += healDamage;
            }
            else
            {
                fiendMeter = maxHunger;
            }

            if (fiendMeter < 0f)
            {
                draining = true;
                fiendMeter = 0f;
            }

            NetworkIdentity networkIdentity = base.gameObject.GetComponent<NetworkIdentity>();
            if (!networkIdentity)
            {
                return;
            }

            new SyncHunger(networkIdentity.netId, (ulong)(this.fiendMeter * 100f)).Send(R2API.Networking.NetworkDestination.Clients);
        }
        private void IsInsatiable()
        {
            if (inInsatiable && !hasStartedInsatiable)
            {
                draining = false;
                insatiableDuration = SeamstressStaticValues.butcheredDuration;
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
                hasStartedInsatiable = true;
            }
            else if (!inInsatiable && hasStartedInsatiable)
            {
                drainAmount = healthComponent.fullHealth / 125;
                draining = true;
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
                if (skillLocator.utility.skillDef == SeamstressAssets.snapBackSkillDef)
                {
                    skillLocator.utility.ExecuteIfReady();
                }
                Instantiate(endReap, characterBody.modelLocator.transform);
                Util.PlaySound("Play_voidman_transform_return", characterBody.gameObject);
                hasStartedInsatiable = false;
            }
        }
        //butchered end sound
        private void ButcheredSound()
        {
            if (inInsatiable)
            {
                if (insatiableDuration < 2f && !hasPlayed)
                {
                    Util.PlaySound("Play_nullifier_impact", characterBody.gameObject);
                    hasPlayed = true;
                }
            }
            hasPlayed = false;
        }
    }
}