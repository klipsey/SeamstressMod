using System.Linq;
using RoR2;
using RoR2.Navigation;
using UnityEngine;
using SeamstressMod.Modules.BaseStates;
using SeamstressMod.Survivors.Seamstress;
using R2API;
using UnityEngine.Networking;
using EntityStates;
using static R2API.DamageAPI;

namespace SeamstressMod.SkillStates
{
    public class Snapback : BaseSeamstressSkillState
    {

        public static string exitSoundString = "Play_item_proc_bounceChain";

        private CameraTargetParams.AimRequest aimRequest;

        private Vector3 snapPosition;

        private Vector3 currentPosition;

        private Animator animator;

        private CharacterModel characterModel;

        private HurtBoxGroup hurtboxGroup;

        private Transform modelTransform;

        private float duration;

        private bool hasSnapped;
        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(exitSoundString, base.gameObject);
            Util.CleanseBody(base.characterBody, false, false, false, true, false, false);
            CalculateSnapDestination();
            this.modelTransform = GetModelTransform();
            if (this.modelTransform)
            {
                this.animator = this.modelTransform.GetComponent<Animator>();
                this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
                this.hurtboxGroup = this.modelTransform.GetComponent<HurtBoxGroup>();
            }
            if (this.hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }
            if (base.cameraTargetParams)
            {
                aimRequest = base.cameraTargetParams.RequestAimType(CameraTargetParams.AimType.Aura);
            }
            skillLocator.utility.UnsetSkillOverride(base.gameObject, SeamstressAssets.snapBackSkillDef, GenericSkill.SkillOverridePriority.Contextual);

        }
        private void SetPosition(Vector3 newPosition)
        {
            if (base.characterMotor)
            {
                base.characterMotor.Motor.SetPositionAndRotation(newPosition, Quaternion.identity);
            }
        }
        private void CalculateSnapDestination()
        {
            if (base.isAuthority)
            {
                snapPosition = seamCon.snapBackPosition;
                currentPosition = base.characterBody.corePosition;
                duration = (currentPosition - snapPosition).magnitude / 10f;
                duration = Mathf.Clamp(duration, 0.2f, 1f);
                base.characterDirection.forward = snapPosition;
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.characterMotor)
            {
                base.characterMotor.velocity = Vector3.zero;
            }
            if(!hasSnapped)
            {
                SetPosition(Vector3.Lerp(currentPosition, snapPosition, base.fixedAge / duration));
            }
            if (base.fixedAge >= duration && base.isAuthority)
            {
                hasSnapped = true;
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            if (this.hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }
            if (base.cameraTargetParams)
            {
                aimRequest.Dispose();
            }
            base.OnExit();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (fixedAge < duration) return InterruptPriority.Death;
            else return InterruptPriority.Any;
        }
        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(snapPosition);
            writer.Write(currentPosition);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            snapPosition = reader.ReadVector3();
            currentPosition = reader.ReadVector3();
        }
    }
}