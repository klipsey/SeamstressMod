using RoR2;
using RoR2.Projectile;
using UnityEngine;
using R2API;
using SeamstressMod.Survivors.Seamstress;
using SeamstressMod.Modules.BaseStates;

namespace SeamstressMod.SkillStates
{
    public class ReapRecast: BaseMeleeAttack
    {
        public static GameObject _expungeEffect = SeamstressAssets.expungeEffect;
        public static GameObject _expungeEffect2 = SeamstressAssets.expungeSlashEffect;
        public static GameObject _expungeEffect3 = SeamstressAssets.expungeSlashEffect2;
        public static GameObject _expungeEffect4 = SeamstressAssets.expungeSlashEffect3;
        public override void OnEnter()
        {
            RefreshState();
            hitboxGroupName = "Sew";
            damageType = DamageType.Stun1s;
            moddedDamageType = DamageTypes.CutDamage;
            procCoefficient = 1f;
            pushForce = 300;
            bonusForce = Vector3.zero;
            baseDuration = 0.1f;

            //0-1 multiplier of= baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
            //for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds
            attackStartPercentTime = 0f;
            attackEndPercentTime = 0f;

            //this is the point at which an attack can be interrupted by itself, continuing a combo
            earlyExitPercentTime = 0f;

            hitStopDuration = 0.1f;
            attackRecoil = 0f;
            hitHopVelocity = 0f;
            swingSoundString = "Play_voidman_m2_explode";
            hitSoundString = "";
            hitEffectPrefab = SeamstressAssets.scissorsHitImpactEffect;

            muzzleString = "SewCenter";
            impactSound = SeamstressAssets.sewHitSoundEvent.index;
            hitEffectPrefab = SeamstressAssets.scissorsButcheredHitImpactEffect;
            isFlatDamage = true;
            damageTotal = this.characterBody.GetComponent<SeamstressController>().GetButcheredConversion();
            base.OnEnter();
            UnityEngine.Object.Instantiate<GameObject>(_expungeEffect, transform);
            UnityEngine.Object.Instantiate<GameObject>(_expungeEffect2, transform);
            UnityEngine.Object.Instantiate<GameObject>(_expungeEffect3, transform);
            UnityEngine.Object.Instantiate<GameObject>(_expungeEffect4, transform);
            if (base.isAuthority) 
            {
                Util.CleanseBody(this.characterBody, true, false, false, true, true, true);
            }
            this.skillLocator.utility = skillLocator.FindSkill("Utility");
            if (!isGrounded)
            {
                SmallHop(characterMotor, 5f);
            }
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
            if (base.isAuthority)
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
            Transform transform = FindModelChild(muzzleString);
            if ((bool)transform)
            {
                UnityEngine.Object.Instantiate(swingEffectPrefab, transform);
            }
        }

        protected override void PlayAttackAnimation()
        {
            PlayCrossfade("Gesture, Override", "ThrowBomb", "ThrowBomb.playbackRate", duration, 0.1f * duration);
        }
        public override void OnExit()
        {
            base.OnExit();

        }

    }
}