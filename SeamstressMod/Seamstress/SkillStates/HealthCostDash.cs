using System.Linq;
using RoR2;
using RoR2.Navigation;
using UnityEngine;
using SeamstressMod.Modules.BaseStates;
using R2API;
using UnityEngine.Networking;
using EntityStates;
using UnityEngine.Networking.Match;
using SeamstressMod.Modules;
using System.Collections.Generic;
using System;
using RoR2.Projectile;
using SeamstressMod.Seamstress.Content;

namespace SeamstressMod.Seamstress.SkillStates
{
    public class HealthCostDash : BaseSeamstressSkillState
    {
        public static float baseDuration = 0.3f;
        public static float dashPower = 6f;
        public static float damageCoefficient = SeamstressStaticValues.blinkDamageCoefficient;
        public static GameObject uppercutEffect = SeamstressAssets.uppercutEffect;
        public static GameObject projectilePrefab = SeamstressAssets.heartPrefab;
        private Ray aimRay;
        private Vector3 dashVector;
        private OverlapAttack attack;
        private List<HurtBox> victimsStruck = new List<HurtBox>();
        protected string hitBoxString = "Sword";
        private bool hasHit;

        public override void OnEnter()
        {
            base.OnEnter();

            dashVector = inputBank.aimDirection;

            base.characterMotor.disableAirControlUntilCollision = false;

            Transform modelTransform = base.GetModelTransform();
            Animator animator = modelTransform.GetComponent<Animator>();

            if (modelTransform && SeamstressAssets.destealthMaterial)
            {
                TemporaryOverlay temporaryOverlay = animator.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = 1f;
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = SeamstressAssets.destealthMaterial;
                temporaryOverlay.inspectorCharacterModel = animator.gameObject.GetComponent<CharacterModel>();
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.animateShaderAlpha = true;
            }

            this.attack = new OverlapAttack();
            attack.attacker = base.gameObject;
            attack.inflictor = base.gameObject;
            attack.damageType = DamageType.Stun1s;
            attack.procCoefficient = 1f;
            attack.teamIndex = base.GetTeam();
            attack.isCrit = base.RollCrit();
            attack.forceVector = Vector3.up * 3000f;
            attack.damage = damageCoefficient * damageStat;
            attack.hitBoxGroup = FindHitBoxGroup(hitBoxString);
            attack.hitEffectPrefab = SeamstressAssets.scissorsHitImpactEffect;
            if (butchered)
            {
                attack.AddModdedDamageType(DamageTypes.CutDamage);
                attack.AddModdedDamageType(DamageTypes.InsatiableLifeSteal);
            }
            EffectData effectData = new EffectData()
            {
                origin = base.characterBody.corePosition,
                rotation = Util.QuaternionSafeLookRotation(dashVector),
                scale = 3f
            };
            EffectManager.SpawnEffect(SeamstressAssets.impDash, effectData, false);
            EffectManager.SpawnEffect(SeamstressAssets.smallBlinkPrefab, effectData, false);
            
            PlayAnimation("FullBody, Override", "Roll", "Roll.playbackRate", baseDuration);

            base.characterMotor.velocity.y = 0f;
            base.characterMotor.velocity += dashVector * (dashPower * (moveSpeedStat + 1f));

            if (NetworkServer.active)
            {
                characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
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
            }, false);

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
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (isAuthority)
            {
                characterDirection.forward = dashVector;
                characterBody.isSprinting = true;

                if (attack.Fire(victimsStruck))
                {
                    hasHit = true;
                    Transform transform = FindModelChild("UpperCut");
                    if (transform)
                    {
                        UnityEngine.Object.Instantiate(uppercutEffect, transform);
                    }
                    characterMotor.velocity = Vector3.zero;
                    SmallHop(characterMotor, 4f);

                    outer.SetNextStateToMain();
                }

                if (fixedAge >= baseDuration)
                {
                    EffectData effectData = new EffectData()
                    {
                        origin = characterBody.corePosition,
                        rotation = Util.QuaternionSafeLookRotation(dashVector),
                        scale = 3f
                    };
                    EffectManager.SpawnEffect(SeamstressAssets.impDash, effectData, false);
                    outer.SetNextStateToMain();
                }
            }
        }

        public override void OnExit()
        {
            if (NetworkServer.active && healthComponent)
            {
                seamCon.inInsatiable = false;
                DotController.InflictDot(characterBody.gameObject, characterBody.gameObject, Dots.ButcheredDot, SeamstressStaticValues.butcheredDuration, 1, 1u);
            }

            if (!hasHit) base.characterMotor.velocity *= 0.2f;

            if (NetworkServer.active)
            {
                characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
                characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.3f);
            }
            skillLocator.utility.SetSkillOverride(base.gameObject, SeamstressAssets.snapBackSkillDef, GenericSkill.SkillOverridePriority.Contextual);

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}