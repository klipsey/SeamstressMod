﻿using System.Linq;
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
    public class SeamstressBlinkUp : BaseSeamstressSkillState
    {
        protected Transform modelTransform;

        protected GameObject dashPrefab = SeamstressAssets.impDashEffect; 

        protected GameObject blinkPrefab = SeamstressAssets.smallBlinkEffect;

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
            Util.PlaySound(beginSoundString, gameObject);
            PlayAnimation("FullBody, Override", "Roll", "Roll.playbackRate", duration);
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
            characterMotor.velocity = Vector3.zero;
            characterDirection.moveVector = blinkVector;
            CreateBlinkEffect(base.characterBody.corePosition);
            speedCoefficient = 0.3f * characterBody.jumpPower * 3f;
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
                    EffectManager.SpawnEffect(blinkPrefab, effectData, transmit: false);
                    effectData.scale = 3f;
                    EffectManager.SpawnEffect(dashPrefab, effectData, transmit: false);
                }
            }
            else split = false;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (characterMotor && characterDirection && isAuthority)
            {
                characterMotor.Motor.ForceUnground();
                characterMotor.velocity = blinkVector * speedCoefficient;
            }
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }
        public override void OnExit()
        {
            if (!outer.destroying)
            {
                CreateBlinkEffect(base.characterBody.corePosition);
                modelTransform = GetModelTransform();
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
                if (characterMotor)
                {
                    characterMotor.disableAirControlUntilCollision = false;
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