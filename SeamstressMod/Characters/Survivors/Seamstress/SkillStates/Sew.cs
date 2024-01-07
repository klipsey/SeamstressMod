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
        public bool hasLaunched;
        public bool hasLaunched2;
        public bool hasLaunched3;
        Ray aimRay;
        public override void OnEnter()
        {
            RefreshState();
            aimRay = base.GetAimRay();
            projectilePrefab = SeamstressAssets.needlePrefab;
            projectilePrefabEmpowered = SeamstressAssets.needlePrefabEmpowered;
            this.damageType = DamageType.Stun1s;
            this.hitboxName = "SwordSew";
            this.damageCoefficient = SeamstressStaticValues.sewDamageCoefficient;
            this.procCoefficient = 1f;
            this.pushForce = 300f;
            this.bonusForce = Vector3.zero;
            this.baseDuration = 1.5f;

            //0-1 multiplier of= baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
            //for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds
            this.attackStartPercentTime = 0.2f;
            this.attackEndPercentTime = 0.4f;

            //this is the point at which an attack can be interrupted by itself, continuing a combo
            this.earlyExitPercentTime = 0f;

            this.hitStopDuration = 0.2f;
            this.attackRecoil = 0.5f;
            this.hitHopVelocity = 4f;

            this.swingSoundString = "Play_imp_attack";
            this.hitSoundString = "";
            this.hitEffectPrefab = SeamstressAssets.scissorsHitImpactEffect;
            this.swingEffectPrefab = SeamstressAssets.sewSwingEffect;
            this.muzzleString = "SwingCenter";

            this.impactSound = SeamstressAssets.sewHitSoundEvent.index;
            this.moddedDamageType = DamageTypes.CutDamage;
            if (empowered)
            {
                this.damageType |= DamageType.BleedOnHit;
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
            needleDelay += Time.fixedDeltaTime;
            if (empowered)
            {
                if (needleDelay >= 0.2f && !hasLaunched)
                {
                    ProjectileManager.instance.FireProjectile(projectilePrefabEmpowered, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * SeamstressStaticValues.sewNeedleDamageCoefficient, 600f, base.RollCrit(), DamageColorIndex.Default, null, -1f);
                    hasLaunched = true;
                }
                    if (needleDelay >= 0.3f && !hasLaunched2)
                {
                    ProjectileManager.instance.FireProjectile(projectilePrefabEmpowered, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * SeamstressStaticValues.sewNeedleDamageCoefficient, 600f, base.RollCrit(), DamageColorIndex.Default, null, -1f);
                    hasLaunched2 = true;
                }
                if (needleDelay >= 0.4f && !hasLaunched3)
                {
                    ProjectileManager.instance.FireProjectile(projectilePrefabEmpowered, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * SeamstressStaticValues.sewNeedleDamageCoefficient, 600f, base.RollCrit(), DamageColorIndex.Default, null, -1f);
                    hasLaunched3 = true;
                }
            }
            else
            {
                if (needleDelay >= 0.2f && !hasLaunched)
                {
                    ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * SeamstressStaticValues.sewNeedleDamageCoefficient, 600f, base.RollCrit(), DamageColorIndex.Default, null, -1f);
                    hasLaunched = true;
                }
            }
        }
        protected override void FireAttack()
        {
            if (isAuthority)
            {
                Vector3 direction = GetAimRay().direction;
                direction.y = Mathf.Max(direction.y, direction.y * 0.5f);
                FindModelChild("SwingPivot").rotation = Util.QuaternionSafeLookRotation(direction);
                if (attack.Fire())
                {
                    OnHitEnemyAuthority();
                }
            }
        }

        protected override void PlaySwingEffect()
        {
            Util.PlaySound(swingSoundString, gameObject);
            if (!swingEffectPrefab)
            {
                return;
            }
            Transform transform = FindModelChild(this.muzzleString);
            if ((bool)transform)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate(swingEffectPrefab, transform);
                ScaleParticleSystemDuration component = gameObject.GetComponent<ScaleParticleSystemDuration>();
                if ((bool)component)
                {
                    component.newDuration = component.initialDuration;
                }
            }
        }
        protected override void PlayAttackAnimation()
        {
            PlayCrossfade("Gesture, Override", "Slash1", "Slash.playbackRate", this.duration, 0.1f * duration);
        }
        public override void OnExit()
        {
            base.OnExit();
        }

    }
}
