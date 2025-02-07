using RoR2;
using UnityEngine;
using R2API;
using SeamstressMod.Modules.BaseStates;
using System;
using System.ComponentModel;
using SeamstressMod.Seamstress.Content;

using Object = UnityEngine.Object;


namespace SeamstressMod.Seamstress.SkillStates
{
    public class Flurry : BaseMeleeAttack
    {
        public override void OnEnter()
        {
            RefreshState();
            hitboxGroupName = "Sword";
            damageType = DamageType.Generic;
            damageSource = DamageSource.Secondary;
            damageTotal = SeamstressConfig.flurryDamageCoefficient.Value;
            procCoefficient = 1f;
            pushForce = 300f;
            bonusForce = Vector3.zero;
            baseDuration = 1.1f;
            baseScissorDuration = 2f;
            //0-1 multiplier of= baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
            //for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds
            attackStartPercentTime = 0.2f;
            attackEndPercentTime = 0.4f;

            //this is the point at which an attack can be interrupted by itself, continuing a combo
            earlyExitPercentTime = 0.5f;
            hitStopDuration = 0.05f;
            attackRecoil = 2 / attackSpeedStat;
            hitHopVelocity = 3.5f;

            swingSoundString = "sfx_seamstress_swing";
            hitSoundString = "";
            hitEffectPrefab = seamstressController.blue ? SeamstressAssets.scissorsHitImpactEffect2 : SeamstressAssets.scissorsHitImpactEffect;
            swingEffectPrefab = seamstressController.blue ? SeamstressAssets.clawSlashEffect2 : SeamstressAssets.clawSlashEffect;
            bonusSwingEffectPrefab = seamstressController.blue ? SeamstressAssets.scissorsSlashEffect2 : SeamstressAssets.scissorsSlashEffect;
            muzzleString = swingIndex % 2 == 0 ? "SwingLeftSmall" : "SwingRightSmall";
            buffer = false;
            if (isInsatiable)
            {
                moddedDamageTypeHolder.Add(DamageTypes.CutDamage);
                moddedDamageTypeHolder.Add(DamageTypes.GainNeedles);
            }
            moddedDamageTypeHolder.Add(DamageTypes.SeamstressLifesteal);
            scissorHit = true;
            if (muzzleString == "SwingLeftSmall" && !scissorRight)
            {
                //change to remove the next states double hit instead
                scissorHit = false;
            }
            if (muzzleString == "SwingRightSmall" && !scissorLeft)
            {
                //change to remove the next states double hit instead
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
            PlayCrossfade("Gesture, Additive", swingIndex % 2 == 0 ? "Slash1" : "Slash2", "Slash.playbackRate", duration, 0.1f * duration);
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