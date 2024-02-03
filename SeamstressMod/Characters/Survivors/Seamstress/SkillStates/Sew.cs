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

        public static GameObject supaEffect = SeamstressAssets.sewButcheredEffect;

        public static GameObject boringEffect = SeamstressAssets.sewEffect;

        private float needleCompareDelay = 0.1f;
        private float needleDelay;
        private bool hasLaunched;
        private Ray aimRay;
        private float duration;
        public override void OnEnter()
        {
            base.OnEnter();
            RefreshState();
            this.duration = (needleCompareDelay / attackSpeedStat * characterBody.GetBuffCount(SeamstressBuffs.needles));
            if (empowered)
            {
                this.projectilePrefab = SeamstressAssets.needleButcheredPrefab;
                Util.PlaySound("Play_imp_overlord_teleport_end", gameObject);
                UnityEngine.Object.Instantiate<GameObject>(supaEffect, transform);
            }
            else
            {
                UnityEngine.Object.Instantiate<GameObject>(boringEffect, transform);
                this.projectilePrefab = SeamstressAssets.needlePrefab;
            }
            PlayAttackAnimation();
            if(base.isAuthority) 
            {
                aimRay = base.GetAimRay();
                ProjectileManager.instance.FireProjectile(this.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), gameObject, damageStat * SeamstressStaticValues.sewNeedleDamageCoefficient, 0f, RollCrit(), DamageColorIndex.Default, null, -1f);
            }
            if (!base.characterMotor.isGrounded)
            {
                SmallHop(base.characterMotor, 6f);
            }
            Util.PlaySound("Play_bandit2_m2_alt_throw", gameObject);
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            needleDelay += Time.fixedDeltaTime;
            if (!hasLaunched)
            {
                if (needleDelay >= needleCompareDelay / attackSpeedStat)
                {
                    if (base.characterBody.HasBuff(SeamstressBuffs.needles))
                    {
                        if(base.isAuthority) 
                        {
                            aimRay = base.GetAimRay();
                            ProjectileManager.instance.FireProjectile(this.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), gameObject, damageStat * SeamstressStaticValues.sewNeedleDamageCoefficient, 0f, RollCrit(), DamageColorIndex.Default, null, -1f);
                        }
                        base.characterBody.RemoveBuff(SeamstressBuffs.needles);
                        Log.Debug("I SHOT MY FIRST CUM");
                        Util.PlaySound("Play_bandit2_m2_alt_throw", gameObject);
                        needleCompareDelay += (0.1f / attackSpeedStat);
                    }
                    else
                    {
                        hasLaunched = true;
                    }
                }
            }
            if(base.fixedAge >= duration && base.isAuthority) 
            {
                this.outer.SetNextStateToMain();
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (needleDelay >= this.duration)
            {
                return InterruptPriority.Any;
            }
            return InterruptPriority.Frozen;
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
