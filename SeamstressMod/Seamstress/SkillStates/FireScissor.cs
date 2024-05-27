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

        public static float damageCoefficient = SeamstressStaticValues.scissorDamageCoefficient;

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
            if(modelAnimator)
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
                PlayCrossfade("Gesture, Override", chosenAnim, "Slash.playbackRate", duration, 0.05f);
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
                if (sceptered) projectilePrefab.transform.GetChild(0).GetChild(5).gameObject.GetComponent<PickupFilter>().pickupTimer = 1.5f;
                if (insatiable)
                {
                    projectilePrefab.GetComponent<DamageAPI.ModdedDamageTypeHolderComponent>().Add(DamageTypes.CutDamage);
                }
                else
                {
                    projectilePrefab.GetComponent<DamageAPI.ModdedDamageTypeHolderComponent>().Remove(DamageTypes.CutDamage);
                }
                float healthMissing = ((this.characterBody.healthComponent.fullHealth + this.characterBody.healthComponent.fullShield) - (this.characterBody.healthComponent.health + this.characterBody.healthComponent.shield)) / (this.characterBody.healthComponent.fullHealth + this.characterBody.healthComponent.fullShield);
                projectilePrefab.GetComponent<ProjectileHealOwnerOnDamageInflicted>().fractionOfDamage = healthMissing * SeamstressStaticValues.passiveHealingScaling;
                if (base.characterBody.inventory.GetItemCount(DLC1Content.Items.EquipmentMagazineVoid) > 0)
                {
                    projectilePrefab.transform.GetChild(0).GetChild(5).gameObject.GetComponent<PickupFilter>().pickupTimer *= 0.66f;
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
                EffectManager.SpawnEffect(scissorFiringPrefab, effectData, transmit: false);
            }
            if (base.isAuthority)
            {
                ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, damageStat * damageCoefficient, 0f, Util.CheckRoll(critStat, characterBody.master), DamageColorIndex.Default, null, -1f);
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}