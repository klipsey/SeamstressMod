using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;
using TMPro;
using UnityEngine.Networking;
using static RoR2.CharacterMotor;
using R2API;
using SeamstressMod.Seamstress.Content;
using SeamstressMod.Seamstress.SkillStates;

namespace SeamstressMod.Seamstress.Components
{
    public class DetonateOnImpactThrownTelekinesis : MonoBehaviour
    {
        public class DetonateOnImpact : MonoBehaviour
        {
            public DetonateOnImpactThrownTelekinesis endBlast;
            private void OnCollisionEnter(Collision collision)
            {
                float massCalc = endBlast.victimRigid.mass / 10f;
                float num = 60f / massCalc;
                float magnitude = collision.relativeVelocity.magnitude;
                if (collision.gameObject.layer == LayerIndex.world.intVal || collision.gameObject.layer == LayerIndex.entityPrecise.intVal || collision.gameObject.layer == LayerIndex.defaultLayer.intVal && magnitude >= num)
                {
                    endBlast.detonate = true;
                }
            }
        }

        public GameObject attacker;

        public SphereCollider tempSphereCollider;

        private CharacterBody victimBody;

        private CharacterMotor victimMotor;

        private Rigidbody victimRigid;

        private RigidbodyMotor victimRigidMotor;

        public CollisionDetectionMode coll;

        public bool bodyCouldTakeImpactDamage;

        private float stopwatch;

        public float previousMass;

        public bool theyDidNotHaveRigid;

        public bool detonate;

        private bool hasFired;
        private void Awake()
        {
            victimBody = GetComponent<CharacterBody>();
            victimMotor = victimBody.characterMotor;
            victimRigid = victimBody.rigidbody;
            victimRigidMotor = victimBody.gameObject.GetComponent<RigidbodyMotor>();
            detonate = false;
        }

        private void Start()
        {
            if (victimMotor)
            {
                victimMotor.disableAirControlUntilCollision = true;
                victimMotor.onMovementHit += DoSplashDamage;
            }
            else
            {
                if(theyDidNotHaveRigid)
                {
                    victimRigid = victimBody.gameObject.GetComponent<Rigidbody>();
                    tempSphereCollider = victimBody.gameObject.GetComponent<SphereCollider>();
                }
                if (victimBody.gameObject.GetComponent<DetonateOnImpact>()) Destroy(victimBody.gameObject.GetComponent<DetonateOnImpact>());
                victimBody.gameObject.AddComponent<DetonateOnImpact>();
                victimBody.gameObject.GetComponent<DetonateOnImpact>().endBlast = this;
            }
            stopwatch = 0f;
        }
        private void FixedUpdate()
        {
            stopwatch += Time.fixedDeltaTime;
            if (Util.HasEffectiveAuthority(victimBody.gameObject) && detonate && !hasFired)
            {
                CharacterBody component = attacker.GetComponent<CharacterBody>();
                float bonusDamage = Mathf.Clamp(victimRigid.velocity.magnitude * 
                    (SeamstressConfig.telekinesisDamageCoefficient.Value * attacker.GetComponent<CharacterBody>().damage * 
                    component.GetBuffCount(SeamstressBuffs.Needles)), 
                    SeamstressConfig.telekinesisDamageCoefficient.Value * 
                    attacker.GetComponent<CharacterBody>().damage * component.GetBuffCount(SeamstressBuffs.Needles), victimBody.healthComponent.fullHealth * 0.75f);
                EffectManager.SpawnEffect(SeamstressAssets.impactExplosionEffectDefault, new EffectData
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
                SeamstressController seamstressController = attacker.GetComponent<SeamstressController>();
                BlastAttack blastAttack = new BlastAttack();
                blastAttack.position = victimBody.footPosition;
                blastAttack.baseDamage = bonusDamage;
                blastAttack.baseForce = 800f;
                blastAttack.bonusForce = Vector3.zero;
                blastAttack.radius = 10f;
                blastAttack.attacker = attacker;
                blastAttack.inflictor = attacker;
                blastAttack.teamIndex = component.teamComponent.teamIndex;
                blastAttack.crit = component.RollCrit();
                blastAttack.procChainMask = default;
                blastAttack.procCoefficient = 1f;
                blastAttack.falloffModel = BlastAttack.FalloffModel.Linear;
                blastAttack.damageColorIndex = DamageColorIndex.Default;
                blastAttack.damageType = DamageType.Stun1s | DamageType.AOE;
                if (attacker.GetComponent<CharacterBody>().HasBuff(SeamstressBuffs.SeamstressInsatiableBuff))
                {
                    blastAttack.AddModdedDamageType(DamageTypes.CutDamage);
                }
                blastAttack.AddModdedDamageType(DamageTypes.SeamstressLifesteal);
                blastAttack.attackerFiltering = AttackerFiltering.Default;
                blastAttack.Fire();
                stopwatch = 5f;
                detonate = false;
                hasFired = true;
            }
            if (stopwatch >= 5f)
            {
                EndGrab();
            }
        }

        private void DoSplashDamage(ref MovementHitInfo movementHitInfo)
        {
            float num = Mathf.Abs(movementHitInfo.velocity.magnitude);
            float num2 = Mathf.Max(num - (victimBody.baseMoveSpeed + 70f), 0f);
            if(num2 > 0) detonate = true;
        }

        private void EndGrab()
        {
            if (victimBody.gameObject.GetComponent<DetonateOnImpact>())
            {
                Destroy(victimBody.gameObject.GetComponent<DetonateOnImpact>());
            }
            if (victimRigidMotor)
            {
                victimRigidMotor.canTakeImpactDamage = bodyCouldTakeImpactDamage;
                victimRigidMotor.enabled = true;
            }
            if (victimMotor)
            {
                victimMotor.disableAirControlUntilCollision = false;
                victimMotor.onMovementHit -= DoSplashDamage;
            }
            if (NetworkServer.active && victimBody)
            {
                victimBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            }
            if (coll != victimRigid.collisionDetectionMode)
            {
                victimRigid.collisionDetectionMode = coll;
            }
            if (theyDidNotHaveRigid)
            {
                Destroy(victimRigid);
                Destroy(tempSphereCollider);
            }
            if (SeamstressConfig.heavyEnemy.Value)
            {
                if (victimMotor && victimMotor.mass != previousMass)
                {
                    victimMotor.mass = previousMass;
                    victimMotor.mass = Mathf.Clamp(victimMotor.mass, 60f, 120f);
                }
                else if (victimRigid && victimRigid.mass != previousMass)
                {
                    victimRigid.mass = previousMass;
                    victimRigid.mass = Mathf.Clamp(victimRigid.mass, 60f, 120f);
                }
            }
            Destroy(this);
        }
    }
}
