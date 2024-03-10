using System.Linq;
using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using SeamstressMod.Modules;
using System.Diagnostics;
using R2API;
using EntityStates.VoidJailer.Weapon;
using UnityEngine.UIElements;

namespace SeamstressMod.Survivors.Seamstress

{
    public class PullComponent : MonoBehaviour
    {
        private AnimationCurve pullSuitabilityCurve = new AnimationCurve();

        private static GameObject pullTracerPrefab = SeamstressAssets.pullShit;

        public CharacterBody attackerBody;

        private Rigidbody victimRigidBody;

        private CharacterBody victimBody;

        private bool hasFired;

        private float stopwatch;

        private float delay = 0.75f;

        private float dashDuration = 0.2f;

        private float duration;

        private Vector3 pullStart;

        private Vector3 pullEnd;
        private void Awake()
        {
            pullSuitabilityCurve.AddKey(0, 1);
            pullSuitabilityCurve.AddKey(2000, 0);
            duration = delay + dashDuration;
            victimBody = this.gameObject.GetComponent<CharacterBody>();
            victimRigidBody = this.gameObject.GetComponent<Rigidbody>();
        }
        private void Pull()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            HurtBox hurtBox = victimBody.mainHurtBox;
            HurtBox attackerHurtBox = attackerBody.mainHurtBox;
            if (!hurtBox)
            {
                return;
            }
            pullStart = hurtBox.transform.position;
            pullEnd = attackerHurtBox.transform.position;
            pullEnd = Vector3.Lerp(pullStart, pullEnd, 0.8f);
            Vector3 vector = pullEnd - pullStart;
            float magnitude = vector.magnitude;
            Vector3 vector2 = vector / magnitude;
            float num = 1f;
            if (victimBody.characterMotor)
            {
                num = victimBody.characterMotor.mass;
            }
            else if (victimRigidBody)
            {
                num = victimRigidBody.mass;
            }
            float num2 = pullSuitabilityCurve.Evaluate(num);
            Vector3 vector3 = vector2;
            float num3 = Trajectory.CalculateInitialYSpeedForHeight(Mathf.Abs(75f - magnitude)) * Mathf.Sign(75f - magnitude);
            vector3 *= num3;
            vector3.y = 2f;
            DamageInfo damage = new DamageInfo
            {
                damage = attackerBody.damage * SeamstressStaticValues.parryDamageCoefficient,
                damageColorIndex = DamageColorIndex.Default,
                damageType = DamageType.Stun1s,
                attacker = attackerBody.gameObject,
                crit = attackerBody.RollCrit(),
                position = hurtBox.transform.position,
                procCoefficient = 1f
            };
            damage.AddModdedDamageType(DamageTypes.CutDamage);
            damage.AddModdedDamageType(DamageTypes.ButcheredLifeSteal);
            if (victimBody.characterMotor) victimBody.characterMotor.Motor.ForceUnground();
            hurtBox.healthComponent.TakeDamageForce(vector3 * (num * num2), alwaysApply: true, disableAirControlUntilCollision: true);
            hurtBox.healthComponent.TakeDamage(damage);
            GlobalEventManager.instance.OnHitEnemy(damage, hurtBox.healthComponent.gameObject);
            if (pullTracerPrefab)
            {
                Vector3 position = attackerBody.corePosition;
                Vector3 start = hurtBox.transform.position;
                EffectData effectData = new EffectData
                {
                    origin = position,
                    start = start
                };
                EffectManager.SpawnEffect(pullTracerPrefab, effectData, transmit: true);
            }
        }
        private void FixedUpdate()
        {
            stopwatch += Time.fixedDeltaTime;
            if (attackerBody.gameObject.GetComponent<SeamstressController>().isDashing == false && stopwatch > delay)
            {
                if (!hasFired)
                {
                    Pull();
                    hasFired = true;
                }
                if (stopwatch >= duration)
                {
                    Util.PlaySound("Play_imp_overlord_teleport_end", base.gameObject);
                    Object.DestroyImmediate(this);
                }
            }
        }
    }
}

