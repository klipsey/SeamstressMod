using SeamstressMod.SkillStates.BaseStates;
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
    public class Weave : BaseSeamstressSkillState//WeavePrep
    {
        private Transform modelTransform;

        private float stopwatch;

        private CameraTargetParams.AimRequest aimRequest;

        public static float smallHopVelocity = 12f;

        public static float dashPrepDuration = 0.2f;

        public static float dashDuration = 0.3f;

        public static float speedCoefficient = 100f;

        private Vector3 dashVector = Vector3.zero;

        private OverlapAttack overlapAttack;

        public static float damageCoefficient = SeamstressStaticValues.weaveDamageCoefficient;

        public static float procCoefficient = 1;

        public static GameObject hitEffectPrefab;

        public static float hitPauseDuration = 0.05f;

        private bool isDashing;

        private bool inHitPause;

        private float hitPauseTimer;
        public bool hasHit { get; private set; }

        public int dashIndex { private get; set; }

        private string hitSound;
        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound("Play_imp_overlord_attack2_tell", base.gameObject);
            modelTransform = GetModelTransform();
            if ((bool)base.cameraTargetParams)
            {
                aimRequest = base.cameraTargetParams.RequestAimType(CameraTargetParams.AimType.Aura);
            }
            SmallHop(base.characterMotor, smallHopVelocity);
            //PlayAnimation("FullBody, Override", "AssaulterPrep", "AssaulterPrep.playbackRate", dashPrepDuration);
            dashVector = base.inputBank.aimDirection;
            if (empowered)
            {
                hitEffectPrefab = SeamstressAssets.scissorsButcheredHitImpactEffect;
                //overlapAttack.damageType |= DamageType.BleedOnHit;
                overlapAttack.AddModdedDamageType(DamageTypes.ResetWeave);
                hitSound = "Play_imp_overlord_impact";
            }
            else
            {
                hitEffectPrefab = SeamstressAssets.scissorsHitImpactEffect;
                overlapAttack.AddModdedDamageType(DamageTypes.Empty);
                overlapAttack.RemoveModdedDamageType(DamageTypes.ResetWeave);
                hitSound = "Play_bandit2_m2_impact";
            }
            overlapAttack.AddModdedDamageType(DamageTypes.AddNeedlesKill);
            overlapAttack = InitMeleeOverlap(damageCoefficient, hitEffectPrefab, modelTransform, "Weave");
            overlapAttack.damageType = DamageType.BonusToLowHealth;

        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.characterDirection.forward = dashVector;
            if (stopwatch > dashPrepDuration / attackSpeedStat && !isDashing)
            {
                isDashing = true;
                dashVector = base.inputBank.aimDirection;
                //CreateDashEffect();
                //PlayCrossfade("FullBody, Override", "AssaulterLoop", 0.1f);
                base.gameObject.layer = LayerIndex.fakeActor.intVal;
                base.characterMotor.Motor.RebuildCollidableLayers();
            }
            if (!isDashing)
            {
                stopwatch += Time.fixedDeltaTime;
            }
            else if (base.isAuthority)
            {
                base.characterMotor.velocity = Vector3.zero;
                if (!inHitPause)
                {
                    bool num = overlapAttack.Fire();
                    stopwatch += Time.fixedDeltaTime;
                    if (num)
                    {
                        if (!hasHit)
                        {
                            Util.PlaySound(hitSound, base.gameObject);
                            hasHit = true;
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
            if (stopwatch >= dashDuration + dashPrepDuration / attackSpeedStat && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }
        public override void OnExit()
        {
            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
            Util.PlaySound("Play_imp_overlord_spawn", base.gameObject);
            if (base.isAuthority)
            {
                base.characterMotor.velocity *= 0.1f;
                SmallHop(base.characterMotor, smallHopVelocity);
            }
            aimRequest?.Dispose();
            //PlayAnimation("FullBody, Override", "EvisLoopExit");
            base.OnExit();
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write((byte)dashIndex);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            dashIndex = reader.ReadByte();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
 
    }
}
