using UnityEngine;
using RoR2;
using EntityStates;
using RoR2.Projectile;
using SeamstressMod.Modules.BaseStates;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using SeamstressMod.Seamstress;
using SeamstressMod.Seamstress.Content;

namespace SeamstressMod.Seamstress.SkillStates
{
    public class SeamstressJump : BaseSeamstressState
    {
        private bool hasNeedles;

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
            base.FixedUpdate();
            if (needleCount > 0) hasNeedles = true;
            else hasNeedles = false;
            if (empowered)
            {
                projectilePrefab = SeamstressAssets.needleButcheredPrefab;
            }
            else
            {
                projectilePrefab = SeamstressAssets.needlePrefab;
            }
            //Log.Debug("blinkCD: " + seamCon.blinkCd);
            if ((characterMotor.jumpCount < characterBody.maxJumpCount || hasNeedles) && (seamCon.blinkCd <= 0 || !isGrounded)) seamCon.RefreshBlink();
            if (inputBank.jump.justPressed && isGrounded && seamCon.blinkReady)
            {
                seamCon.blinkReady = false;
                seamCon.blinkCd = SeamstressStaticValues.blinkCooldown;
                if (inputBank.moveVector != Vector3.zero) BlinkForward();
                else BlinkUp();
                return;
            }
            else if (inputBank.jump.justPressed && characterMotor.jumpCount >= characterBody.maxJumpCount && seamCon.blinkReady)
            {
                seamCon.blinkReady = false;
                seamCon.blinkCd = SeamstressStaticValues.blinkCooldown;
                if (characterMotor.jumpCount >= characterBody.maxJumpCount)
                {
                    Util.PlaySound("Play_bandit2_m2_alt_throw", gameObject);
                    if (NetworkServer.active) characterBody.RemoveBuff(SeamstressBuffs.needles);
                }
                if (isAuthority)
                {
                    aimRay = GetAimRay();
                    aimRay.direction = Util.ApplySpread(aimRay.direction, minSpread, maxSpread, 1f, 1f, 0f, projectilePitchBonus);

                    if (characterBody.inventory && characterBody.inventory.GetItemCount(DLC1Content.Items.MoreMissile) > 0)
                    {
                        float damageMult = SeamstressSurvivor.GetICBMDamageMult(characterBody);

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
                return;
            }
        }
        private void BlinkForward()
        {
            EntityStateMachine.FindByCustomName(gameObject, "Body").SetInterruptState(new SeamstressBlink(), InterruptPriority.Any);
        }
        private void BlinkUp()
        {
            EntityStateMachine.FindByCustomName(gameObject, "Body").SetInterruptState(new SeamstressBlinkUp(), InterruptPriority.Any);
        }
    }
}