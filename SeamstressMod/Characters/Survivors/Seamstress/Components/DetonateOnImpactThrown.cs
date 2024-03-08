using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;
using TMPro;
using UnityEngine.Networking;
using static RoR2.CharacterMotor;

namespace SeamstressMod.Survivors.Seamstress
{
    public class DetonateOnImpactThrown : MonoBehaviour
    {
        public GameObject attacker;

        private CharacterBody victimBody;

        private CharacterMotor victimMotor;

        private Rigidbody victimRigid;

        private RigidbodyMotor victimRigidMotor;

        public CollisionDetectionMode coll;

        public bool bodyCouldTakeImpactDamage;

        private float stopwatch;

        public float previousMass;

        public bool theyDidNotHaveRigid;

        private void Awake()
        {
            victimBody = base.gameObject.GetComponent<CharacterBody>();
            victimMotor = victimBody.characterMotor;
            victimRigid = victimBody.rigidbody;
            victimRigidMotor = base.GetComponent<RigidbodyMotor>();
            if (victimMotor != null)
            {
                victimMotor.disableAirControlUntilCollision = true;
                victimMotor.onMovementHit += DoSplashDamage;
            }
        }

        private void Start()
        {
            stopwatch = 0f;
        }
        private void FixedUpdate()
        {
            stopwatch += Time.fixedDeltaTime;
            if(stopwatch > 5f)
            {
                EndGrab();
            }
        }
        private void OnCollisionEnter(UnityEngine.Collision collision)
        {
            float massCalc = (victimMotor ? victimMotor.mass : victimRigid.mass) / 10f;
            float num = 60f / massCalc;
            float magnitude = collision.relativeVelocity.magnitude;
            if (victimMotor == null && magnitude >= num)
            {
                if (collision.gameObject.layer == LayerIndex.world.intVal || collision.gameObject.layer == LayerIndex.entityPrecise.intVal || collision.gameObject.layer == LayerIndex.defaultLayer.intVal)
                {
                    float bonusDamage = Mathf.Clamp(victimRigid.velocity.magnitude * (SeamstressStaticValues.telekinesisDamageCoefficient * attacker.GetComponent<CharacterBody>().damage), SeamstressStaticValues.telekinesisDamageCoefficient * attacker.GetComponent<CharacterBody>().damage, victimBody.healthComponent.fullHealth * 0.7f); ;
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
                    CharacterBody component = attacker.GetComponent<CharacterBody>();
                    float num2 = component.damage;
                    BlastAttack blastAttack = new BlastAttack();
                    blastAttack.position = victimBody.footPosition;
                    blastAttack.baseDamage = SeamstressStaticValues.telekinesisDamageCoefficient * num2 + bonusDamage;
                    blastAttack.baseForce = 800f;
                    blastAttack.bonusForce = Vector3.zero;
                    blastAttack.radius = 10f;
                    blastAttack.attacker = attacker;
                    blastAttack.inflictor = attacker;
                    blastAttack.teamIndex = component.teamComponent.teamIndex;
                    blastAttack.crit = component.RollCrit();
                    blastAttack.procChainMask = default(ProcChainMask);
                    blastAttack.procCoefficient = 1f;
                    blastAttack.falloffModel = BlastAttack.FalloffModel.Linear;
                    blastAttack.damageColorIndex = DamageColorIndex.Default;
                    blastAttack.damageType = DamageType.Stun1s;
                    blastAttack.attackerFiltering = AttackerFiltering.Default;
                    blastAttack.Fire();
                }
            }
            EndGrab();
        }

        private void DoSplashDamage(ref CharacterMotor.MovementHitInfo movementHitInfo)
        {
            float massCalc = (victimMotor ? victimMotor.mass : victimRigid.mass) / 10f;
            float num = 60f / massCalc;
            float magnitude = movementHitInfo.velocity.magnitude;
            if(magnitude >= num)
            {
                float bonusDamage = Mathf.Clamp(victimMotor.velocity.magnitude * (SeamstressStaticValues.telekinesisDamageCoefficient * attacker.GetComponent<CharacterBody>().damage), SeamstressStaticValues.telekinesisDamageCoefficient * attacker.GetComponent<CharacterBody>().damage, victimBody.healthComponent.fullHealth * 0.7f); ;
                EffectManager.SpawnEffect(SeamstressAssets.genericImpactExplosionEffect, new EffectData
                {
                    origin = victimBody.footPosition,
                    rotation = Quaternion.identity,
                    color = SeamstressAssets.coolRed,
                }, true);
                CharacterBody component = attacker.GetComponent<CharacterBody>();
                float num2 = component.damage;
                BlastAttack blastAttack = new BlastAttack();
                blastAttack.position = victimBody.footPosition;
                blastAttack.baseDamage = SeamstressStaticValues.telekinesisDamageCoefficient * num2 + bonusDamage;
                blastAttack.baseForce = 800f;
                blastAttack.bonusForce = Vector3.up * 2000f;
                blastAttack.radius = 10f;
                blastAttack.attacker = attacker;
                blastAttack.inflictor = attacker;
                blastAttack.teamIndex = component.teamComponent.teamIndex;
                blastAttack.crit = component.RollCrit();
                blastAttack.procChainMask = default(ProcChainMask);
                blastAttack.procCoefficient = 1f;
                blastAttack.falloffModel = BlastAttack.FalloffModel.Linear;
                blastAttack.damageColorIndex = DamageColorIndex.Default;
                blastAttack.damageType = DamageType.Stun1s;
                blastAttack.attackerFiltering = AttackerFiltering.Default;
                blastAttack.Fire();
                victimMotor.onMovementHit -= DoSplashDamage;
            }
            EndGrab();
        }
        
        private void EndGrab()
        {
            if (victimRigidMotor != null)
            {
                victimRigidMotor.canTakeImpactDamage = bodyCouldTakeImpactDamage;
                victimRigidMotor.enabled = true;
            }
            if (theyDidNotHaveRigid)
            {
                GameObject.Destroy(base.gameObject.GetComponent<Rigidbody>());
                GameObject.Destroy(base.gameObject.GetComponent<SphereCollider>());
            }
            if (NetworkServer.active && victimBody != null)
            {
                victimBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            }
            if (coll != victimRigid.collisionDetectionMode )
            {
                victimRigid.collisionDetectionMode = coll;
            }
            if (SeamstressStaticValues.funny)
            {
                if (victimMotor != null && victimMotor.mass != previousMass)
                {
                    victimMotor.mass = previousMass;
                    victimMotor.mass = Mathf.Clamp(victimMotor.mass, 60f, 120f);
                }
                else if (victimRigid != null && victimRigid.mass != previousMass)
                {
                    victimRigid.mass = previousMass;
                    victimRigid.mass = Mathf.Clamp(victimRigid.mass, 60f, 120f);
                }
            }
            Object.Destroy(this);
        }

    }
}
