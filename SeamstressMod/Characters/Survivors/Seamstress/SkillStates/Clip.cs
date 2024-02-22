using RoR2;
using RoR2.Projectile;
using UnityEngine;
using R2API;
using SeamstressMod.Survivors.Seamstress;
using SeamstressMod.Modules.BaseStates;
using System;
using EntityStates;
using UnityEngine.Networking;
using IL.RoR2.ConVar;
using static R2API.DamageAPI;
using IL.RoR2.UI;
using static EntityStates.BaseState;
using UnityEngine.UIElements;

namespace SeamstressMod.SkillStates
{
    public class Clip : BaseSeamstressSkillState
    {
        public static GameObject supaEffect = SeamstressAssets.clipSlashEffect;

        public static GameObject boringEffect = SeamstressAssets.clipSwingEffect;

        private GameObject hitEffectPrefab = SeamstressAssets.scissorsHitImpactEffect;

        protected Animator animator;

        private OverlapAttack overlapAttack;
        private DamageType damageType = DamageType.Generic;
        private DamageAPI.ModdedDamageType moddedDamageType2 = DamageTypes.PlanarLifeSteal;
        private DamageAPI.ModdedDamageType moddedDamageType = DamageTypes.CutDamage;
        private float damageCoefficient;
        private float procCoefficient = 1f;
        private float pushForce = 0f;
        private Vector3 bonusForce = Vector3.zero;

        protected float stopwatch;
        private BaseState.HitStopCachedState hitStopCachedState;
        private float hitPauseTimer;
        protected float hitStopDuration = 0.04f;
        private bool hasHopped;
        protected bool inHitPause;
        protected string playbackRateParam = "Slash.playbackRate";

        private int snips;

        private float longWindUp = 0.25f;

        private float shortWindUp = 0.05f;

        private float snipDuration = 0.025f;

        private float duration;

        private float currentDuration = 0f;

        private float exhaustDuration = 6f;

        private bool hasFired;

        private bool iAmAwesome;

        private Vector3 storedVelocity;
        public override void OnEnter()
        {
            SnipMath();
            animator = GetModelAnimator();
            base.OnEnter();
            longWindUp /= attackSpeedStat;
            shortWindUp /= attackSpeedStat;
            snipDuration /= attackSpeedStat;
            if (snips == 1) duration = shortWindUp;
            else if (snips == 2) duration = (2 * longWindUp) + shortWindUp;
            else duration = (2 * longWindUp) + ((snips - 2) * shortWindUp);
            duration += snipDuration * snips;
            currentDuration = shortWindUp;
            base.skillLocator.secondary.stock = 0;
            StartAimMode(0.5f + duration, false);
            PlayAttackAnimation();
            characterBody.skillLocator.secondary.rechargeStopwatch = 0f;
            float exhaustApply = Mathf.Min(exhaustDuration, Mathf.Max(0.5f, exhaustDuration * skillLocator.secondary.cooldownScale - skillLocator.secondary.flatCooldownReduction));
            if (NetworkServer.active) characterBody.AddTimedBuff(SeamstressBuffs.needlesChill, exhaustApply);
            seamCon.lockOutLength = exhaustApply;
            skillLocator.secondary.SetSkillOverride(gameObject, SeamstressAssets.lockOutSkillDef, RoR2.GenericSkill.SkillOverridePriority.Contextual);
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            hitPauseTimer -= Time.fixedDeltaTime;

            if (hitPauseTimer <= 0f && inHitPause)
            {
                RemoveHitstop();
            }

            if (!inHitPause)
            {
                stopwatch += Time.fixedDeltaTime;
            }
            else
            {
                if (base.characterMotor) base.characterMotor.velocity = Vector3.zero;
                if (animator) animator.SetFloat(playbackRateParam, 0f);
            }
            //if higher than 1 snip, fire once 
            if (stopwatch > currentDuration && snips >= 2 && !hasFired) 
            {
                if(snips == 2) currentDuration += snipDuration + (2 * longWindUp);
                else currentDuration += snipDuration + longWindUp;
                hasFired = true;
                iAmAwesome = true;
                EnterAttack();
                FireAttack();
                snips--;
            }
            //if equal to 2-4 snips snip until 1 snip left
            if (stopwatch > currentDuration && snips >= 2)
            {
                if(snips == 2)
                {
                    currentDuration += snipDuration + longWindUp;
                }
                else
                {
                    currentDuration += snipDuration + shortWindUp;
                }
                EnterAttack();
                FireAttack();
                snips--;
            }
            //if only 1 snip left, fire
            if(stopwatch > currentDuration && snips == 1)
            {
                currentDuration += snipDuration;
                iAmAwesome = true;
                EnterAttack();
                FireAttack();
                snips--;
            }
            if (stopwatch > duration && snips == 0 && base.isAuthority) 
            {
                outer.SetNextStateToMain();
            }
        }
        protected void ApplyHitstop()
        {
            if (!inHitPause && hitStopDuration > 0f)
            {
                storedVelocity = base.characterMotor.velocity;
                hitStopCachedState = CreateHitStopCachedState(base.characterMotor, animator, playbackRateParam);
                hitPauseTimer = hitStopDuration / attackSpeedStat;
                inHitPause = true;
            }
        }
        private void RemoveHitstop()
        {
            ConsumeHitStopCachedState(hitStopCachedState, base.characterMotor, animator);
            inHitPause = false;
            base.characterMotor.velocity = storedVelocity;
        }
        protected virtual void OnHitEnemyAuthority()
        {
            if (!hasHopped)
            {
                if (base.characterMotor && !base.characterMotor.isGrounded)
                {
                    SmallHop(base.characterMotor, 4f);
                }

                hasHopped = true;
            }

            ApplyHitstop();
        }
        private void SnipMath()
        {
            if (skillLocator.secondary.stock > 10)
            {
                snips = 5;
            }
            else if (skillLocator.secondary.stock <= 10)
            {
                if (skillLocator.secondary.stock % 2 == 1)
                {
                    snips = (skillLocator.secondary.stock + 1) / 2;
                }
                else
                {
                    snips = skillLocator.secondary.stock / 2;
                }
            }
            damageCoefficient = ((float)skillLocator.secondary.stock / (float)snips) * SeamstressStaticValues.clipDamageCoefficient;
        }
        private void EnterAttack()
        {
            Util.PlayAttackSpeedSound("Play_bandit2_m2_impact", gameObject, attackSpeedStat);
        }
        protected virtual void PlaySwingEffect()
        {
            if (!supaEffect || !boringEffect)
            {
                Log.Debug("Error, no effect?");
                return;
            }
            Transform transform = FindModelChild("SwingCenter");
            Transform transform2 = FindModelChild("SwingCharCenter");
            if (transform)
            {
                if (empowered || iAmAwesome)
                {
                    UnityEngine.Object.Instantiate(SeamstressAssets.scissorsButcheredComboSwingEffect, transform);
                    UnityEngine.Object.Instantiate(supaEffect, transform2);
                    iAmAwesome = false;
                }
                else
                {
                    UnityEngine.Object.Instantiate(SeamstressAssets.scissorsButcheredSwingEffect, transform);
                    UnityEngine.Object.Instantiate(boringEffect, transform2);
                }
            }
        }
        protected virtual void FireAttack()
        {
            hasHopped = false;

            overlapAttack = new OverlapAttack();
            overlapAttack.damageType = damageType;
            if (empowered)
            {
                overlapAttack.AddModdedDamageType(moddedDamageType2);
            }
            overlapAttack.AddModdedDamageType(moddedDamageType);
            overlapAttack.attacker = gameObject;
            overlapAttack.inflictor = gameObject;
            overlapAttack.teamIndex = GetTeam();
            overlapAttack.damage = damageCoefficient * base.damageStat;
            overlapAttack.procCoefficient = procCoefficient;
            overlapAttack.hitEffectPrefab = hitEffectPrefab;
            overlapAttack.forceVector = bonusForce;
            overlapAttack.pushAwayForce = pushForce;
            overlapAttack.hitBoxGroup = FindHitBoxGroup("Sword");
            overlapAttack.isCrit = RollCrit();

            if (base.isAuthority)
            {
                Vector3 direction = GetAimRay().direction;
                direction.y = Mathf.Max(direction.y, direction.y * 0.5f);
                FindModelChild("SwingPivot").rotation = Util.QuaternionSafeLookRotation(direction);
                if (overlapAttack.Fire())
                {
                    OnHitEnemyAuthority();
                }
            }
            PlaySwingEffect();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (stopwatch >= this.duration)
            {
                return InterruptPriority.Any;
            }
            return InterruptPriority.PrioritySkill;
        }
        private void PlayAttackAnimation()
        {
            PlayCrossfade("Gesture, Override", "Slash1", "Slash.playbackRate", this.duration, 0.1f * this.duration);
        }
        public override void OnExit()
        {
            base.OnExit();
        }

    }
}
