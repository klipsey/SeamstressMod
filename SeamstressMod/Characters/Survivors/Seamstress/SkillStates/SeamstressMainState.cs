using System;
using RoR2;
using EntityStates;
using UnityEngine;
using SeamstressMod.Survivors.Seamstress;

namespace SeamstressMod.SkillStates
{
    public class SeamstressMainState : GenericCharacterMain
    {
        private Animator animator;
        private SeamstressController seamCon;
        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = this.modelAnimator;
            seamCon = this.GetComponent<SeamstressController>();
        }
        public override void ProcessJump()
        {
            if (this.hasCharacterMotor)
            {
                bool hopooFeather = false;
                bool waxQuail = false;
                if (this.jumpInputReceived && base.characterBody && base.characterMotor.jumpCount < base.characterBody.maxJumpCount)
                {
                    int waxQuailCount = base.characterBody.inventory.GetItemCount(RoR2Content.Items.JumpBoost);
                    float horizontalBonus = 1f;
                    float verticalBonus = 1f;

                    if (base.characterMotor.jumpCount >= base.characterBody.baseJumpCount)
                    {
                        this.seamCon.hopoopFeatherTimer = 0.1f;
                        hopooFeather = true;
                        horizontalBonus = 1.5f;
                        verticalBonus = 1.5f;
                    }
                    else if (waxQuailCount > 0 && base.characterBody.isSprinting)
                    {
                        float v = base.characterBody.acceleration * base.characterMotor.airControl;

                        if (base.characterBody.moveSpeed > 0f && v > 0f)
                        {
                            waxQuail = true;
                            float num2 = Mathf.Sqrt(10f * (float)waxQuailCount / v);
                            float num3 = base.characterBody.moveSpeed / v;
                            horizontalBonus = (num2 + num3) / num3;
                        }
                    }

                    if(seamCon.blinkReady)
                    {
                        GenericCharacterMain.ApplyJumpVelocity(base.characterMotor, base.characterBody, horizontalBonus, verticalBonus, false);
                    }

                    if (hasModelAnimator)
                    {
                        int layerIndex = base.modelAnimator.GetLayerIndex("Body");
                        if (layerIndex >= 0)
                        {
                            if (base.characterMotor.jumpCount == 0 || base.characterBody.baseJumpCount == 1)
                            {
                                base.modelAnimator.CrossFadeInFixedTime("Jump", smoothingParameters.intoJumpTransitionTime, layerIndex);
                            }
                            else
                            {
                                base.modelAnimator.CrossFadeInFixedTime("BonusJump", smoothingParameters.intoJumpTransitionTime, layerIndex);
                            }
                        }
                    }

                    if (hopooFeather)
                    {
                        EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/FeatherEffect"), new EffectData
                        {
                            origin = base.characterBody.footPosition
                        }, true);
                    }
                    else if (base.characterMotor.jumpCount > 0)
                    {
                        EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/CharacterLandImpact"), new EffectData
                        {
                            origin = base.characterBody.footPosition,
                            scale = base.characterBody.radius
                        }, true);
                    }

                    if (waxQuail)
                    {
                        EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BoostJumpEffect"), new EffectData
                        {
                            origin = base.characterBody.footPosition,
                            rotation = Util.QuaternionSafeLookRotation(base.characterMotor.velocity)
                        }, true);
                    }

                    if (seamCon.blinkReady)
                    {
                        base.characterMotor.jumpCount++;
                    }
                    #region For later? thank you rob
                    /*
                    if (this.animator)
                    {
                        float x = this.animatorWalkParamCalculator.animatorWalkSpeed.y;
                        float y = this.animatorWalkParamCalculator.animatorWalkSpeed.x;

                        if (Mathf.Abs(x) <= 0.45f && Mathf.Abs(y) <= 0.45f || this.inputBank.moveVector == Vector3.zero)
                        {
                            x = 0f;
                            y = 0f;
                        }

                        if (Mathf.Abs(x) > Mathf.Abs(y))
                        {
                            // side flip
                            if (x > 0f) x = 1f;
                            else x = -1f;
                            y = 0f;
                        }
                        else if (Mathf.Abs(x) < Mathf.Abs(y))
                        {
                            // forward/backflips
                            if (y > 0f) y = 1f;
                            else y = -1f;
                            x = 0f;
                        }

                        this.animator.SetFloat("forwardSpeedCached", y);
                        this.animator.SetFloat("rightSpeedCached", x);

                    } */
                    #endregion
                }
            }
        }
    }
}
