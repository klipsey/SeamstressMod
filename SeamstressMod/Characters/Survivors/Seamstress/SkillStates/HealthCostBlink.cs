using System.Linq;
using RoR2;
using RoR2.Navigation;
using UnityEngine;
using SeamstressMod.Modules.BaseStates;
using SeamstressMod.Survivors.Seamstress;
using R2API;
using UnityEngine.Networking;
using EntityStates;
using UnityEngine.Networking.Match;

namespace SeamstressMod.SkillStates
{
    public class HealthCostBlink : SeamstressBlink
    {
        private float healthCostFraction = 0.5f;
        //test sphere collider changing with scale
        public static float blastAttackRadius = SeamstressAssets.blinkDestinationPrefab.transform.GetChild(1).gameObject.GetComponent<SphereCollider>().radius - 10;

        public static float blastAttackDamageCoefficient = SeamstressStaticValues.blinkDamageCoefficient;

        public static float blastAttackForce = 25f;

        public static float blastAttackProcCoefficient = 1f;

        public override void OnEnter()
        {
            this.blinkPrefab = SeamstressAssets.blinkPrefab;
            this.split = true;
            GenericCharacterMain.ApplyJumpVelocity(base.characterMotor, base.characterBody, 1f, 1f, false);
            base.OnEnter();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
        public override void OnExit()
        {
            if (!outer.destroying)
            {
                if (NetworkServer.active && healthComponent && healthCostFraction >= Mathf.Epsilon)
                {
                    float currentBarrier = healthComponent.barrier;
                    float currentShield = healthComponent.shield;
                    DamageInfo damageInfo = new DamageInfo();
                    damageInfo.damage = (healthComponent.health * healthCostFraction) + healthComponent.shield + healthComponent.barrier;
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
                    healthComponent.shield = currentShield;
                    SeamstressController s = characterBody.GetComponent<SeamstressController>();
                    s.fuckYou = false;
                    characterBody.AddTimedBuff(SeamstressBuffs.butchered, SeamstressStaticValues.butcheredDuration, 1);
                }
                if (blastAttackDamageCoefficient > 0f && base.isAuthority)
                {
                    BlastAttack blastAttack = new BlastAttack();
                    blastAttack.attacker = base.gameObject;
                    blastAttack.inflictor = base.gameObject;
                    blastAttack.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
                    blastAttack.baseDamage = damageStat * blastAttackDamageCoefficient;
                    blastAttack.baseForce = blastAttackForce;
                    blastAttack.position = base.transform.position;
                    blastAttack.procCoefficient = blastAttackProcCoefficient;
                    blastAttack.radius = blastAttackRadius;
                    blastAttack.damageType = DamageType.Stun1s;
                    if (empowered)
                    {
                        blastAttack.AddModdedDamageType(DamageTypes.CutDamage);
                        blastAttack.AddModdedDamageType(DamageTypes.ButcheredLifeSteal);
                    }
                    blastAttack.falloffModel = BlastAttack.FalloffModel.Linear;
                    blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;
                    blastAttack.Fire();
                }
            }
            base.OnExit();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}