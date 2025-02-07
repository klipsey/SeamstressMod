using System.Linq;
using RoR2;
using RoR2.Navigation;
using UnityEngine;
using SeamstressMod.Modules.BaseStates;
using R2API;
using UnityEngine.Networking;
using EntityStates;
using UnityEngine.Networking.Match;
using SeamstressMod.Seamstress.Content;

namespace SeamstressMod.Seamstress.SkillStates
{
    public class SeamstressBlink : BaseSeamstressSkillState
    {
        protected Transform modelTransform;

        public GameObject dashPrefab = SeamstressAssets.impDashEffect;
        public GameObject blinkPrefab = SeamstressAssets.smallBlinkEffect;
        public Material destealthMaterial = SeamstressAssets.destealthMaterial;

        protected CameraTargetParams.AimRequest request;

        protected Vector3 blinkVector;
        protected Vector3 slipVector = Vector3.zero;
        private Vector3 cachedForward;

        private Quaternion slideRotation;

        public float duration = 0.1f;

        protected float speedCoefficient;

        public static string beginSoundString = "Play_imp_attack_blink";

        public string animationLayer = "FullBody, Override";

        public string animString = "Dash";

        protected CharacterModel characterModel;

        protected HurtBoxGroup hurtboxGroup;

        public bool split;

        protected Animator animator;

        public override void OnEnter()
        {
            RefreshState();

            if (seamstressController.blue)
            {
                dashPrefab = SeamstressAssets.impDashEffect2;
                blinkPrefab = SeamstressAssets.smallBlinkEffect2;
                destealthMaterial = SeamstressAssets.destealthMaterial2;
            }

            base.OnEnter();
            Util.PlaySound(beginSoundString, gameObject);
            modelTransform = GetModelTransform();
            if (modelTransform)
            {
                characterModel = modelTransform.GetComponent<CharacterModel>();
                hurtboxGroup = modelTransform.GetComponent<HurtBoxGroup>();
                animator = modelTransform.GetComponent<Animator>();
            }
            if (characterModel)
            {
                characterModel.invisibilityCount++;
            }
            if (hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }
            if (cameraTargetParams)
            {
                request = cameraTargetParams.RequestAimType(CameraTargetParams.AimType.Aura);
            }
            blinkVector = GetBlinkVector();
            if (characterMotor.isGrounded)
            {
                characterMotor.velocity = Vector3.zero;
                if(SeamstressConfig.changeGroundedBlinkVelocity.Value > 0f)
                {
                    Vector3 to = inputBank.aimDirection;
                    to.y = 0f;
                    if (inputBank.aimDirection.y < 0f && (Vector3.Angle(to, blinkVector) <= 90) || inputBank.aimDirection.y > 0f && (Vector3.Angle(to, blinkVector) >= 90))
                    {
                        blinkVector.y *= -1;
                    }
                    if (Vector3.Angle(inputBank.aimDirection, to) <= 15)
                    {
                        blinkVector.y = SeamstressConfig.changeGroundedBlinkVelocity.Value;
                    }
                }
                blinkVector.y = Mathf.Clamp(blinkVector.y, 0.1f, 0.75f);
            }
            characterDirection.moveVector = blinkVector;

            CreateBlinkEffect(base.characterBody.corePosition, true);

            int waxQuailCount = base.characterBody.inventory.GetItemCount(RoR2Content.Items.JumpBoost);
            float horizontalBonus = 1f;

            if (waxQuailCount > 0 && base.characterBody.isSprinting)
            {
                float v = base.characterBody.acceleration * characterMotor.airControl;

                if (base.characterBody.moveSpeed > 0f && v > 0f)
                {
                    float num2 = Mathf.Sqrt(10f * waxQuailCount / v);
                    float num3 = characterBody.moveSpeed / v;
                    horizontalBonus = (num2 + num3) / num3;
                }
            }

            speedCoefficient = 0.3f * characterBody.jumpPower * Mathf.Clamp((characterBody.moveSpeed * horizontalBonus) / 4f, 5f, 20f);
            gameObject.layer = LayerIndex.fakeActor.intVal;
            characterMotor.Motor.RebuildCollidableLayers();
        }

        protected virtual Vector3 GetBlinkVector()
        {
            Vector3 aimDirection = inputBank.aimDirection;
            aimDirection.y = 0f;
            Vector3 axis = -Vector3.Cross(Vector3.up, aimDirection);
            float num = Vector3.Angle(inputBank.aimDirection, aimDirection);
            if (inputBank.aimDirection.y < 0f)
            {
                num = 0f - num;
            }
            return Vector3.Normalize(Quaternion.AngleAxis(num, axis) * inputBank.moveVector);
        }
        protected void CreateBlinkEffect(Vector3 origin, bool first)
        {
            if (blinkPrefab)
            {
                EffectData effectData = new EffectData();
                effectData.rotation = Util.QuaternionSafeLookRotation(blinkVector);
                effectData.origin = origin;
                effectData.scale = 0.15f;
                EffectManager.SpawnEffect(blinkPrefab, effectData, transmit: true);
                effectData.scale = 3f;
                if(!first)
                {
                    EffectManager.SpawnEffect(dashPrefab, effectData, transmit: true);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && characterMotor && characterDirection)
            {
                characterMotor.Motor.ForceUnground();
                characterMotor.velocity = blinkVector * speedCoefficient;
            }
            if (base.isAuthority && fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
        }
        public override void OnExit()
        {
            gameObject.layer = LayerIndex.defaultLayer.intVal;
            characterMotor.Motor.RebuildCollidableLayers();
            if (!outer.destroying)
            {
                CreateBlinkEffect(base.characterBody.corePosition, false);
                modelTransform = GetModelTransform();
                if (modelTransform && this.destealthMaterial)
                {
                    TemporaryOverlayInstance temporaryOverlayInstance = TemporaryOverlayManager.AddOverlay(base.gameObject);
                    temporaryOverlayInstance.duration = 1f;
                    temporaryOverlayInstance.destroyComponentOnEnd = true;
                    temporaryOverlayInstance.originalMaterial = this.destealthMaterial;
                    temporaryOverlayInstance.inspectorCharacterModel = animator.gameObject.GetComponent<CharacterModel>();
                    temporaryOverlayInstance.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlayInstance.animateShaderAlpha = true;
                }
                if (characterModel)
                {
                    characterModel.invisibilityCount--;
                }
                if (hurtboxGroup)
                {
                    HurtBoxGroup hurtBoxGroup = hurtboxGroup;
                    int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
                    hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
                }
                if (cameraTargetParams)
                {
                    request.Dispose();
                }
            }
            base.OnExit();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(blinkVector);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            blinkVector = reader.ReadVector3();
        }
    }
}