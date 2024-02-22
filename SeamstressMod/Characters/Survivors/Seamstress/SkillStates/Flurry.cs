using RoR2;
using UnityEngine;
using R2API;
using SeamstressMod.Survivors.Seamstress;
using SeamstressMod.Modules.BaseStates;
using System;
using System.ComponentModel;


namespace SeamstressMod.SkillStates
{   
    public class Flurry : BaseMeleeAttack
    {
        private GameObject swingEffectInstance;

        private GameObject chargeEffectInstance;

        public static GameObject chargeEffectPrefab = SeamstressAssets.flurryCharge;
        public override void OnEnter()
        {
            RefreshState();
            hitboxGroupName = "Sword";
            damageType = DamageType.Generic;
            damageTotal = SeamstressStaticValues.flurryDamageCoefficient;
            procCoefficient = 1f;
            pushForce = 600f;
            bonusForce = Vector3.zero;
            baseDuration = 2f;
            moddedDamageType = DamageTypes.StitchDamage;
            moddedDamageType2 = DamageTypes.Empty;
            moddedDamageType3 = DamageTypes.Empty;
            //0-1 multiplier of= baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
            //for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds
            attackStartPercentTime = 0.4f;
            attackEndPercentTime = 0.6f;

            //this is the point at which an attack can be interrupted by itself, continuing a combo
            earlyExitPercentTime = 0.85f;
            hitStopDuration = 0.1f;
            attackRecoil = 0.75f;
            hitHopVelocity = 3.5f;

            swingSoundString = "Play_moonBrother_swing_horizontal";
            hitSoundString = "";
            hitEffectPrefab = SeamstressAssets.scissorsHitImpactEffect;
            swingEffectPrefab = SeamstressAssets.scissorsSwingEffect;
            muzzleString = swingIndex % 2 == 0 ? "SwingLeft" : "SwingRight";
            buffer = true;
            if (empowered)
            {
                moddedDamageType2 = DamageTypes.CutDamage;
            }
            impactSound = SeamstressAssets.scissorsHitSoundEvent.index;

            base.OnEnter();
            Util.PlayAttackSpeedSound("Play_imp_overlord_attack2_tell", gameObject, duration * attackStartPercentTime);
            Transform transform = FindModelChild(this.muzzleString);
            if (transform && chargeEffectPrefab)
            {
                chargeEffectInstance = UnityEngine.Object.Instantiate(chargeEffectPrefab, transform.position, transform.rotation);
                chargeEffectInstance.transform.parent = transform;
            }
        }

        protected override void FireAttack()
        {
            if (base.isAuthority)
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
            PlayCrossfade("Gesture, Override", "ThrowBomb", "ThrowBomb.playbackRate", attackStartPercentTime * duration, 0.1f * duration);
        }

        protected override void PlayTrueAttackAnimation()
        {
            Destroy(chargeEffectInstance);
            PlayCrossfade("Gesture, Override", swingIndex % 2 == 0 ? "Slash1" : "Slash2", "Slash.playbackRate", duration * (earlyExitPercentTime - attackStartPercentTime), 0.1f * duration);
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

    }

}