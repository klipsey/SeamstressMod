using SeamstressMod.Modules.BaseStates;
using SeamstressMod.Survivors.Seamstress;
using RoR2;
using UnityEngine;
using R2API;
using UnityEngine.Networking;
using TMPro;
using EntityStates;
using static R2API.DamageAPI;
using static UnityEngine.UI.Image;

namespace SeamstressMod.SkillStates
{
    public class WeaveLeap : BaseSeamstressSkillState
    {
        public static GameObject dashPrefab = SeamstressAssets.weaveDashButchered;

        public static GameObject supaPrefab = SeamstressAssets.sewButcheredEffect;

        protected GameObject hitEffectPrefab;
        public bool hasHit { get; private set; }

        public static float dashPrepDuration = 0.4f;

        public static float speedCoefficient = 100f;

        public static float damageCoefficient = SeamstressStaticValues.weaveLeapDamageCoefficient;

        public static float procCoefficient = 1;

        public static float hitPauseDuration = 0.075f;

        private Vector3 direction;

        public static float airControl = 0.15f;

        public static float aimVelocity = 5;

        public static float upwardVelocity = 7;

        public static float forwardVelocity = 4;

        public static float minimumY = 0.1f;

        private OverlapAttack overlapAttack;

        private ChildLocator childLocator;

        private Transform modelTransform;

        private CameraTargetParams.AimRequest aimRequest;

        private float stopwatch;

        private float dashDuration;

        private float previousAirControl;

        private bool isDashing;

        private bool inHitPause;

        private float hitPauseTimer;

        private string hitSound;
        public override void OnEnter()
        {
            base.OnEnter();
            direction = GetAimRay().direction;
            dashDuration = 0.5f;
            previousAirControl = base.characterMotor.airControl;
            base.characterMotor.airControl = airControl;
            RefreshState();
            modelTransform = GetModelTransform();
            childLocator = modelTransform.GetComponent<ChildLocator>();
            if ((bool)base.cameraTargetParams)
            {
                aimRequest = base.cameraTargetParams.RequestAimType(CameraTargetParams.AimType.Aura);
            }
            //PlayAnimation("FullBody, Override", "AssaulterPrep", "AssaulterPrep.playbackRate", dashPrepDuration);
            overlapAttack = InitMeleeOverlap(damageCoefficient, hitEffectPrefab, modelTransform, "Weave");
            overlapAttack.AddModdedDamageType(DamageTypes.StitchDamage);
            overlapAttack.procCoefficient = procCoefficient;
            if (empowered)
            {
                hitEffectPrefab = SeamstressAssets.scissorsButcheredHitImpactEffect;
                overlapAttack.AddModdedDamageType(DamageTypes.CutDamage);
                Util.PlaySound("Play_imp_overlord_attack2_tell", gameObject);
                hitSound = "Play_imp_overlord_impact";
            }
            else
            {
                hitEffectPrefab = SeamstressAssets.scissorsHitImpactEffect;
                overlapAttack.RemoveModdedDamageType(DamageTypes.CutDamage);
                Util.PlaySound("Play_merc_m2_uppercut", gameObject);
                hitSound = "Play_bandit2_m2_impact";
            }
            SmallHop(base.characterMotor, 5f);
            PlayAnimation("FullBody, Override", "Roll", "Roll.playbackRate", dashDuration + dashPrepDuration);
            base.skillLocator.special.SetSkillOverride(gameObject, SeamstressAssets.weaveRecastSkillDef, GenericSkill.SkillOverridePriority.Contextual);
        }
        public void CreateDashEffect()
        {
            Transform transform = childLocator.FindChild("CharacterCenter");
            if ((bool)transform && (bool)dashPrefab)
            {
                if (empowered && (bool)supaPrefab)
                {
                    Object.Instantiate<GameObject>(supaPrefab, Util.GetCorePosition(base.gameObject), Util.QuaternionSafeLookRotation(direction));
                }
                Object.Instantiate<GameObject>(dashPrefab, transform.position, Util.QuaternionSafeLookRotation(direction), transform);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            characterDirection.forward = direction;
            if (stopwatch > dashPrepDuration / attackSpeedStat && !isDashing)
            {
                isDashing = true;
                direction = GetAimRay().direction;
                CreateDashEffect();
                //PlayCrossfade("FullBody, Override", "AssaulterLoop", 0.1f);
                base.characterBody.isSprinting = true;
                direction.y = Mathf.Max(direction.y, minimumY);
                Vector3 vector = direction.normalized * aimVelocity * 3 * (moveSpeedStat * 0.25f);
                Vector3 vector2 = Vector3.up * upwardVelocity;
                Vector3 vector3 = new Vector3(direction.x, 0f, direction.z).normalized * forwardVelocity;
                base.characterMotor.Motor.ForceUnground();
                base.characterMotor.velocity = vector + vector2 + vector3;
                base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
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
                        }
                        inHitPause = true;
                        hitPauseTimer = hitPauseDuration / attackSpeedStat;
                    }
                    if((bool)base.characterMotor)
                    {
                        base.characterMotor.moveDirection = base.inputBank.moveVector;
                    }
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
            if ((stopwatch >= dashDuration + dashPrepDuration / attackSpeedStat) && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }
        public override void OnExit()
        {
            gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            base.characterMotor.airControl = previousAirControl;
            base.characterBody.isSprinting = false;
            if (!empowered) Util.PlaySound("Play_item_proc_whip", gameObject);
            if (base.isAuthority)
            {
                base.characterMotor.disableAirControlUntilCollision = false;
                SmallHop(base.characterMotor, 5f);
            }
            aimRequest?.Dispose();
            //PlayAnimation("FullBody, Override", "EvisLoopExit");
            base.OnExit();
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(direction);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            direction = reader.ReadVector3();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}

