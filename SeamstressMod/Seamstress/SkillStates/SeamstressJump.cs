using UnityEngine;
using RoR2;
using EntityStates;
using RoR2.Projectile;
using SeamstressMod.Modules.BaseStates;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using SeamstressMod.Seamstress;
using SeamstressMod.Seamstress.Content;
using SeamstressMod.Seamstress.Components;

namespace SeamstressMod.Seamstress.SkillStates
{
    public class SeamstressJump : BaseSeamstressState
    {
        public float minSpread;

        public float maxSpread;

        public float projectilePitchBonus;

        private Ray aimRay;

        private GameObject projectilePrefab;
        public override void OnEnter()
        {
            base.OnEnter();
        }
        public override void FixedUpdate()
        {
            RefreshState();
            base.FixedUpdate();
            if (this.insatiable)
            {
                if (seamstressController.blue) projectilePrefab = SeamstressAssets.needleButcheredPrefab2;
                else projectilePrefab = SeamstressAssets.needleButcheredPrefab;
            }
            else
            {
                if (seamstressController.blue) projectilePrefab = SeamstressAssets.needlePrefab2;
                else projectilePrefab = SeamstressAssets.needlePrefab;
            }
            if (((this.characterBody.characterMotor.jumpCount < this.characterBody.maxJumpCount || this.characterBody.GetBuffCount(SeamstressBuffs.Needles) > 0) && this.seamstressController.blinkCd >= SeamstressStaticValues.blinkCooldown) && this.seamstressController.blinkReady == false)
            {
                seamstressController.blinkCd = 0f;
                seamstressController.blinkReady = true;
            }
            if (base.inputBank.jump.justPressed && base.isGrounded && seamstressController.blinkReady)
            {
                this.seamstressController.blinkReady = false;
                int waxQuailCount = base.characterBody.inventory.GetItemCount(RoR2Content.Items.JumpBoost);
                float horizontalBonus = 1f;
                float verticalBonus = 1f;

                if (characterMotor.jumpCount > base.characterBody.baseJumpCount)
                {
                    horizontalBonus = 1.5f;
                    verticalBonus = 1.5f;
                }
                else if (waxQuailCount > 0 && base.characterBody.isSprinting)
                {
                    float v = base.characterBody.acceleration * characterMotor.airControl;

                    if (base.characterBody.moveSpeed > 0f && v > 0f)
                    {
                        float num2 = Mathf.Sqrt(10f * waxQuailCount / v);
                        float num3 = characterBody.moveSpeed / v;
                        horizontalBonus = (num2 + num3) / num3;
                    }
                }
                GenericCharacterMain.ApplyJumpVelocity(base.characterMotor, base.characterBody, horizontalBonus, verticalBonus, false);
                return;
            }
            else if (base.inputBank.jump.justPressed && !base.isGrounded && seamstressController.blinkReady)
            {
                seamstressController.blinkReady = false;
                if (base.characterMotor.jumpCount >= base.characterBody.maxJumpCount && this.characterBody.inventory.GetItemCount(RoR2.RoR2Content.Items.Feather) > 0 && !seamstressController.hopooHasHopped)
                {
                    if (!seamstressController.hopooHasHopped) seamstressController.hopooHasHopped = true;
                    Util.PlaySound("Play_bandit2_m2_alt_throw", gameObject);
                }
                else if(base.characterMotor.jumpCount >= base.characterBody.maxJumpCount)
                {
                    base.needleCon.consumeNeedle = true;
                    Util.PlaySound("Play_bandit2_m2_alt_throw", gameObject);
                }
                else
                {
                    Util.PlaySound("Play_bandit2_m2_alt_throw", gameObject);
                }
                if (base.isAuthority)
                {
                    aimRay = GetAimRay();
                    aimRay.direction = Util.ApplySpread(aimRay.direction, minSpread, maxSpread, 1f, 1f, 0f, projectilePitchBonus);
                    if (characterBody.inventory && characterBody.inventory.GetItemCount(DLC1Content.Items.MoreMissile) > 0)
                    {
                        float damageMult = GetICBMDamageMult(characterBody);

                        Vector3 rhs = Vector3.Cross(Vector3.up, aimRay.direction);
                        Vector3 axis = Vector3.Cross(aimRay.direction, rhs);

                        float currentSpread = 20f;
                        float angle = 0f;
                        float num2 = 0f;
                        num2 = Random.Range(1f + currentSpread, 1f + currentSpread) * 3f;   //Bandit is x2
                        angle = num2 / 2f;  //3 - 1 rockets

                        Vector3 direction = Quaternion.AngleAxis(-num2 * 0.5f, axis) * aimRay.direction;
                        Quaternion rotation = Quaternion.AngleAxis(angle, axis);
                        Ray aimRay2 = new Ray(aimRay.origin, direction);
                        for (int i = 0; i < 3; i++)
                        {
                            ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay2.origin, Util.QuaternionSafeLookRotation(aimRay2.direction), gameObject, damageMult * damageStat * SeamstressStaticValues.needleDamageCoefficient, 0f, RollCrit(), DamageColorIndex.Default, null, -1f);
                            aimRay2.direction = rotation * aimRay2.direction;
                        }
                    }
                    else
                    {
                        ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), gameObject, damageStat * SeamstressStaticValues.needleDamageCoefficient, 0f, RollCrit(), DamageColorIndex.Default, null, -1f);
                    }
                    if (inputBank.moveVector != Vector3.zero) BlinkForward();
                    else BlinkUp();
                }
            }
        }
        public static float GetICBMDamageMult(CharacterBody body)
        {
            float mult = 1f;
            if (body && body.inventory)
            {
                int itemcount = body.inventory.GetItemCount(DLC1Content.Items.MoreMissile);
                int stack = itemcount - 1;
                if (stack > 0) mult += stack * 0.5f;
            }
            return mult;
        }

        private void BlinkForward()
        {
            EntityStateMachine.FindByCustomName(gameObject, "Passive").SetInterruptState(new SeamstressBlink(), InterruptPriority.Any);
        }
        private void BlinkUp()
        {
            EntityStateMachine.FindByCustomName(gameObject, "Passive").SetInterruptState(new SeamstressBlinkUp(), InterruptPriority.Any);
        }
    }
}