using System.Linq;
using RoR2;
using RoR2.Navigation;
using UnityEngine;
using SeamstressMod.Modules.BaseStates;
using R2API;
using UnityEngine.Networking;
using EntityStates;
using static R2API.DamageAPI;
using SeamstressMod.Seamstress.Content;

namespace SeamstressMod.Seamstress.SkillStates
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
            Util.PlaySound(exitSoundString, gameObject);
            if(NetworkServer.active) Util.CleanseBody(base.characterBody, false, false, false, true, false, false);
            CalculateSnapDestination();
            modelTransform = GetModelTransform();
            if (modelTransform)
            {
                animator = modelTransform.GetComponent<Animator>();
                characterModel = modelTransform.GetComponent<CharacterModel>();
                hurtboxGroup = modelTransform.GetComponent<HurtBoxGroup>();
            }
            if (hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }
            if (cameraTargetParams)
            {
                aimRequest = cameraTargetParams.RequestAimType(CameraTargetParams.AimType.Aura);
            }
            skillLocator.utility.UnsetSkillOverride(gameObject, SeamstressSurvivor.snapBackSkillDef, GenericSkill.SkillOverridePriority.Contextual);
        }
        private void SetPosition(Vector3 newPosition)
        {
            if (characterMotor)
            {
                characterMotor.Motor.SetPositionAndRotation(newPosition, Quaternion.identity);
            }
        }
        private void CalculateSnapDestination()
        {
            if (base.isAuthority)
            {
                snapPosition = seamstressController.snapBackPosition;
                currentPosition = characterBody.corePosition;
                duration = (currentPosition - snapPosition).magnitude;
                duration = Util.Remap(duration, 0f, 100f, 0.2f, 1f);
                characterDirection.forward = snapPosition;
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (characterMotor)
            {
                characterMotor.velocity = Vector3.zero;
            }
            if (!hasSnapped)
            {
                SetPosition(Vector3.Lerp(currentPosition, snapPosition, fixedAge / duration));
            }
            if (fixedAge >= duration && base.isAuthority)
            {
                hasSnapped = true;
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            if (hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }
            if (cameraTargetParams)
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