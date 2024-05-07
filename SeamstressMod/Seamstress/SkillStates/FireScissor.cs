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
        public static float baseDuration = 0.5f;

        public static float damageCoefficient = SeamstressStaticValues.scissorDamageCoefficient;

        public static float procCoefficient = 1f;

        public static GameObject scissorFiringPrefab = SeamstressAssets.impDashEffect;

        public static string attackSoundString = "Play_imp_overlord_attack1_throw";

        private GameObject projectilePrefab;

        private Animator modelAnimator;

        private float duration;

        private int chosenAnim = 2;

        private bool hasFired;

        Ray aimRay;

        private string fireString;
        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound("Play_item_lunar_specialReplace_explode", gameObject);
            characterBody.GetComponent<ScissorController>().isRight = true;
            duration = baseDuration / attackSpeedStat;
            aimRay = GetAimRay();
            StartAimMode(aimRay, 2f);
            modelAnimator = GetModelAnimator();
            if (modelAnimator)
            {
                //string animationStateName = ((chosenAnim == 2) ? "FireVoidspikesL" : "FireVoidspikesR");
                //PlayAnimation("Gesture, Override", animationStateName, "FireVoidspikes.playbackRate", duration);
                PlayAnimation("Gesture, Override", "ThrowBomb", "ThrowBomb.playbackRate", duration);

            }
            if (scissorRight && scissorLeft)
            {
                chosenAnim = 2;
            }
            else if (scissorRight && !scissorLeft)
            {
                chosenAnim = 1;
            }
            else if (scissorLeft && !scissorRight)
            {
                chosenAnim = 2;
            }
            else
            {
                chosenAnim = 2;
            }
        }

        public override void OnExit()
        {
            RefreshState();
            if (!scissorLeft && !scissorRight) 
            {
                characterBody.GetComponent<SeamstressController>().ReactivateScissor("meshScissors", false);
            }
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (modelAnimator && !hasFired)
            {
                if (chosenAnim == 2)
                {
                    projectilePrefab = SeamstressAssets.scissorLPrefab;
                    fireString = "SwingRightSmall";
                }
                else
                {
                    projectilePrefab = SeamstressAssets.scissorRPrefab;
                    fireString = "SwingLeftSmall";
                }
                if (insatiable)
                {
                    projectilePrefab.GetComponent<DamageAPI.ModdedDamageTypeHolderComponent>().Add(DamageTypes.CutDamage);
                }
                else
                {
                    projectilePrefab.GetComponent<DamageAPI.ModdedDamageTypeHolderComponent>().Remove(DamageTypes.CutDamage);
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
                ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), gameObject, damageStat * damageCoefficient, 0f, Util.CheckRoll(critStat, characterBody.master), DamageColorIndex.Default, null, -1f);
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write((char)chosenAnim);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            chosenAnim = reader.ReadChar();
        }
    }

}