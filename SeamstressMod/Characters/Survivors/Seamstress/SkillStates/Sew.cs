using RoR2;
using RoR2.Projectile;
using UnityEngine;
using R2API;
using SeamstressMod.Survivors.Seamstress;
using SeamstressMod.Modules.BaseStates;
using System;
using EntityStates;

namespace SeamstressMod.SkillStates
{
    public class Sew : BaseSeamstressSkillState
    {
        public GameObject projectilePrefab;
        public float needleDelay;
        public float needleCompareDelay = 0.1f;
        public bool hasLaunched;
        Ray aimRay;
        public float duration;
        public override void OnEnter()
        {
            this.RefreshState();
            duration = (this.needleCompareDelay / base.attackSpeedStat * base.characterBody.GetBuffCount(SeamstressBuffs.needles));
            aimRay = base.GetAimRay();
            hasLaunched = false;
            if (empowered)
            {
                this.projectilePrefab = SeamstressAssets.needleButcheredPrefab;
                Util.PlaySound("Play_imp_overlord_teleport_end", base.gameObject);
                UnityEngine.Object.Instantiate<GameObject>(SeamstressAssets.sewButcheredEffect, transform);
            }
            else
            {
                UnityEngine.Object.Instantiate<GameObject>(SeamstressAssets.sewEffect, transform);
                this.projectilePrefab = SeamstressAssets.needlePrefab;
            }
            if(!base.characterMotor.isGrounded)
            {
                SmallHop(base.characterMotor, 6f);
            }
            PlayAttackAnimation();
            base.OnEnter();
            ProjectileManager.instance.FireProjectile(projectilePrefab, this.aimRay.origin, Util.QuaternionSafeLookRotation(this.aimRay.direction), base.gameObject, this.damageStat * SeamstressStaticValues.sewNeedleDamageCoefficient, 0f, base.RollCrit(), DamageColorIndex.Default, null, -1f);
            Util.PlaySound("Play_bandit2_m2_alt_throw", base.gameObject);
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.aimRay = base.GetAimRay();
            this.needleDelay += Time.fixedDeltaTime;
            if (!hasLaunched)
            {
                if (this.needleDelay >= this.needleCompareDelay / base.attackSpeedStat)
                {
                    if (base.characterBody.HasBuff(SeamstressBuffs.needles))
                    {
                        ProjectileManager.instance.FireProjectile(projectilePrefab, this.aimRay.origin, Util.QuaternionSafeLookRotation(this.aimRay.direction), base.gameObject, this.damageStat * SeamstressStaticValues.sewNeedleDamageCoefficient, 0f, base.RollCrit(), DamageColorIndex.Default, null, -1f);
                        Util.PlaySound("Play_bandit2_m2_alt_throw", base.gameObject);
                        base.characterBody.RemoveBuff(SeamstressBuffs.needles);
                        this.needleCompareDelay += (0.1f / base.attackSpeedStat);
                    }
                    else
                    {
                        hasLaunched = true;
                    }
                }
            }
            else if (base.characterBody.GetBuffCount(SeamstressBuffs.needles) < baseNeedleAmount)
            {
                base.characterBody.AddBuff(SeamstressBuffs.needles);
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (this.needleDelay >= duration)
            {
                return InterruptPriority.Any;
            }
            return InterruptPriority.PrioritySkill;
        }
        protected void PlayAttackAnimation()
        {
            PlayCrossfade("Gesture, Override", "ThrowBomb", "ThrowBomb.playbackRate", this.duration, 0.1f * duration);
        }
        public override void OnExit()
        {
            base.OnExit();
        }

    }
}
