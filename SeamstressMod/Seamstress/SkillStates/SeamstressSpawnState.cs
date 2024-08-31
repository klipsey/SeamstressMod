using RoR2;
using EntityStates.ImpMonster;
using UnityEngine;
using EntityStates;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using R2API;
using SeamstressMod.Seamstress.Content;
using SeamstressMod.Modules.BaseStates;

namespace SeamstressMod.Seamstress.SkillStates
{
    public class SeamstressSpawnState : BaseSeamstressState
    {
        private CameraRigController cameraController;

        public GameObject spawnPrefab = SeamstressAssets.spawnPrefab;

        public static float duration = 3f;

        private bool initCamera;

        private bool check = false;
        public override void OnEnter()
        {
            base.OnEnter();

            if (NetworkServer.active) characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);

            PlayAnimation("Body", "Spawn");

            Transform modelTransform = GetModelTransform();
            if (modelTransform)
            {
                TemporaryOverlayInstance temporaryInstance = TemporaryOverlayManager.AddOverlay(base.gameObject);
                temporaryInstance.duration = 1.5f;
                temporaryInstance.animateShaderAlpha = true;
                temporaryInstance.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryInstance.destroyComponentOnEnd = true;
                temporaryInstance.originalMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDissolve.mat").WaitForCompletion();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge > 1f && !check)
            {
                RefreshState();
                if (seamstressController.blue) spawnPrefab = SeamstressAssets.spawnPrefab2;
                check = true;
                EffectData effectData = new EffectData();
                effectData.origin = transform.position;
                EffectManager.SpawnEffect(spawnPrefab, effectData, false);

                Util.PlaySound("sfx_seamstress_spawn", gameObject);
            }

            if (!cameraController)
            {
                if (characterBody && characterBody.master)
                {
                    if (characterBody.master.playerCharacterMasterController)
                    {
                        if (characterBody.master.playerCharacterMasterController.networkUser)
                        {
                            cameraController = characterBody.master.playerCharacterMasterController.networkUser.cameraRigController;
                        }
                    }
                }
            }
            else
            {
                if (!initCamera)
                {
                    initCamera = true;
                    ((RoR2.CameraModes.CameraModePlayerBasic.InstanceData)cameraController.cameraMode.camToRawInstanceData[cameraController]).SetPitchYawFromLookVector(-characterDirection.forward);
                }
            }

            if (base.fixedAge >= duration && base.isAuthority)
            {
                base.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (NetworkServer.active) characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}

