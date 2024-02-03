﻿using System.Linq;
using RoR2;
using RoR2.Navigation;
using UnityEngine;
using SeamstressMod.Modules.BaseStates;
using SeamstressMod.Survivors.Seamstress;
using R2API;
using UnityEngine.Networking;
using EntityStates;

namespace SeamstressMod.SkillStates
{
    public class BlinkSeamstress : BaseSeamstressSkillState
    {
        public static bool disappearWhileBlinking = true;

        public CameraTargetParams.AimRequest aimRequest;

        private Vector3 blinkDestination = Vector3.zero;

        private Vector3 blinkStart = Vector3.zero;

        public static float baseDuration = 0.5f;

        public float exitDuration = 0.15f;

        public float destinationAlertDuration;

        public static float blinkDistance = 30f;

        public static string beginSoundString = "Play_imp_overlord_teleport_start";

        public static string endSoundString = "Play_imp_overlord_teleport_end";

        //test sphere collider changing with scale
        public static float blastAttackRadius = SeamstressAssets.blinkDestinationPrefab.transform.GetChild(1).gameObject.GetComponent<SphereCollider>().radius - 15;

        public static float empoweredBlastAttackRadius = SeamstressAssets.blinkDestinationPrefab.transform.GetChild(1).gameObject.GetComponent<SphereCollider>().radius - 5;

        public static float blastAttackDamageCoefficient = SeamstressStaticValues.blinkDamageCoefficient;

        public static float blastAttackForce = 50f;

        public static float blastAttackProcCoefficient = 1f;

        public static float healthCostFraction = SeamstressStaticValues.reapHealthCost / 2;

        private Animator animator;

        private CharacterModel characterModel;

        private HurtBoxGroup hurtboxGroup;

        private ChildLocator childLocator;

        private Transform modelTransform;

        private GameObject blinkDestinationInstance;

        private bool isExiting = false;

        private bool hasBlinked = false;

        public override void OnEnter()
        {
            this.exitDuration = (baseDuration / 2) / this.attackSpeedStat;
            this.destinationAlertDuration = (baseDuration / 2) / this.attackSpeedStat;
            base.OnEnter();
            RefreshState();
            if ((bool)base.cameraTargetParams)
            {
                aimRequest = base.cameraTargetParams.RequestAimType(CameraTargetParams.AimType.Aura);
            }
            Util.PlaySound(beginSoundString, base.gameObject);
            this.modelTransform = GetModelTransform();
            if ((bool)this.modelTransform)
            {
                this.animator = this.modelTransform.GetComponent<Animator>();
                this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
                this.hurtboxGroup = this.modelTransform.GetComponent<HurtBoxGroup>();
                this.childLocator = this.modelTransform.GetComponent<ChildLocator>();
            }
            if (disappearWhileBlinking)
            {
                if ((bool)this.characterModel)
                {
                    this.characterModel.invisibilityCount++;
                }
                if ((bool)this.hurtboxGroup)
                {
                    HurtBoxGroup hurtBoxGroup = hurtboxGroup;
                    int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
                    hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
                }
                /*
                if ((bool)this.childLocator)
                {
                    this.childLocator.FindChild("DustCenter").gameObject.SetActive(value: false);
                }
                */
            }
            CalculateBlinkDestination();
            CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
            PlayAnimation("FullBody, Override", "Roll", "Roll.playbackRate", baseDuration / this.attackSpeedStat + exitDuration);
        }

        private void CalculateBlinkDestination()
        {
            if(base.isAuthority)
            {
                RaycastHit hitInfo;
                Vector3 position = (!base.inputBank.GetAimRaycast(blinkDistance, out hitInfo) ? Vector3.MoveTowards(base.inputBank.GetAimRay().GetPoint(blinkDistance), base.transform.position, 5f) : Vector3.MoveTowards(hitInfo.point, base.transform.position, 5f));
                position.y += 2.5f;
                blinkDestination = position;
                Vector3 vector = new Vector3(0f, 0.1f, 0f);
                blinkDestination -= (base.characterBody.footPosition - base.transform.position + vector);
                blinkStart = base.transform.position;
                base.characterDirection.forward = position;
            }
        }
        private void CreateBlinkEffect(Vector3 origin)
        {
            if ((bool)SeamstressAssets.blinkPrefab)
            {
                EffectData effectData = new EffectData();
                effectData.rotation = Util.QuaternionSafeLookRotation(blinkDestination - blinkStart);
                effectData.origin = origin;
                effectData.scale = 0.2f;
                EffectManager.SpawnEffect(SeamstressAssets.blinkPrefab, effectData, transmit: true);
            }
        }

        private void SetPosition(Vector3 newPosition)
        {
            if ((bool)base.characterMotor)
            {
                base.characterMotor.Motor.SetPositionAndRotation(newPosition, Quaternion.identity);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if ((bool)base.characterMotor)
            {
                base.characterMotor.velocity = Vector3.zero;
            }
            if (!hasBlinked)
            {
                SetPosition(Vector3.Lerp(blinkStart, blinkDestination, base.fixedAge / baseDuration));
            }
            if (base.fixedAge >= baseDuration / this.attackSpeedStat - destinationAlertDuration && !hasBlinked)
            {
                hasBlinked = true;
                if ((bool)SeamstressAssets.blinkDestinationPrefab)
                {
                    blinkDestinationInstance = Object.Instantiate(SeamstressAssets.blinkDestinationPrefab, blinkDestination, Quaternion.identity);
                    blinkDestinationInstance.GetComponent<ScaleParticleSystemDuration>().newDuration = destinationAlertDuration;
                    if(empowered)
                    {
                        Object.Instantiate(SeamstressAssets.expungeEffect, blinkDestination, Quaternion.identity);
                    }
                }
                SetPosition(blinkDestination);
            }
            if (base.fixedAge >= baseDuration / this.attackSpeedStat)
            {
                ExitCleanup();
            }
            if (base.fixedAge >= baseDuration / this.attackSpeedStat + exitDuration && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        private void ExitCleanup()
        {
            if (isExiting)
            {
                return;
            }
            if (NetworkServer.active && (bool)healthComponent && healthCostFraction >= Mathf.Epsilon)
            {
                float currentBarrier = healthComponent.barrier;
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.damage = ((healthComponent.health + healthComponent.shield) * healthCostFraction) + healthComponent.barrier;
                damageInfo.position = characterBody.corePosition;
                damageInfo.force = Vector3.zero;
                damageInfo.damageColorIndex = DamageColorIndex.Default;
                damageInfo.crit = false;
                damageInfo.attacker = null;
                damageInfo.inflictor = null;
                damageInfo.damageType = DamageType.NonLethal | DamageType.BypassArmor | DamageType.BypassBlock;
                damageInfo.procCoefficient = 0f;
                healthComponent.TakeDamage(damageInfo);
                healthComponent.AddBarrier(currentBarrier);
            }
                isExiting = true;

            Util.PlaySound(endSoundString, base.gameObject);
            CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
            modelTransform = GetModelTransform();
            if (blastAttackDamageCoefficient > 0f)
            {
                BlastAttack blastAttack = new BlastAttack();
                blastAttack.attacker = base.gameObject;
                blastAttack.inflictor = base.gameObject;
                blastAttack.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
                blastAttack.baseDamage = damageStat * blastAttackDamageCoefficient;
                blastAttack.baseForce = blastAttackForce;
                blastAttack.position = blinkDestination;
                blastAttack.procCoefficient = blastAttackProcCoefficient;
                if (empowered)
                {
                    blastAttack.radius = empoweredBlastAttackRadius;
                    blastAttack.damageType = DamageType.Stun1s;
                }
                else
                {
                    blastAttack.radius = blastAttackRadius;
                }
                blastAttack.falloffModel = BlastAttack.FalloffModel.Linear;
                blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;
                blastAttack.AddModdedDamageType(DamageTypes.CutDamage);
                blastAttack.Fire();
            }
            if (disappearWhileBlinking)
            {
                if ((bool)this.modelTransform && (bool)SeamstressAssets.destealthMaterial)
                {
                    TemporaryOverlay temporaryOverlay = animator.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = 1f;
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = SeamstressAssets.destealthMaterial;
                    temporaryOverlay.inspectorCharacterModel = animator.gameObject.GetComponent<CharacterModel>();
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay.animateShaderAlpha = true;
                }
                if ((bool)this.characterModel)
                {
                    characterModel.invisibilityCount--;
                }
                if ((bool)this.hurtboxGroup)
                {
                    HurtBoxGroup hurtBoxGroup = hurtboxGroup;
                    int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
                    hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
                }
                /*
                if ((bool)this.childLocator)
                {
                    childLocator.FindChild("DustCenter").gameObject.SetActive(value: true);
                }
                */
                //PlayAnimation("Gesture, Additive", "BlinkEnd", "BlinkEnd.playbackRate", exitDuration);
            }
            if ((bool)blinkDestinationInstance)
            {
                UnityEngine.Object.Destroy(blinkDestinationInstance);
            }
        }

        public override void OnExit()
        {
            aimRequest?.Dispose();
            base.OnExit();
            ExitCleanup();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(blinkDestination);
            writer.Write(blinkStart);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            blinkDestination = reader.ReadVector3();
            blinkStart = reader.ReadVector3();
        }
    }
}