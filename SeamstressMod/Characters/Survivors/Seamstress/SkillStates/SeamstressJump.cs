using UnityEngine;
using RoR2;
using EntityStates;
using RoR2.Projectile;
using SeamstressMod.Modules.BaseStates;
using UnityEngine.Networking;
using SeamstressMod.Survivors.Seamstress;
using System;

namespace SeamstressMod.SkillStates
{
    public class SeamstressJump : BaseSeamstressSkillState
    {
        private float blinkCd;

        private bool hasNeedles;

        private Ray aimRay;

        private GameObject projectilePrefab;
        public override void OnEnter()
        {
            base.OnEnter();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active) hasNeedles = characterBody.HasBuff(SeamstressBuffs.needles);
            if (empowered)
            {
                this.projectilePrefab = SeamstressAssets.needleButcheredPrefab;
            }
            else
            {
                this.projectilePrefab = SeamstressAssets.needlePrefab;
            }
            if(blinkCd > 0) blinkCd -= Time.fixedDeltaTime;
            if ((base.characterMotor.jumpCount < base.characterBody.maxJumpCount || hasNeedles) && (blinkCd <= 0 || !this.isGrounded)) this.seamCon.blinkReady = true;
            if (this.inputBank.jump.justPressed && this.isGrounded)
            {
                if (this.seamCon.blinkReady)
                {
                    this.seamCon.blinkReady = false;
                    blinkCd = 0.5f;
                    if (this.inputBank.moveVector != Vector3.zero) this.BlinkForward();
                    else BlinkUp();
                    return;
                }
            }
            else if (this.inputBank.jump.justPressed && !this.isGrounded)
            {
                if (this.seamCon.blinkReady)
                {
                    this.seamCon.blinkReady = false;
                    blinkCd = 0.5f;
                    if(base.characterMotor.jumpCount == base.characterBody.maxJumpCount)
                    {
                        if (NetworkServer.active) characterBody.RemoveBuff(SeamstressBuffs.needles);
                    }
                    if (base.isAuthority)
                    {
                        aimRay = base.GetAimRay();
                        ProjectileManager.instance.FireProjectile(this.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), gameObject, damageStat * SeamstressStaticValues.needleDamageCoefficient, 0f, RollCrit(), DamageColorIndex.Default, null, -1f);
                    }
                    if (this.inputBank.moveVector != Vector3.zero) this.BlinkForward();
                    else BlinkUp();
                    return;
                }
            }
        }
        private void BlinkForward()
        {
            EntityStateMachine.FindByCustomName(this.gameObject, "Blink").SetInterruptState(new SeamstressBlink(), InterruptPriority.Any);
        }
        private void BlinkUp()
        {
            EntityStateMachine.FindByCustomName(this.gameObject, "Blink").SetInterruptState(new SeamstressBlinkUp(), InterruptPriority.Any);
        }
    }
}