using System;
using RoR2;
using EntityStates;
using UnityEngine;
using SeamstressMod.Seamstress.Components;

namespace SeamstressMod.Seamstress.SkillStates
{
    public class MainState : GenericCharacterMain
    {
        private Animator animator;
        private SeamstressController seamCom;
        public override void OnEnter()
        {
            base.OnEnter();
            animator = modelAnimator;
            seamCom = GetComponent<SeamstressController>();
        }
        public override void ProcessJump()
        {
            if (base.hasCharacterMotor && seamCom.blinkReady)
            {
                bool hopooFeather = false;
                bool waxQuail = false;
                if (base.jumpInputReceived && base.characterBody && base.characterMotor.jumpCount < base.characterBody.maxJumpCount)
                {
                    int waxQuailCount = base.characterBody.inventory.GetItemCount(RoR2Content.Items.JumpBoost);
                    float horizontalBonus = 1f;
                    float verticalBonus = 1f;

                    if (characterMotor.jumpCount >= base.characterBody.baseJumpCount)
                    {
                        hopooFeather = true;
                        horizontalBonus = 1.5f;
                        verticalBonus = 1.5f;
                    }
                    else if (waxQuailCount > 0 && base.characterBody.isSprinting)
                    {
                        float v = base.characterBody.acceleration * characterMotor.airControl;

                        if (base.characterBody.moveSpeed > 0f && v > 0f)
                        {
                            waxQuail = true;
                            float num2 = Mathf.Sqrt(10f * waxQuailCount / v);
                            float num3 = characterBody.moveSpeed / v;
                            horizontalBonus = (num2 + num3) / num3;
                        }
                    }

                    base.characterMotor.jumpCount++;

                    if (this.hasModelAnimator)
                    {
                        int layerIndex = modelAnimator.GetLayerIndex("Body");
                        if (base.inputBank.moveVector == Vector3.zero)
                        {
                            if (layerIndex >= 0)
                            {
                                modelAnimator.CrossFadeInFixedTime("Jump", smoothingParameters.intoJumpTransitionTime, layerIndex);
                            }
                        }
                        else
                        {
                            layerIndex = modelAnimator.GetLayerIndex("FullBody, Override");
                            if (layerIndex >= 0)
                            {
                                Vector3 slipVector = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
                                Vector3 cachedForward = this.characterDirection.forward;
                                Vector3 rhs = base.characterDirection ? base.characterDirection.forward : slipVector;
                                Vector3 rhs2 = Vector3.Cross(Vector3.up, rhs);
                                float num = Vector3.Dot(slipVector, rhs);
                                float num2 = Vector3.Dot(slipVector, rhs2);
                                modelAnimator.SetFloat("dashF", num);
                                modelAnimator.SetFloat("dashR", num2);
                                modelAnimator.CrossFadeInFixedTime("Dash", smoothingParameters.intoJumpTransitionTime, layerIndex);
                            }
                        }
                    }
                    if (hopooFeather)
                    {
                        EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/FeatherEffect"), new EffectData
                        {
                            origin = characterBody.footPosition
                        }, false);
                    }
                    else if (characterMotor.jumpCount > 0)
                    {
                        EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/CharacterLandImpact"), new EffectData
                        {
                            origin = characterBody.footPosition,
                            scale = characterBody.radius
                        }, false);
                    }

                    if (waxQuail)
                    {
                        EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BoostJumpEffect"), new EffectData
                        {
                            origin = characterBody.footPosition,
                            rotation = Util.QuaternionSafeLookRotation(characterMotor.velocity)
                        }, false);
                    }
                }
            }
        }
    }
}
