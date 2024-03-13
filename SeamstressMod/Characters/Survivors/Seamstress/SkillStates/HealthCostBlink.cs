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
using RoR2.Projectile;

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

        public static GameObject projectilePrefab = SeamstressAssets.heartPrefab;
        public override void OnEnter()
        {
            this.blinkPrefab = SeamstressAssets.blinkPrefab;
            this.split = true;
            base.OnEnter();
            Vector3 effectPos = this.transform.localPosition;
            RaycastHit raycastHit;
            if (Physics.Raycast(effectPos, Vector3.one, out raycastHit, 10f, LayerIndex.world.mask))
            {
                effectPos = raycastHit.point;
            }
            EffectManager.SpawnEffect(SeamstressAssets.splat, new EffectData
            {
                origin = effectPos,
                rotation = Quaternion.identity,
                color = SeamstressAssets.coolRed,
            }, true);
            GenericCharacterMain.ApplyJumpVelocity(base.characterMotor, base.characterBody, 1f, 1f, false);
            seamCon.snapBackPosition = base.characterBody.corePosition;

            Vector3 position = base.characterBody.corePosition;
            GameObject obj = UnityEngine.Object.Instantiate(projectilePrefab, position, Quaternion.identity);
            ProjectileController component = obj.GetComponent<ProjectileController>();
            if (component)
            {
                component.owner = base.gameObject;
                component.Networkowner = base.gameObject;
            }
            obj.GetComponent<TeamFilter>().teamIndex = base.GetComponent<TeamComponent>().teamIndex;
            if(NetworkServer.active)
            {
                NetworkServer.Spawn(obj);
            }
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
                    s.inButchered = false;
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