﻿using System.Linq;
using RoR2;
using RoR2.Navigation;
using UnityEngine;
using SeamstressMod.Modules.BaseStates;
using SeamstressMod.Survivors.Seamstress;
using R2API;
using UnityEngine.Networking;
using EntityStates;
using UnityEngine.Networking.Match;

namespace SeamstressMod.SkillStates
{
    public class SeamstressBlinkUp : BaseSeamstressSkillState
    {
        protected Transform modelTransform;

        protected GameObject blinkPrefab = SeamstressAssets.smallBlinkPrefab;

        protected CameraTargetParams.AimRequest request;

        protected Vector3 blinkVector = Vector3.up;

        public float duration = 0.2f;

        public float speedCoefficient = 30f;

        public static string beginSoundString = "Play_imp_attack_blink";

        protected CharacterModel characterModel;

        protected HurtBoxGroup hurtboxGroup;

        public bool split;

        protected Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(beginSoundString, base.gameObject);
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
            if (base.cameraTargetParams)
            {
                request = base.cameraTargetParams.RequestAimType(CameraTargetParams.AimType.Aura);
            }
            if (characterMotor.isGrounded) base.characterMotor.velocity = Vector3.zero;
            base.characterDirection.forward = blinkVector;
            CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
            base.characterBody.SetAimTimer(0f);
        }
        protected void CreateBlinkEffect(Vector3 origin)
        {
            if (!split)
            {
                if (blinkPrefab)
                {
                    EffectData effectData = new EffectData();
                    effectData.rotation = Util.QuaternionSafeLookRotation(blinkVector);
                    effectData.origin = origin;
                    effectData.scale = 0.15f;
                    EffectManager.SpawnEffect(blinkPrefab, effectData, transmit: true);
                }
            }
            else split = false;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.characterMotor && base.characterDirection)
            {
                base.characterMotor.rootMotion += blinkVector * (speedCoefficient * Time.fixedDeltaTime);
            }
            if (base.fixedAge >= duration && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }
        public override void OnExit()
        {
            if (!outer.destroying)
            {
                CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
                modelTransform = GetModelTransform();
                if (this.modelTransform && SeamstressAssets.destealthMaterial)
                {
                    TemporaryOverlay temporaryOverlay = animator.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = 1f;
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = SeamstressAssets.destealthMaterial;
                    temporaryOverlay.inspectorCharacterModel = animator.gameObject.GetComponent<CharacterModel>();
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay.animateShaderAlpha = true;
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
                if (base.characterMotor)
                {
                    base.characterMotor.disableAirControlUntilCollision = false;
                }
                if (base.cameraTargetParams)
                {
                    request.Dispose();
                }
                if(base.isAuthority) SmallHop(base.characterMotor, 3f);
            }
            base.OnExit();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
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