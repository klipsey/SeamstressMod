using RoR2;
using SeamstressMod.Survivors.Seamstress;
using SeamstressMod.SkillStates.BaseStates;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates;

namespace SeamstressMod.SkillStates
{
    public class Reap : BaseSeamstressSkillState
    {
        public static float baseDuration = 0.6f;

        public static float duration;

        public static float firePercentTime = 0f;

        private float fireTime;

        private bool hasFired;

        public static float healthCostFraction = 0.5f;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            fireTime = firePercentTime * duration;
            Util.PlaySound("Play_item_proc_novaonheal_impact", gameObject);
            PlayAnimation("Gesture, Override", "ThrowBomb", "ThrowBomb.playbackRate", duration * 0.1f);
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
                    characterBody.AddTimedBuff(SeamstressBuffs.bloodBath, 6f);
                    characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.5f);
                }
            }
        }
        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
