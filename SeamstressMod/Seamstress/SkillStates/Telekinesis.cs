using EntityStates;
using R2API;
using RoR2;
using SeamstressMod.Modules.BaseStates;
using SeamstressMod.Seamstress.Components;
using SeamstressMod.Seamstress.Content;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements.UIR;
using static RoR2.CharacterMotor;

namespace SeamstressMod.Seamstress.SkillStates
{
    public class Telekinesis : BaseSeamstressSkillState
    {
        public class DetonateOnImpact : MonoBehaviour
        {
            public Telekinesis telekinesis;
            private void OnCollisionEnter(UnityEngine.Collision collision)
            {
                float massCalc = telekinesis.victimRigid.mass / 10f;
                float num = 60f / massCalc;
                float magnitude = collision.relativeVelocity.magnitude;
                if (collision.gameObject.layer == LayerIndex.world.intVal || collision.gameObject.layer == LayerIndex.entityPrecise.intVal || collision.gameObject.layer == LayerIndex.defaultLayer.intVal && magnitude >= num)
                {
                    telekinesis.detonateNextFrame = true;
                }
            }
        }
        public GameObject genericImpactExplosionEffect = SeamstressAssets.impactExplosionEffectDefault;

        public GameObject slamEffect = SeamstressAssets.slamEffect;

        private Transform _barrelPoint;

        private Vector3 _pickTargetPosition;

        private Vector3 _pickOffset;

        private HurtBox victim;

        private Tracker tracker;

        private CharacterMotor victimMotor;

        private RigidbodyMotor victimRigidMotor;

        private Rigidbody victimRigid;

        private CharacterBody victimBody;

        private AnimationCurve pullSuitabilityCurve = new AnimationCurve();

        private CollisionDetectionMode collisionDetectionMode;

        private float _maxGrabDistance = 30f;

        private float _minGrabDistance = 20f;

        private float _pickDistance;

        private float radius = 30f;

        private float damping = 0.1f;

        private float forceMagnitude = -1500;

        private float forceCoefficientAtEdge = 0.25f;

        private float bonusDamage;

        private float hitStopwatch;

        private float previousMass;

        public bool detonateNextFrame;

        private bool bodyCouldTakeImpactDamage;

        private bool theyDidNotHaveRigid;

        private uint playId;

        public override void OnEnter()
        {
            RefreshState();

            base.OnEnter();
            pullSuitabilityCurve.AddKey(0, 1);
            pullSuitabilityCurve.AddKey(2000, 0);
            tracker = GetComponent<Tracker>();
            if (tracker)
            {
                victim = tracker.GetTrackingTarget();
                if (victim)
                {
                    DefaultVictim();
                    if (NetworkServer.active)
                    {
                        if (!victimBody.HasBuff(SeamstressBuffs.Manipulated)) victimBody.AddBuff(SeamstressBuffs.Manipulated);
                    }
                    base.PlayCrossfade("Gesture, Additive", "Manipulate", "Manipulate.playbackRate", (6f / this.attackSpeedStat) * 0.15f, 0.05f);

                    this.playId = Util.PlaySound("sfx_ravager_charge_beam_loop", base.gameObject);
                }
            }
            else
            {
                if(base.isAuthority)
                {
                    outer.SetNextStateToMain();
                }
            }
            if (!_barrelPoint)
            {
                _barrelPoint = transform;
            }
        }

        private void DefaultVictim()
        {
            victimBody = victim.healthComponent.body;
            victimMotor = victimBody.characterMotor;
            victimRigid = victimBody.rigidbody;
            victimRigidMotor = victimBody.gameObject.GetComponent<RigidbodyMotor>();
            _pickDistance = (victim.transform.position - inputBank.aimOrigin).magnitude;
            _pickOffset = victimBody.coreTransform.position - victim.transform.position;
            if (NetworkServer.active && victimBody)
            {
                victimBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            }
            if (victimMotor)
            {
                victimMotor.onMovementHit += DoSplashDamage;
                victimMotor.disableAirControlUntilCollision = true;
            }
            else
            {
                if (!victimRigid)
                {
                    victimRigid = victimBody.gameObject.AddComponent<Rigidbody>();
                    victimRigid.mass = 100f;
                    victimBody.gameObject.AddComponent<SphereCollider>();
                    theyDidNotHaveRigid = true;
                }
                if (victimBody.gameObject.GetComponent<DetonateOnImpact>()) Destroy(victimBody.gameObject.GetComponent<DetonateOnImpact>());
                victimBody.gameObject.AddComponent<DetonateOnImpact>();
                victimBody.gameObject.GetComponent<DetonateOnImpact>().telekinesis = this;
                if (victimRigidMotor)
                {
                    bodyCouldTakeImpactDamage = victimRigidMotor.canTakeImpactDamage;
                    victimRigidMotor.canTakeImpactDamage = false;
                    victimRigidMotor.enabled = false;
                }
            }
            if (victimRigid)
            {
                collisionDetectionMode = victimRigid.collisionDetectionMode;
                victimRigid.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            }
            if (SeamstressConfig.heavyEnemy.Value)
            {
                if (victimMotor)
                {
                    previousMass = victimMotor.mass;
                    victimMotor.mass = Mathf.Clamp(victimMotor.mass, 60f, 120f);
                }
                else if (victimRigid)
                {
                    previousMass = victimRigid.mass;
                    victimRigid.mass = Mathf.Clamp(victimRigid.mass, 60f, 120f);
                }
            }
        }
        public override void OnExit()
        {
            AkSoundEngine.StopPlayingID(this.playId);

            PlayCrossfade("Gesture, Additive", "BufferEmpty", 0.05f);
            if (victimBody)
            {
                if (NetworkServer.active)
                {
                    if (victimBody.HasBuff(SeamstressBuffs.Manipulated))
                    {
                        victimBody.RemoveBuff(SeamstressBuffs.Manipulated);
                        victimBody.AddTimedBuff(SeamstressBuffs.ManipulatedCd, 
                            Mathf.Min(SeamstressConfig.telekinesisCooldown.Value, 
                            Mathf.Max(0.5f, SeamstressConfig.telekinesisCooldown.Value * 
                            characterBody.skillLocator.secondary.cooldownScale - characterBody.skillLocator.secondary.flatCooldownReduction)));
                    }
                }
                if (victimBody.gameObject.GetComponent<DetonateOnImpact>())
                {
                    Destroy(victimBody.gameObject.GetComponent<DetonateOnImpact>());
                }
                if (victimMotor)
                {
                    victimMotor.disableAirControlUntilCollision = true;
                    victimMotor.onMovementHit -= DoSplashDamage;
                }
                if (victimBody.gameObject.GetComponent<DetonateOnImpactThrownTelekinesis>()) Destroy(victimBody.gameObject.GetComponent<DetonateOnImpactThrownTelekinesis>());
                DetonateOnImpactThrownTelekinesis thrown = victimBody.gameObject.AddComponent<DetonateOnImpactThrownTelekinesis>();
                thrown.attacker = gameObject;
                thrown.theyDidNotHaveRigid = theyDidNotHaveRigid;
                thrown.bodyCouldTakeImpactDamage = bodyCouldTakeImpactDamage;
                thrown.coll = collisionDetectionMode;
                thrown.previousMass = previousMass;
                skillLocator.secondary.DeductStock(1);
            }
            base.OnExit();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            StartAimMode();
            hitStopwatch += Time.fixedDeltaTime;

            if (victimRigid != null || victimMotor != null)
            {
                if (victimMotor)
                {
                    victimMotor.disableAirControlUntilCollision = true;
                    if (victimMotor.isGrounded) victimMotor.Motor.ForceUnground();
                }
                if (NetworkServer.active)
                {
                    var ray = inputBank.GetAimRay();
                    _pickTargetPosition = ray.origin + ray.direction * _pickDistance + _pickOffset;
                    var forceDir = victim.transform.position - _pickTargetPosition;
                    float num = 1f - Mathf.Clamp(forceDir.magnitude / radius, 0f, 1f - forceCoefficientAtEdge);
                    forceDir = forceDir.normalized * forceMagnitude * (1f - num);
                    Vector3 vector2 = Vector3.zero;
                    float num2 = 0f;
                    if (victimMotor)
                    {
                        vector2 = victimMotor.velocity;
                        num2 = victimMotor.mass;
                    }
                    else if (victimRigid)
                    {
                        vector2 = victimRigid.velocity;
                        num2 = victimRigid.mass;
                    }
                    else vector2.y += Physics.gravity.y * Time.fixedDeltaTime;
                    if(num2 < 1000f) num2 = Util.Remap(num2, 60f, 1500f, 60f, 125f);
                    float num3 = pullSuitabilityCurve.Evaluate(num2);
                    victim.healthComponent.TakeDamageForce(forceDir - vector2 * damping * (num3 * Mathf.Max(num2, 100f)) * num, alwaysApply: true, disableAirControlUntilCollision: false);
                }

                if (victimMotor != null) bonusDamage = Mathf.Clamp(victimMotor.velocity.magnitude * 
                    (SeamstressConfig.telekinesisDamageCoefficient.Value * damageStat * needleCount) + 
                    victim.healthComponent.fullCombinedHealth * 0.2f, 
                    SeamstressConfig.telekinesisDamageCoefficient.Value * damageStat * needleCount, 
                    victim.healthComponent.fullCombinedHealth * 0.7f);
                else bonusDamage = Mathf.Clamp(victimRigid.velocity.magnitude * 
                    (SeamstressConfig.telekinesisDamageCoefficient.Value * damageStat * needleCount), 
                    SeamstressConfig.telekinesisDamageCoefficient.Value * damageStat, victim.healthComponent.fullHealth * 0.5f);
                if (Util.HasEffectiveAuthority(victimBody.gameObject) && detonateNextFrame)
                {
                    EffectManager.SpawnEffect(this.genericImpactExplosionEffect, new EffectData
                    {
                        origin = victimBody.footPosition,
                        rotation = Quaternion.identity,
                    }, true);
                    EffectManager.SpawnEffect(this.slamEffect, new EffectData
                    {
                        origin = victimBody.footPosition,
                        rotation = Quaternion.identity,
                    }, true);
                    BlastAttack blastAttack = new BlastAttack();
                    blastAttack.position = victimBody.footPosition;
                    blastAttack.baseDamage = bonusDamage;
                    blastAttack.baseForce = 0f;
                    blastAttack.bonusForce = Vector3.zero;
                    blastAttack.radius = 10f;
                    blastAttack.attacker = gameObject;
                    blastAttack.inflictor = gameObject;
                    blastAttack.teamIndex = characterBody.teamComponent.teamIndex;
                    blastAttack.crit = characterBody.RollCrit();
                    blastAttack.procChainMask = default;
                    blastAttack.procCoefficient = 1f;
                    blastAttack.falloffModel = BlastAttack.FalloffModel.Linear;
                    blastAttack.damageColorIndex = DamageColorIndex.Default;
                    blastAttack.damageType = DamageType.Stun1s | DamageType.AOE;
                    if (isInsatiable)
                    {
                        blastAttack.AddModdedDamageType(DamageTypes.CutDamage);
                    }
                    blastAttack.AddModdedDamageType(DamageTypes.SeamstressLifesteal);
                    blastAttack.attackerFiltering = AttackerFiltering.Default;
                    blastAttack.Fire();
                    detonateNextFrame = false;
                    hitStopwatch = 0f;
                }
            }
            if (base.isAuthority && base.inputBank.skill2.justReleased || !victim.healthComponent.alive || base.fixedAge >= 7f)
            {
                outer.SetNextStateToMain();
            }

            _pickDistance = Mathf.Clamp(_pickDistance, _minGrabDistance, _maxGrabDistance);
        }
        private void DoSplashDamage(ref MovementHitInfo movementHitInfo)
        {
            float num = Mathf.Abs(movementHitInfo.velocity.magnitude);
            float num2 = Mathf.Max(num - (victimBody.baseMoveSpeed + 70f), 0f);
            if (num2 > 0 && hitStopwatch > 0.75) detonateNextFrame = true;
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}