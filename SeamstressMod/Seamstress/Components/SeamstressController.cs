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

        private CharacterModel charModel;

        private GameObject insatiableEndPrefab = SeamstressAssets.instatiableEndEffect;

        public static GameObject supaPrefab = SeamstressAssets.blinkEffect;

        public float fiendMeter = 0f;

        public float maxHunger;

        private float drainAmount;

        private float parryDashDelayTimer;

        private float checkStatsStopwatch;

        public float blinkCd = 0f;

        public bool blinkReady;

        private bool hasPlayed = false;

        private bool hasPlayedEffect = true;

        public Vector3 heldDashVector;

        public Vector3 heldOrigin;

        public Vector3 snapBackPosition;

        private float insatiableStopwatch = 0f;

        private float cooldownRefund;

        public bool inInsatiable { get; private set; }

        public bool draining;

        private bool hasRefunded;
        public void Awake()
        {
            characterBody = GetComponent<CharacterBody>();
            characterMotor = GetComponent<CharacterMotor>();
            healthComponent = GetComponent<HealthComponent>();
            skillLocator = GetComponent<SkillLocator>();
            charModel = gameObject.GetComponent<CharacterModel>();
        }
        public void Start()
        {
            maxHunger = healthComponent.fullHealth * SeamstressStaticValues.maxFiendGaugeCoefficient;
        }
        public void FixedUpdate()
        {
            blinkCd += Time.fixedDeltaTime;

            if (insatiableStopwatch > 0f)
            {
                insatiableStopwatch -= Time.fixedDeltaTime;
            }
            else if(inInsatiable && insatiableStopwatch <= 0f)
            {
                DeactivateInsatiable();
            }

            if (parryDashDelayTimer > 0)
            {
                parryDashDelayTimer -= Time.fixedDeltaTime;
            }
            else if (parryDashDelayTimer <= 0 && !hasPlayedEffect)
            {
                CreateBlinkEffect(heldOrigin);
            }

            //Updates passive damage ..
            if (checkStatsStopwatch >= 0.5)
            {
                characterBody.MarkAllStatsDirty();
                checkStatsStopwatch = 0f;
            }
            else checkStatsStopwatch += Time.fixedDeltaTime;

            if(draining && fiendMeter > 0f)
            {
                fiendMeter -= drainAmount;
                healthComponent.Heal(drainAmount / 8, default, false);
            }
            else
            {
                fiendMeter = 0;
                draining = false;
            }
            RefundUtil();
            InstatiableSound();
        }
        private void RefundUtil()
        {
            if (!hasRefunded && skillLocator.utility.skillOverrides.Any() && skillLocator.utility.skillDef == SeamstressSurvivor.snapBackSkillDef)
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
        private void CreateBlinkEffect(Vector3 origin)
        {
            if (supaPrefab)
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
            parryDashDelayTimer = 0.75f;
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
                DeactivateInsatiable();
                draining = false;
                fiendMeter = 0f;
            }

            NetworkIdentity networkIdentity = base.gameObject.GetComponent<NetworkIdentity>();
            if (!networkIdentity)
            {
                return;
            }

            new SyncHunger(networkIdentity.netId, (ulong)(this.fiendMeter * 100f)).Send(R2API.Networking.NetworkDestination.Clients);
        }
        //Shit is so scuffed LMAO
        public void ActivateInsatiable()
        {
            inInsatiable = true;
            draining = false;
            insatiableStopwatch = SeamstressStaticValues.instatiableDuation;
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

                TemporaryOverlay temporaryOverlay2 = charModel.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay2.duration = SeamstressStaticValues.instatiableDuation;
                temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0.4f);
                temporaryOverlay2.animateShaderAlpha = true;
                temporaryOverlay2.destroyComponentOnEnd = true;
                temporaryOverlay2.originalMaterial = SeamstressAssets.insatiableOverlayMat;
                temporaryOverlay2.AddToCharacerModel(charModel);
                #endregion
            }
        }
        //If it works it works
        public void DeactivateInsatiable()
        {
            inInsatiable = false;
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
            if (skillLocator.utility.skillDef == SeamstressSurvivor.snapBackSkillDef)
            {
                skillLocator.utility.ExecuteIfReady();
            }
            Instantiate(insatiableEndPrefab, characterBody.modelLocator.transform);
            Util.PlaySound("Play_voidman_transform_return", characterBody.gameObject);
        }
        //end sound
        private void InstatiableSound()
        {
            if (inInsatiable)
            {
                if (insatiableStopwatch < 2f && !hasPlayed)
                {
                    Util.PlaySound("Play_nullifier_impact", characterBody.gameObject);
                    hasPlayed = true;
                }
            }
            hasPlayed = false;
        }
    }
}