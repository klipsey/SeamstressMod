using RoR2;
using UnityEngine;
using R2API;
using SeamstressMod.Survivors.Seamstress;
using SeamstressMod.Modules.BaseStates;
using System;


namespace SeamstressMod.SkillStates
{   
    public class Flurry : BaseMeleeAttack
    {
        public override void OnEnter()
        {
            this.RefreshState();
            this.hitboxGroupName = "Sword";
            this.damageType = DamageType.Generic;
            this.damageTotal = SeamstressStaticValues.flurryDamageCoefficient;
            this.procCoefficient = 1f;
            this.pushForce = 300f;
            this.bonusForce = Vector3.zero;
            this.baseDuration = 1f;
            this.moddedDamageType = DamageTypes.StitchDamage;
            this.moddedDamageType2 = DamageTypes.Empty;
            this.moddedDamageType3 = DamageTypes.Empty;
            //0-1 multiplier of= baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
            //for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds
            this.attackStartPercentTime = 0.2f;
            this.attackEndPercentTime = 0.4f;

            //this is the point at which an attack can be interrupted by itself, continuing a combo
            this.earlyExitPercentTime = 0.5f;
            this.hitStopDuration = 0.05f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 3.5f;

            this.swingSoundString = "Play_imp_attack";
            this.hitSoundString = "";
            this.hitEffectPrefab = SeamstressAssets.scissorsHitImpactEffect;
            this.swingEffectPrefab = SeamstressAssets.scissorsSwingEffect;
            muzzleString = swingIndex % 2 == 0 ? "SwingLeft" : "SwingRight";
            if (empowered)
            {
                this.moddedDamageType = DamageTypes.StitchDamageFlurry;
                this.swingEffectPrefab = SeamstressAssets.scissorsButcheredSwingEffect;
                this.hitEffectPrefab = SeamstressAssets.scissorsButcheredHitImpactEffect;
            }
            this.impactSound = SeamstressAssets.scissorsHitSoundEvent.index;

            base.OnEnter();
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
        protected override void PlayAttackAnimation()
        {

            PlayCrossfade("Gesture, Override", swingIndex % 2 == 0 ? "Slash1" : "Slash2", "Slash.playbackRate", this.duration, 0.1f * duration);
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
        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
        }
        public override void OnExit()
        {
            base.OnExit();
        }

    }

}