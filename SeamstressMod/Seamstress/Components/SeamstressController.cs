using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.EntityLogic;
using UnityEngine.Networking;
using System.Linq;
using SeamstressMod.Seamstress.Content;
using R2API.Networking.Interfaces;
using RoR2.Projectile;
using EntityStates;

namespace SeamstressMod.Seamstress.Components
{
    public class SeamstressController : MonoBehaviour
    {
        private CharacterBody characterBody;
        private ChildLocator childLocator;
        private CharacterMotor characterMotor;
        private HealthComponent healthComponent;
        private SkillLocator skillLocator;
        private Animator animator;
        private ModelSkinController skinController;

        private GameObject insatiableEndPrefab = SeamstressAssets.insatiableEndEffect;

        public GameObject blinkEffect = SeamstressAssets.blinkEffect;

        public GameObject scissorLPrefab = SeamstressAssets.scissorLPrefab;

        public GameObject scissorRPrefab = SeamstressAssets.scissorRPrefab;

        public GameObject trailEffectPrefab;

        public GameObject trailEffectPrefab2;

        public Material destealthMaterial = SeamstressAssets.destealthMaterial;

        public bool blue;

        public float fiendMeter = 0f;

        public float maxHunger;

        private float drainAmount;

        private float dashStopwatch;

        private float checkStatsStopwatch;

        public float blinkCd = 0f;

        public bool blinkReady;

        private bool hasPlayed = false;

        private bool hasPlayedEffect = true;

        public bool isDashing;

        public Vector3 heldDashVector;

        public Vector3 heldOrigin;

        public Vector3 snapBackPosition;

        private float insatiableStopwatch = 0f;

        public bool inInsatiableSkill = false;

        public bool hasStartedInsatiable = false;

        public bool draining;

        public bool hopooHasHopped;

        public void Awake()
        {
            characterBody = GetComponent<CharacterBody>();
            characterMotor = GetComponent<CharacterMotor>();
            healthComponent = GetComponent<HealthComponent>();
            skillLocator = GetComponent<SkillLocator>();
            ModelLocator modelLocator = this.GetComponent<ModelLocator>();
            childLocator = modelLocator.modelBaseTransform.GetComponentInChildren<ChildLocator>();
            animator = modelLocator.modelTransform.GetComponent<Animator>();
            skinController = this.GetComponentInChildren<ModelSkinController>();
            Invoke("SetupSkin", 0.5f);
        }
        public void Start()
        {
            maxHunger = healthComponent.fullHealth * SeamstressStaticValues.maxFiendGaugeCoefficient;
        }

        private void SetupSkin()
        {
            if (childLocator.FindChild("ScissorLModel").gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh && childLocator.FindChild("ScissorRModel").gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh != null)
            {
                if (this.skinController.skins[this.skinController.currentSkinIndex].nameToken == Seamstress.SeamstressSurvivor.SEAMSTRESS_PREFIX + "MASTERY_SKIN_NAME")
                {
                    blue = true;
                    insatiableEndPrefab = SeamstressAssets.insatiableEndEffect2;
                    blinkEffect = SeamstressAssets.blinkEffect2;
                    scissorLPrefab = SeamstressAssets.scissorLPrefab2;
                    scissorRPrefab = SeamstressAssets.scissorRPrefab2;
                    destealthMaterial = SeamstressAssets.destealthMaterial2;
                }
                scissorLPrefab.GetComponent<ProjectileController>().ghostPrefab.GetComponent<MeshFilter>().mesh = childLocator.FindChild("ScissorLModel").gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
                scissorLPrefab.GetComponent<ProjectileController>().ghostPrefab.GetComponent<MeshRenderer>().material = childLocator.FindChild("ScissorRModel").gameObject.GetComponent<SkinnedMeshRenderer>().material;
                scissorRPrefab.GetComponent<ProjectileController>().ghostPrefab.GetComponent<MeshFilter>().mesh = childLocator.FindChild("ScissorRModel").gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
                scissorRPrefab.GetComponent<ProjectileController>().ghostPrefab.GetComponent<MeshRenderer>().material = childLocator.FindChild("ScissorRModel").gameObject.GetComponent<SkinnedMeshRenderer>().material;
            }
        }
        public void FixedUpdate()
        {
            if(this.characterMotor.isGrounded) hopooHasHopped = false;
            blinkCd += Time.fixedDeltaTime;
            if (insatiableStopwatch > 0f) insatiableStopwatch -= Time.fixedDeltaTime;
            if (dashStopwatch > 0 && !hasPlayedEffect) dashStopwatch -= Time.fixedDeltaTime;
            if (checkStatsStopwatch >= 0.5)
            {
                characterBody.MarkAllStatsDirty();
                checkStatsStopwatch = 0f;
            }
            else checkStatsStopwatch += Time.fixedDeltaTime;
            CheckToDrainGauge();
            CreateBlinkEffect(heldOrigin);
            InsatiableSound();
            IsInsatiable();
        }

        public bool HasNeedles()
        {
            return this.characterBody.HasBuff(SeamstressBuffs.needles);
        }
        public void ReactivateScissor(string scissorName, bool activate)
        {
            if (activate == true)
            {
                childLocator.FindChild(scissorName).gameObject.SetActive(true);
            }
            else if (activate == false)
            {
                childLocator.FindChild(scissorName).gameObject.SetActive(false);
            }
        }
        private void CheckToDrainGauge()
        {
            if (draining)
            {
                if (fiendMeter > 0f)
                {
                    fiendMeter -= drainAmount;
                    if (healthComponent.health < healthComponent.fullHealth) healthComponent.Heal(drainAmount / 2, default, false);
                }
                else if (fiendMeter <= 0f)
                {
                    draining = false;
                }
            }
        }
        private void CreateBlinkEffect(Vector3 origin)
        {
            if (blinkEffect && !hasPlayedEffect && dashStopwatch < 0)
            {
                EffectData effectData = new EffectData();
                effectData.rotation = Util.QuaternionSafeLookRotation(heldDashVector);
                effectData.origin = origin;
                effectData.scale = 1f;
                EffectManager.SpawnEffect(blinkEffect, effectData, transmit: false);
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
        public void PlayScissorRSwing()
        {
            PlayCrossfade("Gesture, Override", "ScissorRSlash", "Slash.playbackRate", 1.5f, 0.05f);
        }
        public void PlayScissorLSwing()
        {
            PlayCrossfade("Gesture, Override", "ScissorLSlash", "Slash.playbackRate", 1.5f, 0.05f);
        }
        protected void PlayCrossfade(string layerName, string animationStateName, string playbackRateParam, float duration, float crossfadeDuration)
        {
            if (duration <= 0f)
            {
                Debug.LogWarningFormat("EntityState.PlayCrossfade: Zero duration is not allowed. type={0}", GetType().Name);
                return;
            }
            Animator modelAnimator = this.animator;
            if ((bool)modelAnimator)
            {
                modelAnimator.speed = 1f;
                modelAnimator.Update(0f);
                int layerIndex = modelAnimator.GetLayerIndex(layerName);
                modelAnimator.SetFloat(playbackRateParam, 1f);
                modelAnimator.CrossFadeInFixedTime(animationStateName, crossfadeDuration, layerIndex);
                modelAnimator.Update(0f);
                float length = modelAnimator.GetNextAnimatorStateInfo(layerIndex).length;
                modelAnimator.SetFloat(playbackRateParam, length / duration);
            }
        }
        public void EnableInstatiableLayer()
        {
            if (!this.animator || !this.childLocator) return;

            this.childLocator.FindChild("HeartModel").gameObject.SetActive(false);
            this.animator.SetLayerWeight(this.animator.GetLayerIndex("Body, Butchered"), 1f);
        }
        public void DisableInstatiableLayer() 
        {
            if (!this.animator || !this.childLocator) return;

            this.childLocator.FindChild("HeartModel").gameObject.SetActive(true);
            this.animator.SetLayerWeight(this.animator.GetLayerIndex("Body, Butchered"), 0f);
        }
        private void IsInsatiable()
        {
            if (inInsatiableSkill && !hasStartedInsatiable)
            {
                EnableInstatiableLayer();
                if (trailEffectPrefab) GameObject.Destroy(trailEffectPrefab.gameObject);
                if (trailEffectPrefab2) GameObject.Destroy(trailEffectPrefab2.gameObject);
                Transform handR = this.childLocator.FindChild("HandR");
                Transform handL = this.childLocator.FindChild("HandL");
                trailEffectPrefab = GameObject.Instantiate(this.blue ? SeamstressAssets.trailEffectHands2 : SeamstressAssets.trailEffectHands, handR);
                trailEffectPrefab2 = GameObject.Instantiate(this.blue ? SeamstressAssets.trailEffectHands2 : SeamstressAssets.trailEffectHands, handL);

                draining = false;
                insatiableStopwatch = SeamstressStaticValues.insatiableDuration;
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
            else if (!inInsatiableSkill && hasStartedInsatiable)
            {
                DisableInstatiableLayer();
                if (trailEffectPrefab) GameObject.Destroy(trailEffectPrefab.gameObject);
                if (trailEffectPrefab2) GameObject.Destroy(trailEffectPrefab2.gameObject);

                drainAmount = healthComponent.fullHealth / 125;
                draining = true;
                Transform modelTransform = characterBody.modelLocator.modelTransform;
                if (modelTransform)
                {
                    TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = 1f;
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = this.destealthMaterial;
                    temporaryOverlay.inspectorCharacterModel = modelTransform.GetComponent<CharacterModel>();
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay.animateShaderAlpha = true;
                }
                Instantiate(insatiableEndPrefab, characterBody.modelLocator.transform);
                Util.PlaySound("Play_voidman_transform_return", characterBody.gameObject);
                hasStartedInsatiable = false;
            }
            if(!inInsatiableSkill && !hasStartedInsatiable && skillLocator.utility.skillDef == SeamstressSurvivor.snapBackSkillDef)
            {
                skillLocator.utility.ExecuteIfReady();
            }
        }
        //end sound
        private void InsatiableSound()
        {
            if (inInsatiableSkill)
            {
                if (insatiableStopwatch < 2f && !hasPlayed)
                {
                    Util.PlaySound("Play_nullifier_impact", characterBody.gameObject);
                    hasPlayed = true;
                }
                else if(insatiableStopwatch > 2f || insatiableStopwatch < 0f)
                {
                    hasPlayed = false;
                }
            }
        }
    }
}