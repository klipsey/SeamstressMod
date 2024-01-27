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
    public class ReapRecast: BaseMeleeAttack
    {
        SeamstressController controller;
        public override void OnEnter()
        {
            this.RefreshState();
            this.hitboxGroupName = "Sew";
            this.moddedDamageType = DamageTypes.CutDamage;
            this.procCoefficient = 1f;
            this.pushForce = 300;
            this.bonusForce = Vector3.zero;
            this.baseDuration = 1f;

            //0-1 multiplier of= baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
            //for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds
            this.attackStartPercentTime = 0f;
            this.attackEndPercentTime = 0.2f;

            //this is the point at which an attack can be interrupted by itself, continuing a combo
            this.earlyExitPercentTime = 0.2f;

            this.hitStopDuration = 0.15f;
            this.attackRecoil = 0f;
            this.hitHopVelocity = 0f;
            this.swingSoundString = "Play_voidman_m2_explode";
            this.hitSoundString = "";
            this.hitEffectPrefab = SeamstressAssets.scissorsHitImpactEffect;

            this.muzzleString = "SewCenter";
            this.impactSound = SeamstressAssets.sewHitSoundEvent.index;
            this.hitEffectPrefab = SeamstressAssets.scissorsButcheredHitImpactEffect;
            this.isFlatDamage = true;
            this.damageTotal = characterBody.GetComponent<SeamstressController>().butcheredConversion;
            UnityEngine.Object.Instantiate<GameObject>(SeamstressAssets.expungeEffect, transform);

            if (!base.characterMotor.isGrounded)
            {
                SmallHop(base.characterMotor, 6f);
            }
            controller = characterBody.GetComponent<SeamstressController>();
            base.OnEnter();
            skillLocator.utility = skillLocator.FindSkill("Utility");
        }
        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
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

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (stopwatch >= duration * earlyExitPercentTime)
            {
                return InterruptPriority.Vehicle;
            }
            return InterruptPriority.Vehicle;
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