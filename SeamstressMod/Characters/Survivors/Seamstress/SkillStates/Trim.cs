using RoR2;
using UnityEngine;
using R2API;
using SeamstressMod.Survivors.Seamstress;
using SeamstressMod.Modules.BaseStates;
using System;
using static R2API.DamageAPI;


namespace SeamstressMod.SkillStates
{
    public class Trim : BaseMeleeAttack
    {
        public override void OnEnter()
        {
            RefreshState();
            hitboxGroupName = "Sword";
            damageType = DamageType.Generic;
            damageTotal = SeamstressStaticValues.trimDamageCoefficient;
            procCoefficient = 1f;
            pushForce = 300f;
            bonusForce = Vector3.zero;
            baseDuration = 1.1f - (1.1f * (0.5f * (seamCon.ImpGaugeAmount() / (healthComponent.fullHealth * SeamstressStaticValues.maxFiendGaugeCoefficient))));
            baseScissorDuration = 1.8f - (1.8f * (0.5f * (seamCon.ImpGaugeAmount() / (healthComponent.fullHealth * SeamstressStaticValues.maxFiendGaugeCoefficient))));
            //0-1 multiplier of= baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
            //for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds
            attackStartPercentTime = 0.2f;
            attackEndPercentTime = 0.4f;

            //this is the point at which an attack can be interrupted by itself, continuing a combo
            earlyExitPercentTime = 0.5f;
            hitStopDuration = 0.05f;
            attackRecoil = 2 / attackSpeedStat;
            hitHopVelocity = 5f;

            swingSoundString = "Play_imp_attack";
            hitSoundString = "";
            hitEffectPrefab = SeamstressAssets.scissorsHitImpactEffect;
            swingEffectPrefab = SeamstressAssets.clipSlashEffect;
            bonusSwingEffectPrefab = SeamstressAssets.scissorsSwingEffect;
            scissorHit = true;
            switch (swingIndex)
            {
                case 0:
                    muzzleString = "SwingLeftSmall";
                    if (!scissorRight)
                    {
                        moddedDamageTypeHolder.Add(DamageTypes.NoSword);
                        scissorHit = false;
                    }
                    break;
                case 1:
                    muzzleString = "SwingRightSmall";
                    if (!scissorLeft)
                    {
                        moddedDamageTypeHolder.Add(DamageTypes.NoSword);
                        scissorHit = false;
                    }
                    break;
                case 2:
                    damageTotal = SeamstressStaticValues.trimThirdDamageCoefficient;
                    bonusSwingEffectPrefab = SeamstressAssets.scissorsComboSwingEffect;
                    muzzleString = "SwingCenterSmall";
                    earlyExitPercentTime = 0.75f;
                    attackEndPercentTime = 0.65f;
                    if (!scissorRight || !scissorLeft)
                    {
                        moddedDamageTypeHolder.Add(DamageTypes.NoSword);
                        scissorHit = false;
                    }
                    break;
            }
            if (butchered)
            {
                moddedDamageTypeHolder.Add(DamageTypes.ButcheredLifeSteal);
                moddedDamageTypeHolder.Add(DamageTypes.CutDamage);
            }
            impactSound = SeamstressAssets.scissorsHitSoundEvent.index;

            base.OnEnter();
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
            switch (base.swingIndex)
            {
                case 0:
                    PlayCrossfade("Gesture, Override", "Slash1", "Slash.playbackRate", this.duration, 0.1f * this.duration);
                    break;
                case 1:
                    PlayCrossfade("Gesture, Override", "Slash2", "Slash.playbackRate", this.duration, 0.1f * this.duration);
                    break;
                case 2:
                    PlayCrossfade("Gesture, Override", "Slash1", "Slash.playbackRate", this.duration, 0.1f * this.duration);
                    break;
            }
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