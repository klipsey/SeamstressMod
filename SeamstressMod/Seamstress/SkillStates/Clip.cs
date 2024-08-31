using RoR2;
using RoR2.Projectile;
using UnityEngine;
using R2API;
using SeamstressMod.Modules.BaseStates;
using System;
using EntityStates;
using UnityEngine.Networking;
using SeamstressMod.Seamstress.Content;


namespace SeamstressMod.Seamstress.SkillStates
{
    public class Clip : BaseSeamstressSkillState
    {
        public GameObject clawSlash = SeamstressAssets.clawSlashEffect;

        public GameObject hitEffectPrefab = SeamstressAssets.scissorsHitImpactEffect;

        public GameObject swingEffectPrefab = SeamstressAssets.scissorsSlashComboEffect;

        public GameObject wideEffectPrefab = SeamstressAssets.wideSlashEffect;

        protected Animator animator;

        private OverlapAttack overlapAttack;
        private DamageType damageType = DamageType.Generic;
        private DamageAPI.ModdedDamageType moddedDamageType3 = DamageTypes.Empty;
        private DamageAPI.ModdedDamageType moddedDamageType = DamageTypes.CutDamage;
        private float damageCoefficient = SeamstressStaticValues.clipDamageCoefficient;
        private float procCoefficient = 0.6f;
        private float pushForce = 0f;
        private Vector3 bonusForce = Vector3.zero;

        protected float stopwatch;
        private HitStopCachedState hitStopCachedState;
        private float hitPauseTimer;
        protected float hitStopDuration = 0.04f;
        private bool hasHopped;
        protected bool inHitPause;
        protected string playbackRateParam = "Slash.playbackRate";

        protected string hitBoxString = "SwordBig";

        private int snips;

        private int alternateSwings = 0;

        private float baseDuration = 0.75f;

        private float duration;

        private float firstSnip;

        private float secondSnip;

        private float snipInterval;

        private float lastSnip;

        private bool hasFired;

        private bool hasFired2;

        private bool noScissors;

        private bool inAir;

        private Vector3 storedVelocity;
        public override void OnEnter()
        {
            RefreshState();
            if (seamstressController.blue)
            {
                clawSlash = SeamstressAssets.clawSlashEffect2;
                hitEffectPrefab = SeamstressAssets.scissorsHitImpactEffect2;
                swingEffectPrefab = SeamstressAssets.scissorsSlashComboEffect2;
                wideEffectPrefab = SeamstressAssets.wideSlashEffect2;
            }
            base.OnEnter();
            baseDuration = 0.75f;
            snips = needleCount;
            if (!scissorRight || !scissorLeft)
            {
                noScissors = true;
            }
            if (noScissors)
            {
                hitBoxString = "Sword";
            }
            animator = GetModelAnimator();
            duration = baseDuration / attackSpeedStat;
            firstSnip = duration * 0.2f;
            secondSnip = duration * 0.4f;
            snipInterval = 0;
            lastSnip = duration - firstSnip;
            if (!characterMotor.isGrounded) inAir = true;
            StartAimMode(duration, false);
            PlayAttackAnimation();
            if (inAir) SmallHop(characterMotor, 6f);
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            hitPauseTimer -= Time.deltaTime;

            if (hitPauseTimer <= 0f && inHitPause)
            {
                RemoveHitstop();
            }

            if (!inHitPause)
            {
                stopwatch += Time.deltaTime;
                if (inAir && isAuthority)
                {
                    Vector3 velocity = characterDirection.forward * moveSpeedStat * Mathf.Lerp(3f, 1f, age / duration);
                    velocity.y = characterMotor.velocity.y;
                    characterMotor.velocity = velocity;
                }
            }
            else
            {
                if (characterMotor) characterMotor.velocity = Vector3.zero;
                if (animator) animator.SetFloat(playbackRateParam, 0f);
            }
            if (stopwatch >= firstSnip && !hasFired)
            {
                hasFired = true;
                EnterAttack();
                FireAttack();
            }
            //if equal to 2-4 snips snip until 1 snip left
            if (stopwatch >= secondSnip + snipInterval && snipInterval <= secondSnip && snips > 0)
            {
                EnterAttack();
                FireAttack();
                snips--;
                snipInterval += secondSnip / 5f;
                Util.PlaySound("Play_bandit2_m2_alt_throw", gameObject);
            }
            //if only 1 snip left, fire
            if (stopwatch >= lastSnip && !hasFired2)
            {
                hasFired2 = true;
                EnterAttack();
                FireAttack();
            }
            if (stopwatch >= duration && snips == 0 && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }
        protected void ApplyHitstop()
        {
            if (!inHitPause && hitStopDuration > 0f)
            {
                storedVelocity = characterMotor.velocity;
                hitStopCachedState = CreateHitStopCachedState(characterMotor, animator, playbackRateParam);
                hitPauseTimer = hitStopDuration / attackSpeedStat;
                inHitPause = true;
            }
        }
        private void RemoveHitstop()
        {
            ConsumeHitStopCachedState(hitStopCachedState, characterMotor, animator);
            inHitPause = false;
            characterMotor.velocity = storedVelocity;
        }
        protected virtual void OnHitEnemyAuthority()
        {
            if (!hasHopped)
            {
                if (characterMotor && !characterMotor.isGrounded)
                {
                    SmallHop(characterMotor, 4f);
                }

                hasHopped = true;
            }

            ApplyHitstop();
        }
        private void EnterAttack()
        {
            if (noScissors)
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
            if (!clawSlash)
            {
                Log.Error("Error, no effect?");
                return;
            }
            Transform bigSnip = FindModelChild("SwingCenter");
            Transform smallSlash = FindModelChild("SwingLeftSmall");
            Transform smallSlash2 = FindModelChild("SwingRightSmall");
            Transform bigSnipAir = FindModelChild("SwingCharAirCenter");
            Transform bigSnipAir2 = FindModelChild("SwingCharAirCenter2");
            if (bigSnip && smallSlash && smallSlash2 && bigSnip && bigSnipAir && bigSnipAir2) //lol
            {
                if (noScissors)
                {
                    if (hasFired && !hasFired2)
                    {
                        if (alternateSwings == 0)
                        {
                            UnityEngine.Object.Instantiate(clawSlash, smallSlash);
                            alternateSwings = 1;
                        }
                        else if (alternateSwings == 1)
                        {
                            UnityEngine.Object.Instantiate(clawSlash, smallSlash2);
                            alternateSwings = 0;
                        }
                    }
                    else if (hasFired2)
                    {
                        UnityEngine.Object.Instantiate(clawSlash, smallSlash2);
                    }
                }
                else if (inAir)
                {
                    if (hasFired && !hasFired2)
                    {
                        if (alternateSwings == 0)
                        {
                            UnityEngine.Object.Instantiate(wideEffectPrefab, bigSnipAir2);
                            alternateSwings = 1;
                        }
                        else if (alternateSwings == 1)
                        {
                            UnityEngine.Object.Instantiate(wideEffectPrefab, bigSnipAir);
                            alternateSwings = 0;
                        }
                    }
                    else if (hasFired2)
                    {
                        UnityEngine.Object.Instantiate(wideEffectPrefab, bigSnipAir);
                    }
                }
                else
                {
                    if (hasFired && !hasFired2)
                    {
                        if (alternateSwings == 0)
                        {
                            UnityEngine.Object.Instantiate(clawSlash, smallSlash);
                            alternateSwings = 1;
                        }
                        else if (alternateSwings == 1)
                        {
                            UnityEngine.Object.Instantiate(clawSlash, smallSlash2);
                            alternateSwings = 0;
                        }
                    }
                    else if (hasFired2)
                    {
                        UnityEngine.Object.Instantiate(clawSlash, smallSlash2);
                    }

                    UnityEngine.Object.Instantiate(swingEffectPrefab, bigSnip);
                }
            }
        }
        protected virtual void FireAttack()
        {
            hasHopped = false;

            overlapAttack = new OverlapAttack();
            overlapAttack.damageType = damageType;
            if (isInsatiable)
            {
                overlapAttack.AddModdedDamageType(moddedDamageType);
            }
            overlapAttack.AddModdedDamageType(DamageTypes.SeamstressLifesteal);
            overlapAttack.attacker = gameObject;
            overlapAttack.inflictor = gameObject;
            overlapAttack.teamIndex = GetTeam();
            overlapAttack.damage = damageCoefficient * damageStat;
            overlapAttack.procCoefficient = procCoefficient;
            overlapAttack.hitEffectPrefab = hitEffectPrefab;
            overlapAttack.forceVector = bonusForce;
            overlapAttack.pushAwayForce = pushForce;
            overlapAttack.hitBoxGroup = FindHitBoxGroup(hitBoxString);
            overlapAttack.isCrit = RollCrit();

            if (isAuthority)
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
            if (stopwatch >= duration)
            {
                return InterruptPriority.Any;
            }
            return InterruptPriority.PrioritySkill;
        }
        private void PlayAttackAnimation()
        {
            PlayCrossfade("Gesture, Override", "Clip", "Slash.playbackRate", duration * 1.7f, 0.1f * duration);
        }
        public override void OnExit()
        {
            base.OnExit();
        }

    }
}
