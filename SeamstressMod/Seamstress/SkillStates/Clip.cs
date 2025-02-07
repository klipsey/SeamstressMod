using RoR2;
using RoR2.Projectile;
using UnityEngine;
using R2API;
using SeamstressMod.Modules.BaseStates;
using System;
using EntityStates;
using UnityEngine.Networking;
using SeamstressMod.Seamstress.Content;

using Object = UnityEngine.Object;

namespace SeamstressMod.Seamstress.SkillStates
{
    public class Clip : BaseSeamstressSkillState
    {
        public GameObject clawSlash = SeamstressAssets.clawSlashEffect;

        public GameObject hitEffectPrefab = SeamstressAssets.scissorsHitImpactEffect;

        public GameObject swingEffectPrefab = SeamstressAssets.scissorsSlashComboEffect;

        public GameObject wideEffectPrefab = SeamstressAssets.wideSlashEffect;

        private GameObject swingEffectInstance;
        protected EffectManagerHelper _emh_swingEffectInstance;
        private GameObject swingEffectInstance2;
        protected EffectManagerHelper _emh_swingEffectInstance2;

        protected Animator animator;

        private OverlapAttack overlapAttack;
        private DamageType damageType = DamageType.Generic;
        private DamageAPI.ModdedDamageType moddedDamageType = DamageTypes.CutDamage;
        private DamageAPI.ModdedDamageType moddedDamageType2 = DamageTypes.SeamstressLifesteal;
        private float damageCoefficient = SeamstressConfig.clipDamageCoefficient.Value;
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
            hitPauseTimer -= Time.fixedDeltaTime;

            if (hitPauseTimer <= 0f && inHitPause)
            {
                RemoveHitstop();
            }

            if (!inHitPause)
            {
                stopwatch += Time.fixedDeltaTime;
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
        protected virtual void DetermineSwing()
        {
            Transform transform = FindModelChild("SwingCenter");
            GameObject chosenEffect = clawSlash;

            PlaySwingEffect(transform, chosenEffect);

            if (noScissors)
            {
                chosenEffect = clawSlash;
                if (hasFired && !hasFired2)
                {
                    if (alternateSwings == 0)
                    {
                        transform = FindModelChild("SwingLeftSmall");
                        alternateSwings = 1;
                    }
                    else if (alternateSwings == 1)
                    {
                        transform = FindModelChild("SwingRightSmall");
                        alternateSwings = 0;
                    }
                }
                else if (hasFired2)
                {
                    transform = FindModelChild("SwingRightSmall");
                }
            }
            else if (inAir)
            {
                chosenEffect = wideEffectPrefab;

                if (hasFired && !hasFired2)
                {
                    if (alternateSwings == 0)
                    {
                        transform = FindModelChild("SwingCharAirCenter2");
                        alternateSwings = 1;
                    }
                    else if (alternateSwings == 1)
                    {
                        transform = FindModelChild("SwingCharAirCenter");
                        alternateSwings = 0;
                    }
                }
                else if (hasFired2)
                {
                    transform = FindModelChild("SwingCharAirCenter");
                }
            }
            else
            {
                if (hasFired && !hasFired2)
                {
                    if (alternateSwings == 0)
                    {
                        transform = FindModelChild("SwingLeftSmall");
                        alternateSwings = 1;
                    }
                    else if (alternateSwings == 1)
                    {
                        transform = FindModelChild("SwingRightSmall");
                        alternateSwings = 0;
                    }
                }
                else if (hasFired2)
                {
                    transform = FindModelChild("SwingRightSmall");
                }

                PlaySwingEffect(FindModelChild("SwingCenter"), swingEffectPrefab);
            }

            PlaySwingEffect(transform, chosenEffect);
        }
        protected virtual void PlaySwingEffect(Transform attachedTransform, GameObject Effect)
        {
            if (!EffectManager.ShouldUsePooledEffect(Effect))
            {
                swingEffectInstance = Object.Instantiate(Effect, attachedTransform);
            }
            else
            {
                _emh_swingEffectInstance = EffectManager.GetAndActivatePooledEffect(Effect, attachedTransform, inResetLocal: true);
                swingEffectInstance = _emh_swingEffectInstance.gameObject;
            }
            ScaleParticleSystemDuration component = swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
            if ((bool)component)
            {
                component.newDuration = component.initialDuration;
            }
        }

        protected virtual void PlaySwingEffect2(Transform attachedTransform, GameObject Effect)
        {
            if (attachedTransform)
            {
                if (!EffectManager.ShouldUsePooledEffect(Effect))
                {
                    swingEffectInstance2 = Object.Instantiate(Effect, attachedTransform);
                }
                else
                {
                    _emh_swingEffectInstance2 = EffectManager.GetAndActivatePooledEffect(Effect, attachedTransform, inResetLocal: true);
                    swingEffectInstance2 = _emh_swingEffectInstance2.gameObject;
                }
                ScaleParticleSystemDuration component = swingEffectInstance2.GetComponent<ScaleParticleSystemDuration>();
                if ((bool)component)
                {
                    component.newDuration = component.initialDuration;
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
                overlapAttack.damageType.AddModdedDamageType(moddedDamageType);
            }
            overlapAttack.damageType.AddModdedDamageType(moddedDamageType2);
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
            overlapAttack.damageType.damageSource = DamageSource.Secondary;

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

            DetermineSwing();
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
            PlayCrossfade("Gesture, Additive", "Clip", "Slash.playbackRate", duration * 1.7f, 0.1f * duration);
        }
        public override void OnExit()
        {
            if (base.isAuthority)
            {
                if (_emh_swingEffectInstance != null && _emh_swingEffectInstance.OwningPool != null)
                {
                    _emh_swingEffectInstance.OwningPool.ReturnObject(_emh_swingEffectInstance);
                }
                else if ((bool)swingEffectInstance)
                {
                    EntityState.Destroy(swingEffectInstance);
                }
                swingEffectInstance = null;
                _emh_swingEffectInstance = null;

                if (_emh_swingEffectInstance2 != null && _emh_swingEffectInstance2.OwningPool != null)
                {
                    _emh_swingEffectInstance2.OwningPool.ReturnObject(_emh_swingEffectInstance2);
                }
                else if ((bool)swingEffectInstance2)
                {
                    EntityState.Destroy(swingEffectInstance2);
                }
                swingEffectInstance2 = null;
                _emh_swingEffectInstance2 = null;
            }
            base.OnExit();
        }

    }
}
