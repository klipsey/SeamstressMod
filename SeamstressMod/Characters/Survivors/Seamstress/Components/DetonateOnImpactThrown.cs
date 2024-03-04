using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;
using TMPro;
using UnityEngine.Networking;

namespace SeamstressMod.Survivors.Seamstress
{
    public class DetonateOnImpactThrown : MonoBehaviour
    {
        public HurtBox victim;

        public GameObject attacker;

        public Rigidbody tempRigidbody;

        public SphereCollider tempSphereCollider;

        private CharacterBody victimBody;

        private CharacterMotor victimMotor;

        private Rigidbody victimRigid;

        private RigidbodyMotor victimRigidMotor;

        public CollisionDetectionMode coll;

        public bool bodyCouldTakeImpactDamage;

        private float stopwatch;

        public float previousMass;

        private void OnAwake()
        {
            victimBody = victim.healthComponent.body;
            victimMotor = victimBody.characterMotor;
            victimRigidMotor = victimBody.gameObject.GetComponent<RigidbodyMotor>();
            victimRigid = victim.gameObject.GetComponent<Rigidbody>();
            if (victimMotor)
            {
                victimMotor.onMovementHit += DoSplashDamage;
            }
        }
        private void FixedUpdate()
        {
            stopwatch += Time.fixedDeltaTime;
            if(stopwatch > 5f)
            {
                Object.Destroy(this);
            }
        }
        private void OnCollisionEnter(UnityEngine.Collision collision)
        {
            if(!victimMotor)
            {
                if (collision.gameObject.layer == LayerIndex.world.intVal || collision.gameObject.layer == LayerIndex.entityPrecise.intVal || collision.gameObject.layer == LayerIndex.defaultLayer.intVal)
                {
                    float bonusDamage = Mathf.Clamp(victimRigid.velocity.magnitude * (SeamstressStaticValues.telekinesisDamageCoefficient * attacker.GetComponent<CharacterBody>().damage), SeamstressStaticValues.telekinesisDamageCoefficient * attacker.GetComponent<CharacterBody>().damage, victim.healthComponent.fullHealth * 0.5f); ;
                    EffectManager.SpawnEffect(SeamstressAssets.genericImpactExplosionEffect, new EffectData
                    {
                        origin = victimBody.footPosition,
                        rotation = Quaternion.identity,
                        color = new Color(84f / 255f, 0f / 255f, 11f / 255f),
                    }, true);
                    CharacterBody component = attacker.GetComponent<CharacterBody>();
                    float num = component.damage;
                    BlastAttack blastAttack = new BlastAttack();
                    blastAttack.position = victimBody.footPosition;
                    blastAttack.baseDamage = SeamstressStaticValues.telekinesisDamageCoefficient * num + bonusDamage;
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
                    EndGrab();
                }
            }
        }

        private void DoSplashDamage(ref CharacterMotor.MovementHitInfo movementHitInfo)
        {
            float bonusDamage = Mathf.Clamp(victimMotor.velocity.magnitude * (SeamstressStaticValues.telekinesisDamageCoefficient * attacker.GetComponent<CharacterBody>().damage), SeamstressStaticValues.telekinesisDamageCoefficient * attacker.GetComponent<CharacterBody>().damage, victim.healthComponent.fullHealth * 0.5f); ;
            EffectManager.SpawnEffect(SeamstressAssets.genericImpactExplosionEffect, new EffectData
            {
                origin = victimBody.footPosition,
                rotation = Quaternion.identity,
                color = new Color(84f / 255f, 0f / 255f, 11f / 255f),
            }, true);
            CharacterBody component = attacker.GetComponent<CharacterBody>();
            float num = component.damage;
            BlastAttack blastAttack = new BlastAttack();
            blastAttack.position = victimBody.footPosition;
            blastAttack.baseDamage = SeamstressStaticValues.telekinesisDamageCoefficient * num + bonusDamage;
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
            EndGrab();
        }
        
        private void EndGrab()
        {
            if (victimRigidMotor)
            {
                victimRigidMotor.canTakeImpactDamage = bodyCouldTakeImpactDamage;
                victimRigidMotor.enabled = true;
            }
            if (tempRigidbody)
            {
                GameObject.Destroy(tempRigidbody);
            }
            if (tempSphereCollider)
            {
                GameObject.Destroy(tempSphereCollider);
            }
            if (NetworkServer.active && victimBody)
            {
                victimBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            }
            victimRigid.collisionDetectionMode = coll;
            if(SeamstressStaticValues.funny)
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
            Object.Destroy(this);
        }

    }
}
