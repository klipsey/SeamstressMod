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
    public class Weave : BaseSeamstressSkillState//WeavePrep
    {
        private Transform modelTransform;

        public GameObject dashPrefab;

        private float stopwatch;

        private CameraTargetParams.AimRequest aimRequest;

        public static float smallHopVelocity = 10f;

        public static float dashPrepDuration = 0.2f;

        public float dashDuration;

        public static float speedCoefficient = 100f;

        private Vector3 dashVector;

        private OverlapAttack overlapAttack;

        public static float damageCoefficient = SeamstressStaticValues.weaveDamageCoefficient;

        public static float procCoefficient = 1;

        public static GameObject hitEffectPrefab;

        private ChildLocator childLocator;

        public static float hitPauseDuration = 0.075f;

        private bool isDashing;

        private bool inHitPause;

        private float hitPauseTimer;
        public bool hasHit { get; private set; }

        private string hitSound;
        public override void OnEnter()
        {
            dashDuration = 0.3f;
            base.OnEnter();
            RefreshState();
            modelTransform = GetModelTransform();
            childLocator = modelTransform.GetComponent<ChildLocator>();
            if ((bool)base.cameraTargetParams)
            {
                aimRequest = base.cameraTargetParams.RequestAimType(CameraTargetParams.AimType.Aura);
            }
            //PlayAnimation("FullBody, Override", "AssaulterPrep", "AssaulterPrep.playbackRate", dashPrepDuration);
            dashVector = inputBank.aimDirection;

            overlapAttack = InitMeleeOverlap(damageCoefficient, hitEffectPrefab, modelTransform, "Weave");
            overlapAttack.AddModdedDamageType(DamageTypes.CutDamage);
            if (empowered)
            {
                hitEffectPrefab = SeamstressAssets.scissorsButcheredHitImpactEffect;
                dashPrefab = SeamstressAssets.weaveDashButchered;
                overlapAttack.AddModdedDamageType(DamageTypes.WeaveLifeSteal);
                Util.PlaySound("Play_imp_overlord_attack2_tell", gameObject);
                hitSound = "Play_imp_overlord_impact";
            }
            else
            {
                hitEffectPrefab = SeamstressAssets.scissorsHitImpactEffect;
                Util.PlaySound("Play_merc_m2_uppercut", gameObject);
                dashPrefab = SeamstressAssets.weaveDash;
                overlapAttack.RemoveModdedDamageType(DamageTypes.WeaveLifeSteal);
                hitSound = "Play_bandit2_m2_impact";
            }
            PlayAnimation("FullBody, Override", "Roll", "Roll.playbackRate", dashDuration + dashPrepDuration);
            SmallHop(base.characterMotor, smallHopVelocity);
        }
        public void CreateDashEffect()
        {
            Transform transform = childLocator.FindChild("CharacterCenter");
            if ((bool)transform && (bool)dashPrefab)
            {
                Object.Instantiate<GameObject>(dashPrefab, transform.position, Util.QuaternionSafeLookRotation(dashVector), transform);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            characterDirection.forward = dashVector;
            if (stopwatch > dashPrepDuration / attackSpeedStat && !isDashing)
            {
                isDashing = true;
                dashVector = inputBank.aimDirection;
                CreateDashEffect();
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
            if (stopwatch >= dashDuration + dashPrepDuration / attackSpeedStat && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }
        public override void OnExit()
        {
            gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
            if (!empowered)Util.PlaySound("Play_item_proc_whip", gameObject);
            if (base.isAuthority)
            {
                base.characterMotor.disableAirControlUntilCollision = false;
                base.characterMotor.airControl = 0.25f;
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
