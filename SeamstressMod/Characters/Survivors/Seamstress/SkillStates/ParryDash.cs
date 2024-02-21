using RoR2;
using UnityEngine;
using R2API;
using SeamstressMod.Survivors.Seamstress;
using SeamstressMod.Modules.BaseStates;
using System;
using static UnityEngine.ParticleSystem.PlaybackState;


namespace SeamstressMod.SkillStates
{
    public class ParryDash : BaseMeleeAttack
    {
        private GameObject swingEffectInstance;

        protected bool first = true;
        public override void OnEnter()
        {
            RefreshState();
            hitboxGroupName = "Weave";
            damageType = DamageType.Generic;
            damageTotal = SeamstressStaticValues.parryDamage;
            procCoefficient = 1f;
            pushForce = 200f;
            bonusForce = Vector3.zero;
            baseDuration = 0.1f;
            moddedDamageType = DamageTypes.StitchDamage;
            moddedDamageType2 = DamageTypes.Empty;
            moddedDamageType3 = DamageTypes.Empty;
            //0-1 multiplier of= baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
            //for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds
            attackStartPercentTime = 0f;
            attackEndPercentTime = 1f;

            //this is the point at which an attack can be interrupted by itself, continuing a combo
            earlyExitPercentTime = 1f;
            hitStopDuration = 0.1f;
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
            if (isGrounded)
            {
                float dashVector = 30f;
                if (this.inputBank.moveVector != Vector3.zero) this.characterMotor.velocity += this.characterDirection.forward * dashVector;
                else
                {
                    dashVector = 15f;
                    this.characterMotor.velocity += this.GetAimRay().direction * dashVector;
                }

            }
            else if (!isGrounded)
            {
                float dashVector = 20f;
                this.characterMotor.velocity += this.GetAimRay().direction * dashVector;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
        protected override void PlayAttackAnimation()
        {
            PlayCrossfade("Gesture, Override", "Slash1", "Slash.playbackRate", this.duration + (0.9f / attackSpeedStat), 0.1f * (this.duration + (0.9f / attackSpeedStat)));
        }
        protected override void FireAttack()
        {
            if (base.isAuthority)
            {
                Vector3 direction = GetAimRay().direction;
                direction.y = Mathf.Max(direction.y, direction.y * 0.5f);
                FindModelChild("SwingPivot").rotation = Util.QuaternionSafeLookRotation(direction);
            }
            base.FireAttack();
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
            if(first)
            {
                outer.SetNextState(new ParrySecondSlash());
            }
            base.OnExit();
        }

    }

}