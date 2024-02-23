/*
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using R2API;
using SeamstressMod.Survivors.Seamstress;
using SeamstressMod.Modules.BaseStates;
using System;
using EntityStates;
using UnityEngine.Networking;

namespace SeamstressMod.SkillStates
{
    public class Sew : BaseSeamstressSkillState
    {
        protected GameObject projectilePrefab;

        public static GameObject supaEffect = SeamstressAssets.sewEffect;

        private float baseDuration = 0.1f;
        private bool hasFired;
        private Ray aimRay;
        private float duration;
        public override void OnEnter()
        {
            base.OnEnter();
            RefreshState();
            this.duration = (baseDuration / attackSpeedStat);
            UnityEngine.Object.Instantiate<GameObject>(supaEffect, transform);
            if (empowered)
            {
                this.projectilePrefab = SeamstressAssets.needleButcheredPrefab;
            }
            else
            {
                this.projectilePrefab = SeamstressAssets.needlePrefab;
            }
            if(!isGrounded)
            {
                SmallHop(characterMotor, 2f);
            }
            PlayAttackAnimation();
            Util.PlaySound("Play_bandit2_m2_alt_throw", gameObject);
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= duration && !hasFired)
            {
                if(base.isAuthority) 
                {
                    aimRay = base.GetAimRay();
                    ProjectileManager.instance.FireProjectile(this.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), gameObject, damageStat * SeamstressStaticValues.needleDamageCoefficient, 0f, RollCrit(), DamageColorIndex.Default, null, -1f);
                hasFired = true;
                }
                Util.PlaySound("Play_bandit2_m2_alt_throw", gameObject);
            }
            if(base.fixedAge >= duration && hasFired && base.isAuthority) 
            {
                this.outer.SetNextStateToMain();
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (base.fixedAge >= duration)
            {
                return InterruptPriority.Any;
            }
            return InterruptPriority.PrioritySkill;
        }
        protected void PlayAttackAnimation()
        {
            PlayCrossfade("Gesture, Override", "ThrowBomb", "ThrowBomb.playbackRate", this.duration, 0.1f * this.duration);
        }
        public override void OnExit()
        {
            base.OnExit();
        }

    }
}
*/