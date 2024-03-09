using SeamstressMod.Modules.BaseStates;
using SeamstressMod.Survivors.Seamstress;
using RoR2;
using UnityEngine;
using R2API;
using UnityEngine.Networking;
using TMPro;
using EntityStates;
using static R2API.DamageAPI;


namespace SeamstressMod.SkillStates
{
    public class ParryDash : BaseSeamstressSkillState
    {
        public static GameObject supaPrefab = SeamstressAssets.blinkPrefab;

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
            if (base.cameraTargetParams)
            {
                aimRequest = base.cameraTargetParams.RequestAimType(CameraTargetParams.AimType.Aura);
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
                CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
                //PlayCrossfade("FullBody, Override", "AssaulterLoop", 0.1f);
                gameObject.layer = LayerIndex.fakeActor.intVal;
                base.characterMotor.Motor.RebuildCollidableLayers();
            }
            if (!isDashing)
            {
                stopwatch += Time.fixedDeltaTime;
            }
            else if (base.isAuthority)
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
                    base.characterMotor.rootMotion += dashVector * speedCoefficient * Time.fixedDeltaTime;
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
            if (stopwatch >= dashDuration / attackSpeedStat && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }
        private void CreateBlinkEffect(Vector3 origin)
        {
            if (supaPrefab)
            {
                EffectData effectData = new EffectData();
                effectData.rotation = Util.QuaternionSafeLookRotation(dashVector);
                effectData.origin = origin;
                effectData.scale = 0.1f;
                EffectManager.SpawnEffect(supaPrefab, effectData, transmit: true);
            }
        }
        public override void OnExit()
        {
            gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
            if (!empowered) Util.PlaySound("Play_item_proc_whip", gameObject);
            if (base.isAuthority)
            {
                base.characterMotor.disableAirControlUntilCollision = false;
                base.characterMotor.airControl = 0.25f;
                base.characterMotor.velocity *= 0.3f;
                SmallHop(base.characterMotor, 3f);
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