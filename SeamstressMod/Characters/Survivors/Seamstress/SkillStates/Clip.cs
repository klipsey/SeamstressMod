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

        private GameObject hitEffectPrefab = SeamstressAssets.scissorsHitImpactEffect;

        protected Animator animator;

        private OverlapAttack overlapAttack;
        private DamageType damageType = DamageType.Generic;
        private DamageAPI.ModdedDamageType moddedDamageType2 = DamageTypes.ButcheredLifeSteal;
        private DamageAPI.ModdedDamageType moddedDamageType = DamageTypes.CutDamage;
        private float damageCoefficient = SeamstressStaticValues.clipDamageCoefficient;
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

        protected string hitBoxString = "SwordBig";

        private int snips;

        public static float baseDuration = 0.5f;

        private float duration;

        private float firstSnip;

        private float secondSnip;

        private float snipInterval;

        private float lastSnip;

        private bool hasFired;

        private bool hasFired2;

        private bool iAmAwesome;

        private bool noScissors;

        private Vector3 storedVelocity;
        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active)
            {
                snips = needleCount;
                if (scissorCount < 2) noScissors = true;
            }
            if(noScissors)
            {
                hitBoxString = "Sword";
            }
            animator = GetModelAnimator();
            duration = baseDuration / attackSpeedStat;
            firstSnip = duration * 0.2f; //0.1 - 0.2 0.2 0.4 - 0.5
            secondSnip = duration * 0.4f;
            snipInterval = 0;
            lastSnip = duration - firstSnip;

            StartAimMode(0.5f + duration, false);
            PlayAttackAnimation();
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
            if (stopwatch > firstSnip && !hasFired) 
            {
                hasFired = true;
                if(!noScissors)
                {
                    iAmAwesome = true;
                }
                EnterAttack();
                FireAttack();
            }
            //if equal to 2-4 snips snip until 1 snip left
            if (stopwatch > secondSnip + snipInterval && snipInterval < secondSnip && snips > 0)
            {
                EnterAttack();
                FireAttack();
                snips--;
                snipInterval += secondSnip / 5f;
                if (NetworkServer.active) characterBody.RemoveBuff(SeamstressBuffs.needles);
            }
            //if only 1 snip left, fire
            if(stopwatch > lastSnip && !hasFired2)
            {
                hasFired2 = true;
                if (!noScissors)
                {
                    iAmAwesome = true;
                }
                EnterAttack();
                FireAttack();
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
        private void EnterAttack()
        {
            if(noScissors)
            {
                Util.PlayAttackSpeedSound("Play_imp_attack", gameObject, attackSpeedStat);
            }
            else
            {
                Util.PlayAttackSpeedSound("Play_bandit2_m2_impact", gameObject, attackSpeedStat);
            }
        }
        protected virtual void PlaySwingEffect()
        {
            if (!supaEffect)
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
                    UnityEngine.Object.Instantiate(SeamstressAssets.scissorsComboSwingEffect, transform);
                    UnityEngine.Object.Instantiate(supaEffect, transform2);
                    iAmAwesome = false;
                }
                else
                {
                    UnityEngine.Object.Instantiate(SeamstressAssets.scissorsSwingEffect, transform);
                    UnityEngine.Object.Instantiate(supaEffect, transform2);
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
                overlapAttack.AddModdedDamageType(moddedDamageType);
            }
            overlapAttack.AddModdedDamageType(DamageTypes.ClipLifeSteal);
            overlapAttack.attacker = gameObject;
            overlapAttack.inflictor = gameObject;
            overlapAttack.teamIndex = GetTeam();
            overlapAttack.damage = damageCoefficient * base.damageStat;
            overlapAttack.procCoefficient = procCoefficient;
            overlapAttack.hitEffectPrefab = hitEffectPrefab;
            overlapAttack.forceVector = bonusForce;
            overlapAttack.pushAwayForce = pushForce;
            overlapAttack.hitBoxGroup = FindHitBoxGroup(hitBoxString);
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
