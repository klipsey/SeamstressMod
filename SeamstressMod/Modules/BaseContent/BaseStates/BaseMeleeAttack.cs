using EntityStates;
using R2API;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using SeamstressMod.Seamstress.Content;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SeamstressMod.Modules.BaseStates
{
    public abstract class BaseMeleeAttack : BaseSeamstressSkillState, SteppedSkillDef.IStepSetter
    {
        public int swingIndex;
        public bool scissorHit;
        protected string hitboxGroupName = "Sword";

        protected DamageType damageType = DamageType.Generic;
        protected List<DamageAPI.ModdedDamageType> moddedDamageTypeHolder = new List<DamageAPI.ModdedDamageType>();
        protected bool isFlatDamage = false;
        protected float damageTotal = 3.5f;
        protected float procCoefficient = 1f;
        protected float pushForce = 300f;
        protected Vector3 bonusForce = Vector3.zero;
        protected float baseDuration = 1f;
        protected float baseScissorDuration = 1.5f;

        protected float attackStartPercentTime = 0.2f;
        protected float attackEndPercentTime = 0.4f;
        
        protected float earlyExitPercentTime = 0.4f;

        protected float hitStopDuration = 0.012f;
        protected float attackRecoil = 0.75f;
        protected float hitHopVelocity = 4f;

        protected string swingSoundString = "";
        protected string hitSoundString = "";
        protected string muzzleString = "SwingCenter";
        protected string playbackRateParam = "Slash.playbackRate";
        protected GameObject swingEffectPrefab;
        protected GameObject bonusSwingEffectPrefab;
        protected GameObject hitEffectPrefab;
        protected NetworkSoundEventIndex impactSound;
        protected bool buffer = false;
        protected bool setDiffState = false;
        protected EntityState setState;
        public float duration;
        public float scissorDuration;
        protected bool hasFired;
        protected bool hasFired2;
        protected bool hasPlayed;
        private float hitPauseTimer;
        protected OverlapAttack attack;
        protected OverlapAttack scissorAttack;
        protected bool inHitPause;
        private bool hasHopped;
        protected float stopwatch;
        protected Animator animator;
        private BaseState.HitStopCachedState hitStopCachedState;
        private Vector3 storedVelocity;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            scissorDuration = baseScissorDuration / attackSpeedStat;

            animator = GetModelAnimator();
            StartAimMode(0.5f + duration, false);

            PlayAttackAnimation();

            attack = new OverlapAttack();
            attack.damageType = damageType;
            foreach(DamageAPI.ModdedDamageType i in moddedDamageTypeHolder)
            {
                attack.AddModdedDamageType(i);
            }
            moddedDamageTypeHolder.Clear();
            attack.attacker = gameObject;
            attack.inflictor = gameObject;
            attack.teamIndex = GetTeam();
            if (!isFlatDamage) attack.damage = damageTotal * base.damageStat;
            else attack.damage = damageTotal;
            attack.procCoefficient = procCoefficient;
            attack.hitEffectPrefab = hitEffectPrefab;
            attack.forceVector = bonusForce;
            attack.pushAwayForce = pushForce;
            attack.hitBoxGroup = FindHitBoxGroup(hitboxGroupName);
            attack.isCrit = RollCrit();
            attack.impactSound = impactSound;
        }

        protected virtual void PlayAttackAnimation()
        {
            if(!hasPlayed)
            {
                PlayCrossfade("Gesture, Override", "Slash" + (1 + swingIndex), playbackRateParam, duration, 0.05f);
                hasPlayed = true;
            }
        }
        protected virtual void PlayTrueAttackAnimation()
        {
            PlayCrossfade("Gesture, Override", swingIndex % 2 == 0 ? "Slash1" : "Slash2", "Slash.playbackRate", duration * (attackEndPercentTime - attackStartPercentTime), 0.1f * duration);
        }
        public override void OnExit()
        {
            if (inHitPause)
            {
                RemoveHitstop();
            }
            base.OnExit();
        }

        protected virtual void PlaySwingEffect()
        {
            EffectManager.SimpleMuzzleFlash(swingEffectPrefab, gameObject, muzzleString, false);
        }

        protected virtual void OnHitEnemyAuthority()
        {
            Util.PlaySound(hitSoundString, gameObject);

            if (!hasHopped)
            {
                if (base.characterMotor && !base.characterMotor.isGrounded && hitHopVelocity > 0f)
                {
                    SmallHop(base.characterMotor, hitHopVelocity);
                }

                hasHopped = true;
            }

            ApplyHitstop();
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

        protected virtual void FireAttack()
        {
            if (base.isAuthority)
            {
                if (attack.Fire())
                {
                    OnHitEnemyAuthority();
                }
            }
        }

        private void EnterAttack()
        {
            Util.PlayAttackSpeedSound(swingSoundString, base.gameObject, attackSpeedStat);
            PlaySwingEffect();
            if (buffer == true)
            {
                PlayTrueAttackAnimation();
            }

            if (base.isAuthority)
            {
                AddRecoil(-1f * attackRecoil, -2f * attackRecoil, -0.5f * attackRecoil, 0.5f * attackRecoil);
            }
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

            bool fireStarted = stopwatch >= duration * attackStartPercentTime;
            bool fireEnded = stopwatch >= duration * attackEndPercentTime;
            bool fireStartedS = stopwatch >= scissorDuration * attackStartPercentTime;
            bool fireEndedS = stopwatch >= scissorDuration * attackEndPercentTime;

            //to guarantee attack comes out if at high attack speed the stopwatch skips past the firing duration between frames
            if (!scissorHit)
            {
                if (fireStarted && !fireEnded || fireStarted && fireEnded && !hasFired)
                {
                    if (!hasFired)
                    {
                        EnterAttack();
                        hasFired = true;
                    }
                    FireAttack();
                }

                if (stopwatch >= duration && base.isAuthority && !setDiffState)
                {
                    outer.SetNextStateToMain();
                    return;
                }
                else if (stopwatch >= duration && base.isAuthority && setDiffState)
                {
                    outer.SetNextState(setState);
                    return;
                }
            }
            else
            {
                if (fireStarted && !fireEnded || fireStarted && fireEnded && !hasFired)
                {
                    if (!hasFired)
                    {
                        EnterAttack();
                        swingEffectPrefab = bonusSwingEffectPrefab;
                        hasFired = true;
                    }
                    FireAttack();
                }
                if (fireStartedS && !fireEndedS || fireStartedS && fireEndedS && !hasFired2)
                {
                    if (!hasFired2)
                    {
                        hitStopDuration = 0.1f;
                        attack = new OverlapAttack();
                        attack.attacker = gameObject;
                        attack.inflictor = gameObject;
                        attack.teamIndex = GetTeam();
                        if (!isFlatDamage) attack.damage = SeamstressStaticValues.scissorSlashDamageCoefficient * base.damageStat;
                        else attack.damage = damageTotal;
                        attack.procCoefficient = procCoefficient;
                        attack.hitEffectPrefab = hitEffectPrefab;
                        attack.forceVector = bonusForce;
                        attack.pushAwayForce = pushForce;
                        attack.hitBoxGroup = FindHitBoxGroup(hitboxGroupName);
                        attack.isCrit = RollCrit();
                        attack.impactSound = impactSound;
                        attack.damageType = damageType;
                        moddedDamageTypeHolder.Clear();
                        foreach (DamageAPI.ModdedDamageType i in moddedDamageTypeHolder)
                        {
                            attack.AddModdedDamageType(i);
                        }
                        if (attack.HasModdedDamageType(DamageTypes.NoScissors)) attack.RemoveModdedDamageType(DamageTypes.NoScissors);
                        attack.pushAwayForce = 600f;
                        attackRecoil = 0.2f;
                        swingSoundString = "Play_imp_attack";
                        if (muzzleString == "SwingLeftSmall")
                        {
                            attack.hitBoxGroup = FindHitBoxGroup("Left");
                            muzzleString = "SwingLeft";
                        }
                        else if(muzzleString == "SwingRightSmall")
                        {
                            attack.hitBoxGroup = FindHitBoxGroup("Right");
                            muzzleString = "SwingRight";
                        }
                        else if(muzzleString == "SwingCenterSmall")
                        {
                            swingSoundString = "Play_bandit2_m2_slash";
                            muzzleString = "SwingCenter";
                            attack.hitBoxGroup = FindHitBoxGroup("SwordBig");
                        }

                        EnterAttack();
                        hasFired2 = true;
                    }
                    FireAttack();
                }
                if (stopwatch >= scissorDuration && base.isAuthority && !setDiffState)
                {
                    outer.SetNextStateToMain();
                    return;
                }
                else if (stopwatch >= scissorDuration && base.isAuthority && setDiffState)
                {
                    outer.SetNextState(setState);
                    return;
                }
            }
        }

        private void RemoveHitstop()
        {
            ConsumeHitStopCachedState(hitStopCachedState, base.characterMotor, animator);
            inHitPause = false;
            base.characterMotor.velocity = storedVelocity;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (!scissorHit && stopwatch >= duration * earlyExitPercentTime)
            {
                return InterruptPriority.Any;
            }
            else if(scissorHit && stopwatch >= scissorDuration * earlyExitPercentTime)
            {
                return InterruptPriority.Any;
            }
            return InterruptPriority.Skill;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.swingIndex);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.swingIndex = reader.ReadInt32();
        }

        public void SetStep(int i)
        {
            this.swingIndex = i;
        }
    }
}