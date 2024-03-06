using UnityEngine;
using RoR2;
using EntityStates;
using RoR2.Projectile;
using SeamstressMod.Modules.BaseStates;
using UnityEngine.Networking;
using SeamstressMod.Survivors.Seamstress;
using UnityEngine.AddressableAssets;

namespace SeamstressMod.SkillStates
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
                this.projectilePrefab = SeamstressAssets.needleButcheredPrefab;
            }
            else
            {
                this.projectilePrefab = SeamstressAssets.needlePrefab;
            }
            //Log.Debug("blinkCD: " + seamCon.blinkCd);
            if ((base.characterMotor.jumpCount < base.characterBody.maxJumpCount || hasNeedles) && (this.seamCon.blinkCd <= 0 || !this.isGrounded)) this.seamCon.RefreshBlink();
            if (this.inputBank.jump.justPressed && this.isGrounded)
            {
                if (this.seamCon.blinkReady)
                {
                    this.seamCon.blinkReady = false;
                    this.seamCon.blinkCd = SeamstressStaticValues.blinkCooldown;
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
                    this.seamCon.blinkCd = SeamstressStaticValues.blinkCooldown;
                    if(base.characterMotor.jumpCount == base.characterBody.maxJumpCount)
                    {
                        if (NetworkServer.active) GetComponent<NeedleController>().RpcRemoveNeedle();
                    }
                    if (base.isAuthority)
                    {
                        aimRay = base.GetAimRay();
                        aimRay.direction = Util.ApplySpread(aimRay.direction, this.minSpread, this.maxSpread, 1f, 1f, 0f, this.projectilePitchBonus);

                        if (this.characterBody.inventory && this.characterBody.inventory.GetItemCount(DLC1Content.Items.MoreMissile) > 0)
                        {
                            float damageMult = SeamstressSurvivor.GetICBMDamageMult(this.characterBody);

                            Vector3 rhs = Vector3.Cross(Vector3.up, aimRay.direction);
                            Vector3 axis = Vector3.Cross(aimRay.direction, rhs);

                            float currentSpread = 20f;
                            float angle = 0f;
                            float num2 = 0f;
                            num2 = UnityEngine.Random.Range(1f + currentSpread, 1f + currentSpread) * 3f;   //Bandit is x2
                            angle = num2 / 2f;  //3 - 1 rockets

                            Vector3 direction = Quaternion.AngleAxis(-num2 * 0.5f, axis) * aimRay.direction;
                            Quaternion rotation = Quaternion.AngleAxis(angle, axis);
                            Ray aimRay2 = new Ray(aimRay.origin, direction);
                            for (int i = 0; i < 3; i++)
                            {
                                ProjectileManager.instance.FireProjectile(this.projectilePrefab, aimRay2.origin, Util.QuaternionSafeLookRotation(aimRay2.direction), this.gameObject, damageMult * damageStat * SeamstressStaticValues.needleDamageCoefficient, 0f, this.RollCrit(), DamageColorIndex.Default, null, -1f);
                                aimRay2.direction = rotation * aimRay2.direction;
                            }
                        }
                        else
                        {
                            ProjectileManager.instance.FireProjectile(this.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), gameObject, damageStat * SeamstressStaticValues.needleDamageCoefficient, 0f, RollCrit(), DamageColorIndex.Default, null, -1f);
                        }
                        if (this.inputBank.moveVector != Vector3.zero) this.BlinkForward();
                        else BlinkUp();
                    }
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