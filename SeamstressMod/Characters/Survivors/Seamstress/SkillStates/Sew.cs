using RoR2;
using RoR2.Projectile;
using UnityEngine;
using R2API;
using SeamstressMod.Survivors.Seamstress;
using SeamstressMod.SkillStates.BaseStates;
using System;

namespace SeamstressMod.SkillStates
{
    public class Sew : BaseMeleeAttack
    {
        public GameObject projectilePrefab;
        public GameObject projectilePrefabEmpowered;
        public float needleDelay;
        public float needleCompareDelay = 0.1f;
        public bool hasLaunched;
        public bool hasLaunched2;
        public bool hasLaunched3;
        Ray aimRay;
        public override void OnEnter()
        {
            RefreshState();
            aimRay = base.GetAimRay();
            this.hitboxName = "Sew";
            this.damageCoefficient = SeamstressStaticValues.sewDamageCoefficient;
            this.procCoefficient = 1f;
            this.pushForce = 300;
            this.bonusForce = Vector3.zero;
            this.baseDuration = 1.5f;

            //0-1 multiplier of= baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
            //for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds
            this.attackStartPercentTime = 0.15f;
            this.attackEndPercentTime = 0.5f;

            //this is the point at which an attack can be interrupted by itself, continuing a combo
            this.earlyExitPercentTime = 0f;

            this.hitStopDuration = 0f;
            this.attackRecoil = 0f;
            this.hitHopVelocity = 0f;
            this.swingSoundString = "Play_voidman_m2_explode";
            this.hitSoundString = "";
            this.hitEffectPrefab = SeamstressAssets.scissorsHitImpactEffect;

            this.muzzleString = "SewCenter";
            this.moddedDamageType = DamageTypes.Empty;
            this.impactSound = SeamstressAssets.sewHitSoundEvent.index;
            if (empowered)
            {
                projectilePrefab = SeamstressAssets.needleButcheredPrefab;
                this.swingEffectPrefab = SeamstressAssets.sewButcheredEffect;
                this.hitEffectPrefab = SeamstressAssets.scissorsButcheredHitImpactEffect;
                Util.CleanseBody(base.characterBody, removeDebuffs: true, removeBuffs: false, removeCooldownBuffs: true, removeDots: true, removeStun: true, removeNearbyProjectiles: true);
            }
            else
            {
                projectilePrefab = SeamstressAssets.needlePrefab;
                this.swingEffectPrefab = SeamstressAssets.sewEffect;
                this.hitEffectPrefab = SeamstressAssets.scissorsHitImpactEffect;
            }
            base.OnEnter();

        }
        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            aimRay = base.GetAimRay();
            needleDelay += Time.fixedDeltaTime;
            if (needleDelay >= needleCompareDelay / attackSpeedStat)
            {
                if(characterBody.HasBuff(SeamstressBuffs.needles))
                {
                    ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * SeamstressStaticValues.sewNeedleDamageCoefficient, 600f, base.RollCrit(), DamageColorIndex.Default, null, -1f);
                    Util.PlaySound("Play_bandit2_m2_alt_throw", base.gameObject);
                    characterBody.RemoveBuff(SeamstressBuffs.needles);
                    needleCompareDelay += (0.1f / attackSpeedStat);
                }
            }
        }
        protected override void FireAttack()
        {
            if (isAuthority)
            {
                if (attack.Fire())
                {
                    OnHitEnemyAuthority();
                }
            }
        }
        protected override void PlaySwingEffect()
        {
            if (!swingEffectPrefab)
            {
                return;
            }
            Transform transform = FindModelChild(this.muzzleString);
            if ((bool)transform)
            {
                UnityEngine.Object.Instantiate(swingEffectPrefab, transform);
            }
        }
        protected override void PlayAttackAnimation()
        {
            PlayCrossfade("Gesture, Override", "ThrowBomb", "ThrowBomb.playbackRate", this.duration, 0.1f * duration);
        }
        public override void OnExit()
        {
            base.OnExit();
        }

    }
}
