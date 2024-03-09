using System.Linq;
using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using SeamstressMod.Modules;
using System.Diagnostics;
using R2API;

namespace SeamstressMod.Survivors.Seamstress

{
    public class PullComponent : MonoBehaviour
    {
        public CharacterBody attackerBody;

        private CharacterMotor victimMotor;

        private RigidbodyMotor victimRigidMotor;

        private Rigidbody victimRigidbody;

        private static GameObject dashPrefab = SeamstressAssets.impDash;

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
            duration = delay + dashDuration;
            victimBody = this.gameObject.GetComponent<CharacterBody>();
            victimMotor = this.gameObject.GetComponent<CharacterMotor>();
            victimRigidbody = this.gameObject.GetComponent<Rigidbody>();
            victimRigidMotor = this.gameObject.GetComponent<RigidbodyMotor>();
            pullStart = base.transform.position;
        }
        private void CreateBlinkEffect(Vector3 origin)
        {
            EffectData effectData = new EffectData();
            effectData.rotation = Util.QuaternionSafeLookRotation((pullEnd - pullStart).normalized);
            effectData.origin = origin;
            effectData.scale = 4f;
            EffectManager.SpawnEffect(dashPrefab, effectData, transmit: true);
        }
        private void SetPosition(Vector3 newPosition)
        {
            if (this.victimMotor)
            {
                this.victimMotor.Motor.SetPositionAndRotation(newPosition, Quaternion.identity);
            }
            else
            {
                this.victimRigidMotor.AddDisplacement(newPosition);
            }
        }
        private void FixedUpdate()
        {
            stopwatch += Time.fixedDeltaTime;
            if (attackerBody.gameObject.GetComponent<SeamstressController>().isDashing == false && stopwatch > delay)
            {
                if (!hasFired)
                {
                    DamageInfo damage = new DamageInfo
                    {
                        damage = attackerBody.damage * SeamstressStaticValues.parryDamageCoefficient,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = DamageType.Stun1s,
                        attacker = attackerBody.gameObject,
                        crit = attackerBody.RollCrit(),
                        force = Vector3.zero,
                        inflictor = attackerBody.gameObject,
                        position = attackerBody.corePosition,
                        procCoefficient = 1f
                    };
                    damage.AddModdedDamageType(DamageTypes.CutDamage);
                    damage.AddModdedDamageType(DamageTypes.ButcheredLifeSteal);
                    victimBody.healthComponent.TakeDamage(damage);
                    pullEnd = attackerBody.transform.position;
                    pullEnd += base.transform.position - victimBody.footPosition;
                    CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
                    hasFired = true;
                }
                if (this.victimMotor)
                {
                    this.victimMotor.velocity = Vector3.zero;
                }
                else
                {
                    victimRigidbody.velocity = Vector3.zero;
                }
                SetPosition(Vector3.Lerp(pullStart, pullEnd, 0.8f));
                if (stopwatch >= duration)
                {
                    CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
                    Util.PlaySound("Play_imp_overlord_teleport_end", base.gameObject);
                    Object.DestroyImmediate(this);
                }
            }
        }
    }
}

