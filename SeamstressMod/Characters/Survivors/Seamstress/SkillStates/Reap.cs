using RoR2;
using RoR2.Projectile;
using SeamstressMod.Survivors.Seamstress;
using SeamstressMod.Modules.BaseStates;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates;

namespace SeamstressMod.SkillStates
{
    public class Reap : BaseMeleeAttack
    {
        public GameObject reapPrefab;
        public override void OnEnter()
        {
            this.hitboxGroupName = "Sew";
            this.damageCoefficient = SeamstressStaticValues.sewDamageCoefficient;
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

            this.hitStopDuration = 0f;
            this.attackRecoil = 0f;
            this.hitHopVelocity = 0f;
            this.swingSoundString = "Play_voidman_m2_explode";
            this.hitSoundString = "";
            this.hitEffectPrefab = SeamstressAssets.scissorsHitImpactEffect;

            this.muzzleString = "CharacterCenter";
            this.moddedDamageType = DamageTypes.Empty;
            this.impactSound = SeamstressAssets.sewHitSoundEvent.index;
            this.swingEffectPrefab = SeamstressAssets.sewButcheredEffect;
            this.hitEffectPrefab = SeamstressAssets.scissorsButcheredHitImpactEffect;            
            base.OnEnter();
            //rechargeStocks();//quality of life
            reapPrefab = SeamstressAssets.reapBleedEffect;
            Util.PlaySound("Play_item_proc_novaonheal_impact", gameObject);
            CharacterModel component = (GetModelTransform()).GetComponent<CharacterModel>();
            TemporaryOverlay temporaryOverlay = base.gameObject.AddComponent<TemporaryOverlay>();
            temporaryOverlay.originalMaterial = SeamstressAssets.butcheredOverlayMat;
            temporaryOverlay.duration = SeamstressStaticValues.butcheredDuration;
            temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
            temporaryOverlay.AddToCharacerModel(component);
            Util.PlaySound("Play_bandit2_m2_alt_throw", base.characterBody.gameObject);
            UnityEngine.Object.Instantiate<GameObject>(reapPrefab, base.characterBody.modelLocator.transform);
            if (!base.characterMotor.isGrounded)
            {
                SmallHop(base.characterMotor, 6f);
            }
            Fire();
        }
        private void Fire()
        {
                if (NetworkServer.active && (bool)base.healthComponent && SeamstressStaticValues.reapHealthCost >= Mathf.Epsilon)
                {
                    float currentBarrier = healthComponent.barrier;
                    DamageInfo damageInfo = new DamageInfo();
                    damageInfo.damage = ((base.healthComponent.health + base.healthComponent.shield) * SeamstressStaticValues.reapHealthCost) + base.healthComponent.barrier;
                    damageInfo.position = base.characterBody.corePosition;
                    damageInfo.force = Vector3.zero;
                    damageInfo.damageColorIndex = DamageColorIndex.Default;
                    damageInfo.crit = false;
                    damageInfo.attacker = null;
                    damageInfo.inflictor = null;
                    damageInfo.damageType = DamageType.NonLethal | DamageType.BypassArmor | DamageType.BypassBlock;
                    damageInfo.procCoefficient = 0f;
                    base.healthComponent.TakeDamage(damageInfo);
                    base.healthComponent.AddBarrier(currentBarrier);
                    if (base.characterBody.HasBuff(SeamstressBuffs.butchered))
                    {
                        base.characterBody.RemoveBuff(SeamstressBuffs.butchered);
                    }
                    base.characterBody.AddTimedBuff(SeamstressBuffs.butchered, SeamstressStaticValues.butcheredDuration, 1);
                    base.characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.1f);
                    if (base.characterBody.GetBuffCount(SeamstressBuffs.needles) < SeamstressStaticValues.maxNeedleAmount + base.skillLocator.special.maxStock - 1)
                    {
                        base.characterBody.AddBuff(SeamstressBuffs.needles);
                    }
                    else
                    {
                        GameObject projectilePrefab;
                        Ray aimRay;
                        aimRay = new Ray(base.characterBody.inputBank.aimOrigin, base.characterBody.inputBank.aimDirection);
                        projectilePrefab = SeamstressAssets.needleButcheredPrefab;
                        ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.characterBody.gameObject, base.characterBody.damage * SeamstressStaticValues.sewNeedleDamageCoefficient, 600f, base.characterBody.RollCrit(), DamageColorIndex.Default, null, -1f);
                    }
                }
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
