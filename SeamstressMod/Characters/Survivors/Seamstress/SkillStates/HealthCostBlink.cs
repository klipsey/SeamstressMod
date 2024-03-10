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
            seamCon.snapBackPosition = base.characterBody.corePosition;
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
                    SeamstressController s = characterBody.GetComponent<SeamstressController>();
                    s.fuckYou = false;
                    DotController.InflictDot(characterBody.gameObject, characterBody.gameObject, Dots.ButcheredDot, SeamstressStaticValues.butcheredDuration, 1, 1u);
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
            skillLocator.utility.SetSkillOverride(base.gameObject, SeamstressAssets.snapBackSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            base.OnExit();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}