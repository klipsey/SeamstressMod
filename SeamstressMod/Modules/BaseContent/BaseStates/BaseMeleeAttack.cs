using EntityStates;
using R2API;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using SeamstressMod.Survivors.Seamstress;
using UnityEngine;
using UnityEngine.Networking;

namespace SeamstressMod.Modules.BaseStates
{
    public abstract class BaseMeleeAttack : BaseSeamstressSkillState, SteppedSkillDef.IStepSetter
    {
        public int swingIndex;
        protected string hitboxGroupName = "Sword";

        protected DamageType damageType = DamageType.Generic;
        protected DamageAPI.ModdedDamageType moddedDamageType = DamageTypes.Empty;
        protected DamageAPI.ModdedDamageType moddedDamageType2 = DamageTypes.Empty;
        protected DamageAPI.ModdedDamageType moddedDamageType3 = DamageTypes.Empty;
        protected bool isFlatDamage = false;
        protected float damageTotal = 3.5f;
        protected float procCoefficient = 1f;
        protected float pushForce = 300f;
        protected Vector3 bonusForce = Vector3.zero;
        protected float baseDuration = 1f;

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
        protected GameObject hitEffectPrefab;
        protected NetworkSoundEventIndex impactSound;

        public float duration;
        protected bool hasFired;
        private float hitPauseTimer;
        protected OverlapAttack attack;
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
            animator = GetModelAnimator();
            StartAimMode(0.5f + duration, false);

            PlayAttackAnimation();

            attack = new OverlapAttack();
            attack.damageType = damageType;
            attack.AddModdedDamageType(moddedDamageType);
            attack.AddModdedDamageType(moddedDamageType2);
            attack.AddModdedDamageType(moddedDamageType3);
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
            PlayCrossfade("Gesture, Override", "Slash" + (1 + swingIndex), playbackRateParam, duration, 0.05f);
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
            EffectManager.SimpleMuzzleFlash(swingEffectPrefab, gameObject, muzzleString, true);
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
            hasFired = true;
            Util.PlayAttackSpeedSound(swingSoundString, gameObject, attackSpeedStat);

            PlaySwingEffect();

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

            //to guarantee attack comes out if at high attack speed the stopwatch skips past the firing duration between frames
            if (fireStarted && !fireEnded || fireStarted && fireEnded && !hasFired)
            {
                if (!hasFired)
                {
                    EnterAttack();
                }
                FireAttack();
            }

            if (stopwatch >= duration && base.isAuthority)
            {
                outer.SetNextStateToMain();
                return;
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
            if (stopwatch >= duration * earlyExitPercentTime)
            {
                return InterruptPriority.Any;
            }
            return InterruptPriority.Skill;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(swingIndex);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            swingIndex = reader.ReadInt32();
        }

        public void SetStep(int i)
        {
            swingIndex = i;
        }
    }
}