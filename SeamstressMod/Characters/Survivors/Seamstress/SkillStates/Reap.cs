using RoR2;
using RoR2.Projectile;
using SeamstressMod.Survivors.Seamstress;
using SeamstressMod.Modules.BaseStates;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates;

namespace SeamstressMod.SkillStates
{
    public class Reap : BaseSeamstressSkillState
    {
        public static float baseDuration = 1f;
        public static float firePercentTime = 0f;
        public static GameObject reapBleed = SeamstressAssets.reapBleedEffect;

        private float duration;
        private float fireTime;
        private bool hasFired;

        public static float healthCostFraction = SeamstressStaticValues.reapHealthCost;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            fireTime = firePercentTime * duration;
            Util.PlaySound("Play_item_proc_novaonheal_impact", gameObject);
            Object.Instantiate<GameObject>(reapBleed, characterBody.modelLocator.transform);
            PlayAnimation("Gesture, Override", "ThrowBomb", "ThrowBomb.playbackRate", duration);
            if (!base.characterMotor.isGrounded)
            {
                SmallHop(base.characterMotor, 6f);
            }
            base.skillLocator.utility = base.skillLocator.FindSkill("reapRecast");
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= fireTime)
            {
                Fire();
            }
            if (fixedAge >= duration && base.isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }
        private void Fire()
        {
            if (!hasFired)
            {
                hasFired = true;
                if (NetworkServer.active && (bool)healthComponent && healthCostFraction >= Mathf.Epsilon)
                {
                    float currentBarrier = healthComponent.barrier;
                    DamageInfo damageInfo = new DamageInfo();
                    damageInfo.damage = ((healthComponent.health + healthComponent.shield) * healthCostFraction) + healthComponent.barrier;
                    damageInfo.position = characterBody.corePosition;
                    damageInfo.force = Vector3.zero;
                    damageInfo.damageColorIndex = DamageColorIndex.Default;
                    damageInfo.crit = false;
                    damageInfo.attacker = null;
                    damageInfo.inflictor = null;
                    damageInfo.damageType = DamageType.NonLethal | DamageType.BypassArmor | DamageType.BypassBlock;
                    damageInfo.procCoefficient = 0f;
                    healthComponent.TakeDamage(damageInfo);
                    healthComponent.AddBarrier(currentBarrier);
                    characterBody.AddTimedBuff(SeamstressBuffs.butchered, SeamstressStaticValues.butcheredDuration, 1);
                    characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.25f);
                }
            }
        }
        public override void OnExit()
        {
            base.OnExit();
        }
    }
}