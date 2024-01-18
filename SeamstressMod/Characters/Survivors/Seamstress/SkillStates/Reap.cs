using RoR2;
using RoR2.Projectile;
using SeamstressMod.Survivors.Seamstress;
using SeamstressMod.SkillStates.BaseStates;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates;

namespace SeamstressMod.SkillStates
{
    public class Reap : BaseSeamstressSkillState
    {
        public GameObject reapPrefab;

        public static float baseDuration = 1f;

        public static float duration;

        public static float firePercentTime = 0f;

        private float fireTime;

        private bool hasFired;

        public static float healthCostFraction = SeamstressStaticValues.reapHealthCost;
        public override void OnEnter()
        {
            base.OnEnter();
            //rechargeStocks();//quality of life
            reapPrefab = SeamstressAssets.reapBleedEffect;
            duration = baseDuration / attackSpeedStat;
            fireTime = firePercentTime * duration;
            Util.PlaySound("Play_item_proc_novaonheal_impact", gameObject);
            PlayAnimation("Gesture, Override", "ThrowBomb", "ThrowBomb.playbackRate", duration);
            CharacterModel component = (GetModelTransform()).GetComponent<CharacterModel>();
            TemporaryOverlay temporaryOverlay = base.gameObject.AddComponent<TemporaryOverlay>();
            temporaryOverlay.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matFullCrit");
            temporaryOverlay.duration = SeamstressStaticValues.butcheredDuration;
            temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
            temporaryOverlay.AddToCharacerModel(component);
            Util.PlaySound("Play_bandit2_m2_alt_throw", base.characterBody.gameObject);
            UnityEngine.Object.Instantiate<GameObject>(reapPrefab, base.characterBody.modelLocator.transform);
            if (!base.characterMotor.isGrounded)
            {
                SmallHop(base.characterMotor, 6f);
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= fireTime)
            {
                Fire();
            }
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }
        private void Fire()
        {
            if(!hasFired)
            {
                hasFired = true;
                if (NetworkServer.active && (bool)base.healthComponent && healthCostFraction >= Mathf.Epsilon)
                {
                    float currentBarrier = healthComponent.barrier;
                    DamageInfo damageInfo = new DamageInfo();
                    damageInfo.damage = ((base.healthComponent.health + base.healthComponent.shield) * healthCostFraction) + base.healthComponent.barrier;
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
                    if(base.characterBody.HasBuff(SeamstressBuffs.butchered))
                    {
                        base.characterBody.RemoveBuff(SeamstressBuffs.butchered);
                    }
                    base.characterBody.AddTimedBuff(SeamstressBuffs.butchered, SeamstressStaticValues.butcheredDuration, 1);
                    base.characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.25f);
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
        }
        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
