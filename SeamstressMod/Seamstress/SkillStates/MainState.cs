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

                    ApplyJumpVelocity(base.characterMotor, base.characterBody, horizontalBonus, verticalBonus, false);

                    base.characterMotor.jumpCount++;

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
