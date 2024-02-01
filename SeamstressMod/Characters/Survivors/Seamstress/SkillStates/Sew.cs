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
        public GameObject projectilePrefab;
        public float needleDelay;
        public float needleCompareDelay = 0.1f;
        public bool hasLaunched;
        Ray aimRay;
        public float duration;
        public override void OnEnter()
        {
            this.hasLaunched = false;
            base.OnEnter();
            RefreshState();
            this.duration = (this.needleCompareDelay / attackSpeedStat * characterBody.GetBuffCount(SeamstressBuffs.needles));
            if (empowered)
            {
                this.projectilePrefab = SeamstressAssets.needleButcheredPrefab;
                Util.PlaySound("Play_imp_overlord_teleport_end", gameObject);
                UnityEngine.Object.Instantiate<GameObject>(SeamstressAssets.sewButcheredEffect, transform);
            }
            else
            {
                UnityEngine.Object.Instantiate<GameObject>(SeamstressAssets.sewEffect, transform);
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
                    if (characterBody.HasBuff(SeamstressBuffs.needles))
                    {
                        if(base.isAuthority) 
                        {
                            aimRay = base.GetAimRay();
                            ProjectileManager.instance.FireProjectile(this.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), gameObject, damageStat * SeamstressStaticValues.sewNeedleDamageCoefficient, 0f, RollCrit(), DamageColorIndex.Default, null, -1f);
                        }
                        if (NetworkServer.active)
                        {
                            characterBody.RemoveBuff(SeamstressBuffs.needles);
                        }
                        Util.PlaySound("Play_bandit2_m2_alt_throw", gameObject);
                        needleCompareDelay += (0.1f / attackSpeedStat);
                    }
                    else
                    {
                        hasLaunched = true;
                    }
                }
            }
            else if (characterBody.GetBuffCount(SeamstressBuffs.needles) < this.baseNeedleAmount)
            {
                if(NetworkServer.active)
                {
                    characterBody.AddBuff(SeamstressBuffs.needles);
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
