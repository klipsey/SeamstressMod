using System.Linq;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using SeamstressMod.Modules.BaseStates;
using SeamstressMod.Survivors.Seamstress;
using R2API;
using UnityEngine.Networking;
using EntityStates;
using static Rewired.ComponentControls.Effects.RotateAroundAxis;
using static UnityEngine.UI.Image;

namespace SeamstressMod.SkillStates
{
    public class FireScissor : BaseSeamstressSkillState
    {
        public static float baseDuration = 0.5f;

        public static float damageCoefficient = SeamstressStaticValues.scissorDamageCoefficient;

        public static float procCoefficient = 1f;

        public static GameObject scissorFiringPrefab = SeamstressAssets.blinkPrefab;

        public static GameObject hitEffectPrefab;

        public static string attackSoundString = "Play_imp_overlord_attack1_throw";

        private GameObject projectilePrefab;

        private Animator modelAnimator;

        private float duration;

        private int chosenAnim = 2;

        private bool hasFired;

        private string fireString;
        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound("Play_item_lunar_specialReplace_explode", base.gameObject);
            this.characterBody.GetComponent<ScissorController>().isRight = true;
            duration = baseDuration / attackSpeedStat;
            this.modelAnimator = GetModelAnimator();
            if(this.scissorRight && this.scissorLeft)
            {
                this.chosenAnim = 2;
            }    
            else if(this.scissorRight && !this.scissorLeft)
            {
                this.chosenAnim = 1;
            }
            else if(this.scissorLeft && !this.scissorRight) 
            {
                this.chosenAnim = 2;
            }
            else
            {
                this.chosenAnim = 2;
            }

            if (this.modelAnimator)
            {

                //string animationStateName = ((chosenAnim == 2) ? "FireVoidspikesL" : "FireVoidspikesR");
                //PlayAnimation("Gesture, Override", animationStateName, "FireVoidspikes.playbackRate", duration);
                PlayAnimation("Gesture, Override", "ThrowBomb", "ThrowBomb.playbackRate", duration);

            }
            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(0f);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (this.modelAnimator && !hasFired)
            {
                if (this.chosenAnim == 2)
                {
                    this.projectilePrefab = SeamstressAssets.scissorLPrefab;
                    this.fireString = "SwingRight";
                }
                else if (this.chosenAnim == 1)
                {
                    this.projectilePrefab = SeamstressAssets.scissorRPrefab;
                    this.fireString = "SwingLeft";
                }
                if (this.butchered)
                {
                    projectilePrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>().Add(DamageTypes.CutDamage);
                }
                else
                {
                    projectilePrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>().Remove(DamageTypes.CutDamage);
                }
                Fire(GetAimRay(), fireString);
                hasFired = true;
            }
            if (base.fixedAge >= duration && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        private void Fire(Ray aimRay, string muzzleName)
        {
            Util.PlaySound(attackSoundString, base.gameObject);
            Transform transform = FindModelChild(muzzleName);
            if (transform)
            {
                EffectData effectData = new EffectData();
                effectData.rotation = transform.rotation;
                effectData.origin = transform.position;
                effectData.scale = 0.25f;
                EffectManager.SpawnEffect(scissorFiringPrefab, effectData, transmit: true);
            }
            if (base.isAuthority)
            {
                ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, damageStat * damageCoefficient, 0f, Util.CheckRoll(critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
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