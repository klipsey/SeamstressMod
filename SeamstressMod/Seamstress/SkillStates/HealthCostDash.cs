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
using SeamstressMod.Seamstress.Components;

namespace SeamstressMod.Seamstress.SkillStates
{
    public class HealthCostDash : BaseSeamstressSkillState
    {
        public float baseDuration = 0.8f;
        public float dashPower = 6f;
        public float damageCoefficient = SeamstressStaticValues.blinkDamageCoefficient;
        public GameObject uppercutEffect = SeamstressAssets.uppercutEffect;
        public GameObject projectilePrefab = SeamstressAssets.heartPrefab;
        public GameObject scissorHitImpactEffect = SeamstressAssets.scissorsHitImpactEffect;
        public GameObject bloodExplosionEffect = SeamstressAssets.bloodExplosionEffect;
        public GameObject impDashEffect = SeamstressAssets.impDashEffect;
        public GameObject bloodSplatterEffect = SeamstressAssets.bloodSplatterEffect;
        public GameObject smallBlinkEffect = SeamstressAssets.smallBlinkEffect;
        public Color mainColor = SeamstressAssets.coolRed;
        public Material destealthMaterial = SeamstressAssets.destealthMaterial;
        private Vector3 dashVector;
        private OverlapAttack attack;
        private List<HurtBox> victimsStruck = new List<HurtBox>();
        protected string hitBoxString = "Sword";
        private bool hasHit;
        private bool hasDelayed;

        public override void OnEnter()
        {
            RefreshState();
            if (seamstressController.blue)
            {
                uppercutEffect = SeamstressAssets.uppercutEffect2;
                projectilePrefab = SeamstressAssets.heartPrefab2;
                scissorHitImpactEffect = SeamstressAssets.scissorsHitImpactEffect2;
                bloodExplosionEffect = SeamstressAssets.bloodExplosionEffect2;
                impDashEffect = SeamstressAssets.impDashEffect2;
                bloodSplatterEffect = SeamstressAssets.bloodSplatterEffect2;
                smallBlinkEffect = SeamstressAssets.smallBlinkEffect2;
                mainColor = Color.cyan;
                destealthMaterial = SeamstressAssets.destealthMaterial2;
            }
            base.OnEnter();

            dashVector = base.inputBank.aimDirection;

            base.characterMotor.disableAirControlUntilCollision = false;

            Transform modelTransform = base.GetModelTransform();
            Animator animator = modelTransform.GetComponent<Animator>();

            if (modelTransform && this.destealthMaterial)
            {
                TemporaryOverlayInstance temporaryOverlayInstance = TemporaryOverlayManager.AddOverlay(base.gameObject);
                temporaryOverlayInstance.duration = 1.2f;
                temporaryOverlayInstance.destroyComponentOnEnd = true;
                temporaryOverlayInstance.originalMaterial = this.destealthMaterial;
                temporaryOverlayInstance.inspectorCharacterModel = animator.gameObject.GetComponent<CharacterModel>();
                temporaryOverlayInstance.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlayInstance.animateShaderAlpha = true;
            }
            base.characterMotor.velocity = Vector3.zero;

            PlayCrossfade("FullBody, Override", "RipHeart", "Dash.playbackRate", (baseDuration / attackSpeedStat) * 1.8f, (baseDuration / attackSpeedStat) * 0.05f);
            Util.PlayAttackSpeedSound("Play_imp_overlord_attack2_tell", gameObject, attackSpeedStat);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!hasDelayed) base.characterMotor.velocity.y = 0f;
            if (base.fixedAge > (0.5f / attackSpeedStat) && !hasDelayed)
            {
                Util.PlaySound("sfx_seamstress_dash",base.gameObject);
                Util.PlaySound("sfx_seamstress_chains", base.gameObject);
                hasDelayed = true;
                this.attack = new OverlapAttack();
                attack.attacker = base.gameObject;
                attack.inflictor = base.gameObject;
                attack.damageType = DamageType.Stun1s;
                attack.procCoefficient = 1f;
                attack.teamIndex = base.GetTeam();
                attack.isCrit = base.RollCrit();
                attack.forceVector = Vector3.up * 2000f;
                attack.damage = damageCoefficient * damageStat;
                attack.hitBoxGroup = FindHitBoxGroup(hitBoxString);
                attack.hitEffectPrefab = scissorHitImpactEffect;

                EffectManager.SpawnEffect(this.bloodExplosionEffect, new EffectData
                {
                    origin = this.transform.position,
                    rotation = Quaternion.identity,
                    scale = 0.5f
                }, false);

                if (isInsatiable)
                {
                    attack.AddModdedDamageType(DamageTypes.CutDamage);
                }
                attack.AddModdedDamageType(DamageTypes.SeamstressLifesteal);

                EffectData effectData = new EffectData()
                {
                    origin = base.characterBody.corePosition,
                    rotation = Util.QuaternionSafeLookRotation(dashVector),
                    scale = 3f
                };
                EffectManager.SpawnEffect(this.impDashEffect, effectData, false);
                EffectManager.SpawnEffect(this.smallBlinkEffect, effectData, false);

                base.characterMotor.velocity.y = 0f;
                base.characterMotor.velocity += dashVector * (dashPower * (moveSpeedStat + 1f));

                Vector3 effectPos = this.transform.localPosition;
                RaycastHit raycastHit;
                if (Physics.Raycast(effectPos, Vector3.one, out raycastHit, 10f, LayerIndex.world.mask))
                {
                    effectPos = raycastHit.point;
                }
                EffectManager.SpawnEffect(this.bloodSplatterEffect, new EffectData
                {
                    origin = effectPos,
                    rotation = Quaternion.identity,
                    color = mainColor,
                }, false);

                seamstressController.snapBackPosition = base.characterBody.corePosition;

                Vector3 position = base.characterBody.corePosition;

                if (NetworkServer.active)
                {
                    base.characterBody.AddTimedBuff(SeamstressBuffs.SeamstressInsatiableBuff, SeamstressStaticValues.insatiableDuration);
                    base.gameObject.AddComponent<SeamstressBleedVisualController>();
                }

                GameObject obj = UnityEngine.Object.Instantiate<GameObject>(this.projectilePrefab, position, Quaternion.identity);

                ProjectileController component = obj.GetComponent<ProjectileController>();
                if (component)  
                {
                    component.owner = base.gameObject;
                    component.Networkowner = base.gameObject;
                }
                obj.GetComponent<TeamFilter>().teamIndex = base.GetComponent<TeamComponent>().teamIndex;

                if (NetworkServer.active)
                {
                    NetworkServer.Spawn(obj);
                }
            }
            if (base.isAuthority && hasDelayed)
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

                if (base.fixedAge >= baseDuration && hasDelayed)
                {
                    EffectData effectData = new EffectData()
                    {
                        origin = characterBody.corePosition,
                        rotation = Util.QuaternionSafeLookRotation(dashVector),
                        scale = 3f
                    };
                    EffectManager.SpawnEffect(this.impDashEffect, effectData, false);
                    outer.SetNextStateToMain();
                }
            }
        }

        public override void OnExit()
        {
            if (!hasHit) base.characterMotor.velocity *= 0.2f;

            base.OnExit();

            skillLocator.utility.SetSkillOverride(base.gameObject, SeamstressSurvivor.snapBackSkillDef, GenericSkill.SkillOverridePriority.Contextual);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}