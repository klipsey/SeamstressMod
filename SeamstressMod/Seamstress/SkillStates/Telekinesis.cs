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
using static UnityEngine.ParticleSystem.PlaybackState;


namespace SeamstressMod.Seamstress.SkillStates
{
    public class Telekinesis : BaseSeamstressSkillState
    {
        public class DetonateOnImpact : MonoBehaviour
        {
            public Telekinesis telekinesis;
            private void OnCollisionEnter(UnityEngine.Collision collision)
            {
                float massCalc = (telekinesis.victimMotor ? telekinesis.victimMotor.mass : telekinesis.victimRigid.mass) / 10f;
                float num = 60f / massCalc;
                float magnitude = collision.relativeVelocity.magnitude;
                if (collision.gameObject.layer == LayerIndex.world.intVal || collision.gameObject.layer == LayerIndex.entityPrecise.intVal || collision.gameObject.layer == LayerIndex.defaultLayer.intVal && magnitude >= num)
                {
                    telekinesis.detonateNextFrame = true;
                }
            }
        }
        private Transform _barrelPoint;

        private Vector3 _pickTargetPosition;

        private Vector3 _pickOffset;

        private HurtBox victim;

        private Tracker tracker;

        private CharacterMotor victimMotor;

        private RigidbodyMotor victimRigidMotor;

        private Rigidbody victimRigid;

        private Rigidbody tempRigidbody;

        private SphereCollider tempSphereCollider;

        private CharacterBody victimBody;

        private AnimationCurve pullSuitabilityCurve = new AnimationCurve();

        private CollisionDetectionMode collisionDetectionMode;

        private float _maxGrabDistance = 40f;

        private float _minGrabDistance = 1f;

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

        public override void OnEnter()
        {
            base.OnEnter();
            pullSuitabilityCurve.AddKey(0, 1);
            pullSuitabilityCurve.AddKey(2000, 0);
            tracker = GetComponent<Tracker>();
            if (tracker)
            {
                if (isAuthority)
                {
                    victim = tracker.GetTrackingTarget();
                }
                DefaultVictim();
                if (NetworkServer.active)
                {
                    if (!victimBody.HasBuff(SeamstressBuffs.manipulated)) victimBody.AddBuff(SeamstressBuffs.manipulated);
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
                    tempRigidbody = victimRigid;
                    tempSphereCollider = victimBody.gameObject.AddComponent<SphereCollider>();
                    theyDidNotHaveRigid = true;
                }
                if (victimBody.gameObject.GetComponent<DetonateOnImpact>() != null) Object.Destroy(victimBody.gameObject.GetComponent<DetonateOnImpact>());
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
            if (SeamstressConfig.funny.Value)
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
            if (victimBody)
            {
                if (NetworkServer.active)
                {
                    if (victimBody.HasBuff(SeamstressBuffs.manipulated))
                    {
                        victimBody.RemoveBuff(SeamstressBuffs.manipulated);
                        victimBody.AddTimedBuff(SeamstressBuffs.manipulatedCd, Mathf.Min(SeamstressStaticValues.telekinesisCooldown, Mathf.Max(0.5f, SeamstressStaticValues.telekinesisCooldown * characterBody.skillLocator.secondary.cooldownScale - characterBody.skillLocator.secondary.flatCooldownReduction)));
                    }
                }
                if (victimBody.gameObject.GetComponent<DetonateOnImpact>() != null)
                {
                    Object.Destroy(victimBody.gameObject.GetComponent<DetonateOnImpact>());
                }
                if (victimMotor)
                {
                    victimMotor.disableAirControlUntilCollision = true;
                    victimMotor.onMovementHit -= DoSplashDamage;
                }
                if (victimBody.gameObject.GetComponent<DetonateOnImpactThrown>() != null) Object.Destroy(victimBody.gameObject.GetComponent<DetonateOnImpactThrown>());
                DetonateOnImpactThrown thrown = victimBody.gameObject.AddComponent<DetonateOnImpactThrown>();
                thrown.attacker = gameObject;
                thrown.theyDidNotHaveRigid = theyDidNotHaveRigid;
                thrown.bodyCouldTakeImpactDamage = bodyCouldTakeImpactDamage;
                thrown.coll = collisionDetectionMode;
                thrown.previousMass = previousMass;
                skillLocator.secondary.DeductStock(1);
            }
            base.OnExit();
        }
        public override void Update()
        {
            base.Update();
            if (inputBank.skill2.justReleased || !victim.healthComponent.alive || fixedAge >= 7f)
            {
                outer.SetNextStateToMain();
            }

            _pickDistance = Mathf.Clamp(_pickDistance + Input.mouseScrollDelta.y, _minGrabDistance, _maxGrabDistance);
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            StartAimMode();
            hitStopwatch += Time.deltaTime;

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
                    if (SeamstressConfig.funny.Value) num2 = Mathf.Clamp(num2, 60f, 120f);
                    float num3 = pullSuitabilityCurve.Evaluate(num2);
                    victim.healthComponent.TakeDamageForce(forceDir - vector2 * damping * (num3 * Mathf.Max(num2, 100f)) * num, alwaysApply: true, disableAirControlUntilCollision: true);
                }

                if (victimMotor != null) bonusDamage = Mathf.Clamp(victimMotor.velocity.magnitude * (SeamstressStaticValues.telekinesisDamageCoefficient * damageStat) + victim.healthComponent.fullCombinedHealth * 0.2f, SeamstressStaticValues.telekinesisDamageCoefficient * damageStat, victim.healthComponent.fullCombinedHealth * 0.7f);
                else bonusDamage = Mathf.Clamp(victimRigid.velocity.magnitude * (SeamstressStaticValues.telekinesisDamageCoefficient * damageStat), SeamstressStaticValues.telekinesisDamageCoefficient * damageStat, victim.healthComponent.fullHealth * 0.5f);
                if (Util.HasEffectiveAuthority(victimBody.gameObject) && detonateNextFrame)
                {
                    EffectManager.SpawnEffect(SeamstressAssets.genericImpactExplosionEffect, new EffectData
                    {
                        origin = victimBody.footPosition,
                        rotation = Quaternion.identity,
                        color = SeamstressAssets.coolRed,
                    }, true);
                    EffectManager.SpawnEffect(SeamstressAssets.slamEffect, new EffectData
                    {
                        origin = victimBody.footPosition,
                        rotation = Quaternion.identity,
                    }, true);
                    CharacterBody component = gameObject.GetComponent<CharacterBody>();
                    float num = component.damage;
                    BlastAttack blastAttack = new BlastAttack();
                    blastAttack.position = victimBody.footPosition;
                    blastAttack.baseDamage = bonusDamage;
                    blastAttack.baseForce = 0f;
                    blastAttack.bonusForce = Vector3.zero;
                    blastAttack.radius = 10f;
                    blastAttack.attacker = gameObject;
                    blastAttack.inflictor = gameObject;
                    blastAttack.teamIndex = component.teamComponent.teamIndex;
                    blastAttack.crit = component.RollCrit();
                    blastAttack.procChainMask = default;
                    blastAttack.procCoefficient = 1f;
                    blastAttack.falloffModel = BlastAttack.FalloffModel.Linear;
                    blastAttack.damageColorIndex = DamageColorIndex.Default;
                    blastAttack.damageType = DamageType.Stun1s;
                    if (butchered)
                    {
                        blastAttack.AddModdedDamageType(DamageTypes.CutDamage);
                        blastAttack.AddModdedDamageType(DamageTypes.InsatiableLifeSteal);
                    }
                    blastAttack.attackerFiltering = AttackerFiltering.Default;
                    blastAttack.Fire();
                    detonateNextFrame = false;
                    hitStopwatch = 0f;
                }
            }
        }
        private void DoSplashDamage(ref MovementHitInfo movementHitInfo)
        {
            float num = Mathf.Abs(movementHitInfo.velocity.magnitude);
            float num2 = Mathf.Max(num - (characterBody.baseMoveSpeed + 70f), 0f);
            if (num2 > 0 && hitStopwatch > 0.75) detonateNextFrame = true;
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}