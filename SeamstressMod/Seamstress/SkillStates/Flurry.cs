using RoR2;
using UnityEngine;
using R2API;
using SeamstressMod.Modules.BaseStates;
using System;
using System.ComponentModel;
using SeamstressMod.Seamstress.Content;


namespace SeamstressMod.Seamstress.SkillStates
{
    public class Flurry : BaseMeleeAttack
    {
        public override void OnEnter()
        {
            RefreshState();
            hitboxGroupName = "Sword";
            damageType = DamageType.Generic;
            damageTotal = SeamstressStaticValues.flurryDamageCoefficient;
            procCoefficient = 1f;
            pushForce = 300f;
            bonusForce = Vector3.zero;
            baseDuration = 1.1f - 1.1f * (0.5f * (seamCon.fiendMeter / (healthComponent.fullHealth * SeamstressStaticValues.maxFiendGaugeCoefficient)));
            baseScissorDuration = 2f - 2f * (0.5f * (seamCon.fiendMeter / (healthComponent.fullHealth * SeamstressStaticValues.maxFiendGaugeCoefficient)));
            //0-1 multiplier of= baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
            //for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds
            attackStartPercentTime = 0.2f;
            attackEndPercentTime = 0.4f;

            //this is the point at which an attack can be interrupted by itself, continuing a combo
            earlyExitPercentTime = 0.5f;
            hitStopDuration = 0.05f;
            attackRecoil = 2 / attackSpeedStat;
            hitHopVelocity = 3.5f;

            swingSoundString = "Play_acrid_m1_slash";
            hitSoundString = "";
            hitEffectPrefab = SeamstressAssets.scissorsHitImpactEffect;
            swingEffectPrefab = SeamstressAssets.slashEffect;
            bonusSwingEffectPrefab = SeamstressAssets.scissorsSwingEffect;
            muzzleString = swingIndex % 2 == 0 ? "SwingLeftSmall" : "SwingRightSmall";
            buffer = false;
            if (butchered)
            {
                moddedDamageTypeHolder.Add(DamageTypes.CutDamage);
                moddedDamageTypeHolder.Add(DamageTypes.InsatiableLifeSteal);
            }
            scissorHit = true;
            if (muzzleString == "SwingLeftSmall" && !scissorLeft)
            {
                //change to remove the next states double hit instead
                moddedDamageTypeHolder.Add(DamageTypes.NoScissors);
                scissorHit = false;
            }
            if (muzzleString == "SwingRightSmall" && !scissorRight)
            {
                //change to remove the next states double hit instead
                moddedDamageTypeHolder.Add(DamageTypes.NoScissors);
                scissorHit = false;
            }
            impactSound = SeamstressAssets.scissorsHitSoundEvent.index;

            base.OnEnter();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
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
            PlayCrossfade("Gesture, Override", swingIndex % 2 == 0 ? "Slash1" : "Slash2", "Slash.playbackRate", duration, 0.1f * duration);
        }
        protected override void PlaySwingEffect()
        {
            if (!swingEffectPrefab)
            {
                return;
            }
            Transform transform = FindModelChild(muzzleString);
            if (transform)
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