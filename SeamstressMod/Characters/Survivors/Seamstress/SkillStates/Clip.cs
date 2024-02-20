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

namespace SeamstressMod.SkillStates
{
    public class Clip : BaseSeamstressSkillState
    {
        public static GameObject supaEffect = SeamstressAssets.scissorsButcheredSwingEffect;

        public static GameObject boringEffect = SeamstressAssets.scissorsSwingEffect;

        private GameObject hitEffectPrefab = SeamstressAssets.scissorsHitImpactEffect;

        private OverlapAttack overlapAttack;
        private DamageType damageType = DamageType.Generic;
        private DamageAPI.ModdedDamageType moddedDamageType2 = DamageTypes.PlanarLifeSteal;
        private DamageAPI.ModdedDamageType moddedDamageType = DamageTypes.CutDamage;
        private float damageCoefficient;
        private float procCoefficient = 0.7f;
        private float pushForce = 0f;
        private Vector3 bonusForce = Vector3.zero;

        private int snips;

        private float longWindUp = 0.2f;

        private float shortWindUp = 0.05f;

        private float snipDuration = 0.05f;

        private float duration;

        private float currentDuration = 0f;

        private bool hasFired;
        public override void OnEnter()
        {
            SnipMath();
            Log.Debug("Your damage coefficient is " + damageCoefficient + " and your snip count is " + snips);
            base.OnEnter();
            longWindUp /= attackSpeedStat;
            shortWindUp /= attackSpeedStat;
            snipDuration /= attackSpeedStat;
            if (snips <= 2) duration = (snips * longWindUp);
            else duration = 2 * longWindUp + ((snips - 2) * shortWindUp);
            duration += snipDuration * snips;
            currentDuration = longWindUp;
            base.skillLocator.secondary.stock = 0;
            StartAimMode(0.5f + duration, false);

            if (!isGrounded)
            {
                SmallHop(characterMotor, 2f);
            }
            PlayAttackAnimation();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            //if higher than 1 snip, fire once 
            if(base.fixedAge > currentDuration && snips >= 2 && !hasFired && base.isAuthority) 
            {
                if(snips == 2)
                {
                    currentDuration += snipDuration + longWindUp;
                }
                else
                {
                    currentDuration += snipDuration + shortWindUp;
                }
                snips--;
                hasFired = true;
                EnterAttack();
                FireAttack();
            }
            //if equal to 2-4 snips snip until 1 snip left
            if (base.fixedAge > currentDuration && snips >= 2 && base.isAuthority)
            {
                if(snips == 2)
                {
                    currentDuration += snipDuration + longWindUp;
                }
                else
                {
                    currentDuration += snipDuration + shortWindUp;
                }
                snips--;
                EnterAttack();
                FireAttack();
            }
            //if only 1 snip left, fire
            if(base.fixedAge > currentDuration && snips == 1 && base.isAuthority)
            {
                snips--;
                currentDuration = duration;
                EnterAttack();
                FireAttack();
            }
            if (currentDuration == duration && snips == 0 && base.isAuthority) 
            {
                outer.SetNextStateToMain();
            }
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
            PlaySwingEffect();
        }
        protected virtual void PlaySwingEffect()
        {
            if (!supaEffect || !boringEffect)
            {
                Log.Debug("Error, no effect?");
                return;
            }
            Transform transform = FindModelChild("SwingCenter");
            if (transform)
            {
                if (empowered)
                {
                    UnityEngine.Object.Instantiate(supaEffect, transform);
                }
                else
                {
                    UnityEngine.Object.Instantiate(boringEffect, transform);
                }
            }
        }
        protected virtual void FireAttack()
        {
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
            Vector3 direction = GetAimRay().direction;
            direction.y = Mathf.Max(direction.y, direction.y * 0.5f);
            FindModelChild("SwingPivot").rotation = Util.QuaternionSafeLookRotation(direction);
            overlapAttack.Fire();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (base.fixedAge >= this.duration)
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
