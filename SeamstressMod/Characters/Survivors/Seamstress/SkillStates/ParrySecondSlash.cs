using RoR2;
using UnityEngine;
using R2API;
using SeamstressMod.Survivors.Seamstress;
using SeamstressMod.Modules.BaseStates;
using System;
using static UnityEngine.ParticleSystem.PlaybackState;
using EntityStates;


namespace SeamstressMod.SkillStates
{
    public class ParrySecondSlash : ParryDash
    {
        private GameObject swingEffectInstance;
        public override void OnEnter()
        {
            RefreshState();
            first = false;
            hitboxGroupName = "Weave";
            damageType = DamageType.Generic;
            damageTotal = SeamstressStaticValues.parryDamage;
            procCoefficient = 1f;
            pushForce = 200f;
            bonusForce = Vector3.zero;
            baseDuration = 0.9f;
            moddedDamageType = DamageTypes.StitchDamage;
            moddedDamageType2 = DamageTypes.Empty;
            moddedDamageType3 = DamageTypes.Empty;
            //0-1 multiplier of= baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
            //for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds
            attackStartPercentTime = 0f;
            attackEndPercentTime = 0.4f;

            //this is the point at which an attack can be interrupted by itself, continuing a combo
            earlyExitPercentTime = 0.6f;
            hitStopDuration = 0.2f;
            attackRecoil = 0.75f;
            hitHopVelocity = 3.5f;
            swingSoundString = "Play_imp_attack";
            hitSoundString = "";
            hitEffectPrefab = SeamstressAssets.scissorsHitImpactEffect;
            swingEffectPrefab = SeamstressAssets.parrySlashEffect;
            muzzleString = "SwingCenter";
            if (empowered)
            {
                moddedDamageType2 = DamageTypes.CutDamage;
            }
            impactSound = SeamstressAssets.scissorsHitSoundEvent.index;
            base.OnEnter();
        }
        protected override void PlayAttackAnimation()
        {
        }
        protected override void PlaySwingEffect()
        {
            if (!this.swingEffectPrefab)
            {
                return;
            }
            Transform transform = FindModelChild(this.muzzleString);
            if (transform)
            {
                this.swingEffectInstance = UnityEngine.Object.Instantiate<GameObject>(this.swingEffectPrefab, transform);
                ScaleParticleSystemDuration scale = this.swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                if (scale) scale.newDuration = scale.initialDuration + (scale.initialDuration * (earlyExitPercentTime - attackStartPercentTime));
            }
        }
        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
        }
        public override void OnExit()
        {
            base.OnExit();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

    }

}