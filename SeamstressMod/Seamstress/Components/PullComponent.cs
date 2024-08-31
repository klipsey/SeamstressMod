using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using R2API;
using System;
using SeamstressMod.Seamstress.Content;

namespace SeamstressMod.Seamstress.Components

{
    public class PullComponent : MonoBehaviour
    {
        private static GameObject pullTracerPrefab = SeamstressAssets.parrySlashEffect;

        public CharacterBody attackerBody;

        private Rigidbody victimRigidBody;

        private CharacterBody victimBody;

        private bool hasFired;

        private float stopwatch;

        private float dashStopwatch;

        private float delay = 0.75f;

        private float duration = 0.4f;

        private Vector3 pullStart;

        private Vector3 pullEnd;

        private HurtBox hurtBox;

        private HurtBox attackerHurtBox;
        private void Awake()
        {
            victimBody = gameObject.GetComponent<CharacterBody>();
            victimRigidBody = gameObject.GetComponent<Rigidbody>();
        }
        private void Pull()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            hurtBox = victimBody.mainHurtBox;
            attackerHurtBox = attackerBody.mainHurtBox;
            if (!hurtBox)
            {
                return;
            }
            CalculateSnapDestination();
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
            damage.AddModdedDamageType(DamageTypes.SeamstressLifesteal);
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
        private void SetPosition(Vector3 newPosition)
        {
            if (victimBody.characterMotor)
            {
                victimBody.characterMotor.Motor.SetPositionAndRotation(newPosition, Quaternion.identity);
            }
            else if (victimRigidBody)
            {
                victimRigidBody.MovePosition(newPosition);
            }
        }
        private void CalculateSnapDestination()
        {
            pullStart = hurtBox.transform.position;
            pullEnd = attackerHurtBox.transform.position;
            pullEnd = Vector3.Lerp(pullStart, pullEnd, 0.8f);
        }
        private void FixedUpdate()
        {
            stopwatch += Time.deltaTime;
            if (stopwatch > delay)
            {
                dashStopwatch += Time.deltaTime;
                if (!hasFired)
                {
                    Pull();
                    hasFired = true;
                }
                else
                {
                    if (victimBody.characterMotor)
                    {
                        victimBody.characterMotor.velocity = Vector3.zero;
                    }
                    else if (victimRigidBody)
                    {
                        victimRigidBody.velocity = Vector3.zero;
                    }
                    SetPosition(Vector3.Lerp(pullStart, pullEnd, dashStopwatch / duration));
                }
                if (dashStopwatch >= duration)
                {
                    Util.PlaySound("Play_imp_overlord_teleport_end", gameObject);
                    DestroyImmediate(this);
                }
            }
        }
    }
}

