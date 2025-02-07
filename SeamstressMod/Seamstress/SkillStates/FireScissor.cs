using System.Linq;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using SeamstressMod.Modules.BaseStates;
using R2API;
using UnityEngine.Networking;
using EntityStates;
using SeamstressMod.Seamstress.Components;
using SeamstressMod.Seamstress.Content;

namespace SeamstressMod.Seamstress.SkillStates
{
    public class FireScissor : BaseSeamstressSkillState
    {
        public GameObject scissorFiringPrefab = SeamstressAssets.impDashEffect;

        private GameObject projectilePrefab;

        public static float baseDuration = 0.5f;

        public static float damageCoefficient = 1f;

        public static float procCoefficient = 1f;

        public static string attackSoundString = "Play_imp_overlord_attack1_throw";

        private Animator modelAnimator;

        private float duration;

        private string chosenAnim = "FireScissorL";

        private bool hasFired;

        protected virtual bool sceptered
        {
            get
            {
                return false;
            }
        }

        Ray aimRay;

        private string fireString;
        public override void OnEnter()
        {
            RefreshState();
            if (seamstressController.blue) scissorFiringPrefab = SeamstressAssets.impDashEffect2;
            base.OnEnter();
            Util.PlaySound("Play_item_lunar_specialReplace_explode", gameObject);
            characterBody.GetComponent<ScissorController>().isRight = true;
            duration = baseDuration / attackSpeedStat;
            aimRay = GetAimRay();
            StartAimMode(aimRay, 2f);
            modelAnimator = GetModelAnimator();
            if (modelAnimator)
            {
                if (scissorRight && scissorLeft)
                {
                    chosenAnim = "FireScissorL";
                }
                else if (scissorRight && !scissorLeft)
                {
                    chosenAnim = "FireScissorR";
                }
                else if (scissorLeft && !scissorRight)
                {
                    chosenAnim = "FireScissorL";
                }
                else
                {
                    chosenAnim = "FireScissorL";
                }
                PlayCrossfade("Gesture, Additive", chosenAnim, "Slash.playbackRate", duration, 0.05f);
            }

            if (NetworkServer.active)
            {
                DamageInfo damageInfo = new DamageInfo
                {
                    damage = characterBody.healthComponent.fullCombinedHealth * 0.15f,
                    damageType = DamageType.NonLethal | DamageType.BypassArmor | DamageType.BypassBlock,
                    position = characterBody.corePosition,
                    attacker = null,
                    procCoefficient = 0f,
                    crit = false,
                    damageColorIndex = DamageColorIndex.Bleed,
                };

                characterBody.healthComponent.TakeDamage(damageInfo);
            }
        }

        public override void OnExit()
        {
            RefreshState();
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (modelAnimator && !hasFired)
            {
                if (chosenAnim == "FireScissorL")
                {
                    projectilePrefab = this.seamstressController.scissorLPrefab;
                    fireString = "SwingRightSmall";
                }
                else
                {
                    projectilePrefab = this.seamstressController.scissorRPrefab;
                    fireString = "SwingLeftSmall";
                }
                float num = sceptered ? 1.5f : SeamstressConfig.basePickupCooldown.Value;
                projectilePrefab.transform.GetChild(0).GetChild(5).gameObject.GetComponent<PickupFilterComponent>().pickupTimer = Mathf.Max(0.5f, num * base.skillLocator.special.cooldownScale - base.skillLocator.special.flatCooldownReduction);
                if (isInsatiable)
                {
                    projectilePrefab.GetComponent<ProjectileDamage>().damageType.AddModdedDamageType(DamageTypes.CutDamage);
                }
                else if(projectilePrefab.GetComponent<ProjectileDamage>().damageType.HasModdedDamageType(DamageTypes.CutDamage))
                {
                    projectilePrefab.GetComponent<ProjectileDamage>().damageType.RemoveModdedDamageType(DamageTypes.CutDamage);
                }

                Fire(this.aimRay, fireString);
                hasFired = true;
            }
            if (base.fixedAge >= duration && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        private void Fire(Ray aimRay, string muzzleName)
        {
            Util.PlaySound(attackSoundString, gameObject);
            Transform transform = FindModelChild(muzzleName);
            if (transform)
            {
                EffectData effectData = new EffectData();
                effectData.rotation = Util.QuaternionSafeLookRotation(aimRay.direction);
                effectData.origin = transform.position;
                effectData.scale = 0.5f;
                EffectManager.SpawnEffect(scissorFiringPrefab, effectData, transmit: true);
            }
            if (base.isAuthority)
            {
                ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, damageStat, 0f, Util.CheckRoll(critStat, characterBody.master), DamageColorIndex.Default, null, -1f);
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}