﻿using RoR2;
using UnityEngine;
using R2API;
using SeamstressMod.Survivors.Seamstress;
using SeamstressMod.SkillStates.BaseStates;


namespace SeamstressMod.SkillStates
{
    public class Trim : BaseMeleeAttack
    {
        private bool isComboFinisher => swingIndex == 2;

        public override void OnEnter()
        {
            RefreshState();
            this.hitboxName = "Sword";

            this.damageType = DamageType.Generic;
            this.damageCoefficient = SeamstressStaticValues.scissorsDamageCoefficient;
            this.procCoefficient = 1f;
            this.pushForce = 300f;
            this.bonusForce = Vector3.zero;
            this.baseDuration = 1f;

            //0-1 multiplier of= baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
            //for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds
            this.attackStartPercentTime = 0.2f;
            this.attackEndPercentTime = 0.4f;

            //this is the point at which an attack can be interrupted by itself, continuing a combo
            this.earlyExitPercentTime = 0.6f;

            this.hitStopDuration = 0.012f;
            this.attackRecoil = 0.5f;
            this.hitHopVelocity = 4f;

            this.swingSoundString = "Play_imp_attack";
            this.hitSoundString = "";
            this.hitEffectPrefab = SeamstressAssets.scissorsHitImpactEffect;
            this.swingEffectPrefab = SeamstressAssets.scissorsSwingEffect;

            switch (swingIndex)
            {
                case 0:
                    this.muzzleString = "SwingLeft";
                    break;
                case 1:
                    this.muzzleString = "SwingRight";
                    break;
                case 2:
                    this.muzzleString = "SwingCenter";
                    break;
            }
            if (isComboFinisher)
            {
                this.moddedDamageType = DamageTypes.CutDamage;
                if (empowered) 
                {
                    this.damageType = DamageType.BleedOnHit;
                    healthComponent.AddBarrier(healthComponent.health * 0.1f);
                }
                this.swingSoundString = "Play_bandit2_m2_slash";
                this.hitboxName = "SwordBig";
                this.swingEffectPrefab = SeamstressAssets.scissorsComboSwingEffect;
                this.baseDuration = 1.4f;
            }

            this.impactSound = SeamstressAssets.scissorsHitSoundEvent.index;

            base.OnEnter();
        }
        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
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
            switch (swingIndex)
            {
                case 0:
                    PlayCrossfade("Gesture, Override", "Slash1", "Slash.playbackRate", this.duration, 0.1f * duration);
                    break;
                case 1:
                    PlayCrossfade("Gesture, Override", "Slash2", "Slash.playbackRate", this.duration, 0.1f * duration);
                    break;
                case 2:
                    PlayCrossfade("Gesture, Override", "Slash1", "Slash.playbackRate", this.duration, 0.1f * duration);
                    break;
            }
        }
        public override void OnExit()
        {
            base.OnExit();
        }

    }

}