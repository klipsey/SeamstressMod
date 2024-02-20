﻿using System.Linq;
using RoR2;
using RoR2.Navigation;
using UnityEngine;
using SeamstressMod.Modules.BaseStates;
using SeamstressMod.Survivors.Seamstress;
using R2API;
using UnityEngine.Networking;
using EntityStates;
using UnityEngine.Networking.Match;

namespace SeamstressMod.SkillStates
{
    public class BlinkSeamstress : BaseSeamstressSkillState
    {

        private Transform modelTransform;

        public static GameObject blinkPrefab;

        private CameraTargetParams.AimRequest request;

        private float healthCostFraction = 0.5f;

        private Vector3 blinkVector = Vector3.zero;

        public float duration = 0.3f;

        public float speedCoefficient = 30f;

        public static string beginSoundString = "Play_imp_overlord_teleport_start";

        public static string endSoundString = "Play_imp_overlord_teleport_end";

        //test sphere collider changing with scale
        public static float blastAttackRadius = SeamstressAssets.blinkDestinationPrefab.transform.GetChild(1).gameObject.GetComponent<SphereCollider>().radius - 10;

        public static float blastAttackDamageCoefficient = SeamstressStaticValues.blinkDamageCoefficient;

        public static float blastAttackForce = 25f;

        public static float blastAttackProcCoefficient = 1f;

        private CharacterModel characterModel;

        private HurtBoxGroup hurtboxGroup;

        private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(beginSoundString, base.gameObject);
            modelTransform = GetModelTransform();
            if (modelTransform)
            {
                characterModel = modelTransform.GetComponent<CharacterModel>();
                hurtboxGroup = modelTransform.GetComponent<HurtBoxGroup>();
                animator = modelTransform.GetComponent<Animator>();
            }
            if (characterModel)
            {
                characterModel.invisibilityCount++;
            }
            if (hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }
            if (base.cameraTargetParams)
            {
                request = base.cameraTargetParams.RequestAimType(CameraTargetParams.AimType.Aura);
            }
            blinkVector = GetBlinkVector();
            if (blinkVector.sqrMagnitude < Mathf.Epsilon)
            {
                blinkVector = base.inputBank.aimDirection;
            }
            CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
        }

        protected virtual Vector3 GetBlinkVector()
        {
            Vector3 aimDirection = base.inputBank.aimDirection;
            aimDirection.y = 0f;
            Vector3 axis = -Vector3.Cross(Vector3.up, aimDirection);
            float num = Vector3.Angle(base.inputBank.aimDirection, aimDirection);
            if (base.inputBank.aimDirection.y < 0f)
            {
                num = 0f - num;
            }
            return Vector3.Normalize(Quaternion.AngleAxis(num, axis) * base.inputBank.moveVector);
        }
        private void CreateBlinkEffect(Vector3 origin)
        {
            if ((bool)SeamstressAssets.blinkPrefab)
            {
                EffectData effectData = new EffectData();
                effectData.rotation = Util.QuaternionSafeLookRotation(blinkVector);
                effectData.origin = origin;
                effectData.scale = 0.15f;
                EffectManager.SpawnEffect(SeamstressAssets.blinkPrefab, effectData, transmit: true);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if ((bool)base.characterMotor && (bool)base.characterDirection)
            {
                base.characterMotor.rootMotion += blinkVector * (speedCoefficient * Time.fixedDeltaTime);
            }
            if (base.fixedAge >= duration && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }
        public override void OnExit()
        {
            if (!outer.destroying)
            {
                if (NetworkServer.active && healthComponent && healthCostFraction >= Mathf.Epsilon)
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
                    SeamstressController s = characterBody.GetComponent<SeamstressController>();
                    s.fuckYou = false;
                    characterBody.AddTimedBuff(SeamstressBuffs.butchered, SeamstressStaticValues.butcheredDuration, 1);
                }
                this.skillLocator.special = skillLocator.FindSkill("reapRecast");

                Util.PlaySound(endSoundString, base.gameObject);
                CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
                modelTransform = GetModelTransform();
                if (this.modelTransform && SeamstressAssets.destealthMaterial)
                {
                    TemporaryOverlay temporaryOverlay = animator.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = 1f;
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = SeamstressAssets.destealthMaterial;
                    temporaryOverlay.inspectorCharacterModel = animator.gameObject.GetComponent<CharacterModel>();
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay.animateShaderAlpha = true;
                }
                if (blastAttackDamageCoefficient > 0f && base.isAuthority)
                {
                    BlastAttack blastAttack = new BlastAttack();
                    blastAttack.attacker = base.gameObject;
                    blastAttack.inflictor = base.gameObject;
                    blastAttack.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
                    blastAttack.baseDamage = damageStat * blastAttackDamageCoefficient;
                    blastAttack.baseForce = blastAttackForce;
                    blastAttack.position = base.transform.position;
                    blastAttack.procCoefficient = blastAttackProcCoefficient;
                    blastAttack.radius = blastAttackRadius;
                    blastAttack.damageType = DamageType.Stun1s;
                    blastAttack.AddModdedDamageType(DamageTypes.StitchDamage);
                    if(empowered)
                    {
                        blastAttack.AddModdedDamageType(DamageTypes.CutDamage);
                    }
                    blastAttack.falloffModel = BlastAttack.FalloffModel.Linear;
                    blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;
                    blastAttack.Fire();
                }
                if (characterModel)
                {
                    characterModel.invisibilityCount--;
                }
                if (hurtboxGroup)
                {
                    HurtBoxGroup hurtBoxGroup = hurtboxGroup;
                    int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
                    hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
                }
                if (base.characterMotor)
                {
                    base.characterMotor.disableAirControlUntilCollision = false;
                }
                if(base.cameraTargetParams)
                {
                    request.Dispose();
                }
                if(base.isAuthority) 
                {
                    base.characterMotor.velocity *= 0.3f;
                    SmallHop(base.characterMotor, 3f);
                }
            }
            base.OnExit();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(blinkVector);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            blinkVector = reader.ReadVector3();
        }
    }
}