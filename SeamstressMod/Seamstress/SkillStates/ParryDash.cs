﻿using SeamstressMod.Modules.BaseStates;
using RoR2;
using UnityEngine;
using R2API;
using UnityEngine.Networking;
using TMPro;
using EntityStates;
using static R2API.DamageAPI;
using SeamstressMod.Seamstress.Content;


namespace SeamstressMod.Seamstress.SkillStates
{
    public class ParryDash : BaseSeamstressSkillState
    {
        public static GameObject hitEffectPrefab = SeamstressAssets.scissorsHitImpactEffect;
        public bool hasHit { get; private set; }

        public static float speedCoefficient = 100f;

        public static float damageCoefficient = SeamstressStaticValues.parryDamageCoefficient;

        public static float hitPauseDuration = 0.075f;

        private Vector3 dashVector;

        private OverlapAttack overlapAttack;

        private ChildLocator childLocator;

        private Transform modelTransform;

        private CameraTargetParams.AimRequest aimRequest;

        private float stopwatch;

        private float dashDuration;

        private bool isDashing;

        private bool inHitPause;

        private float hitPauseTimer;

        private bool noScissors;

        private string hitSound;

        private string hitBox;


        public override void OnEnter()
        {
            inDash = true;
            dashDuration = 0.3f;
            base.OnEnter();
            RefreshState();
            modelTransform = GetModelTransform();
            childLocator = modelTransform.GetComponent<ChildLocator>();
            if (cameraTargetParams)
            {
                aimRequest = cameraTargetParams.RequestAimType(CameraTargetParams.AimType.Aura);
            }
            //PlayAnimation("FullBody, Override", "AssaulterPrep", "AssaulterPrep.playbackRate", dashPrepDuration);
            dashVector = inputBank.aimDirection;

            if (!scissorRight && !scissorLeft) noScissors = true;
            if (noScissors) hitBox = "Weave";
            else hitBox = "WeaveBig";
            overlapAttack = InitMeleeOverlap(0, hitEffectPrefab, modelTransform, hitBox);
            overlapAttack.AddModdedDamageType(DamageTypes.PullDamage);
            overlapAttack.damageType = DamageType.Stun1s;
            Util.PlaySound("Play_imp_overlord_attack2_tell", gameObject);
            hitSound = "Play_imp_overlord_impact";
            PlayAnimation("FullBody, Override", "Roll", "Roll.playbackRate", dashDuration);
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            characterDirection.forward = dashVector;
            if (!isDashing)
            {
                isDashing = true;
                dashVector = inputBank.aimDirection;
                seamCom.heldDashVector = dashVector;
                seamCom.heldOrigin = Util.GetCorePosition(gameObject);
                //PlayCrossfade("FullBody, Override", "AssaulterLoop", 0.1f);
                gameObject.layer = LayerIndex.fakeActor.intVal;
                characterMotor.Motor.RebuildCollidableLayers();
            }
            if (!isDashing)
            {
                stopwatch += Time.fixedDeltaTime;
            }
            else if (isAuthority)
            {
                if (!inHitPause)
                {
                    bool num = overlapAttack.Fire();
                    stopwatch += Time.fixedDeltaTime;
                    if (num)
                    {
                        if (!hasHit)
                        {
                            Util.PlaySound(hitSound, gameObject);
                            hasHit = true;
                            dashDuration *= 0.75f;
                        }
                        inHitPause = true;
                        hitPauseTimer = hitPauseDuration / attackSpeedStat;
                    }
                    characterMotor.rootMotion += dashVector * speedCoefficient * Time.fixedDeltaTime;
                }
                else
                {
                    hitPauseTimer -= Time.fixedDeltaTime;
                    if (hitPauseTimer < 0f)
                    {
                        inHitPause = false;
                    }
                }
            }
            if (stopwatch >= dashDuration / attackSpeedStat && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }
        public override void OnExit()
        {
            seamCom.StartDashEffectTimer();
            gameObject.layer = LayerIndex.defaultLayer.intVal;
            characterMotor.Motor.RebuildCollidableLayers();
            if (!insatiable) Util.PlaySound("Play_item_proc_whip", gameObject);
            if (isAuthority)
            {
                characterMotor.disableAirControlUntilCollision = false;
                characterMotor.airControl = 0.25f;
                characterMotor.velocity *= 0.3f;
                SmallHop(characterMotor, 3f);
            }
            aimRequest?.Dispose();
            inDash = false;
            RefreshState();
            //PlayAnimation("FullBody, Override", "EvisLoopExit");
            base.OnExit();
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(dashVector);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            dashVector = reader.ReadVector3();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}