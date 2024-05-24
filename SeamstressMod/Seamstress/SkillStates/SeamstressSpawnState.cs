using RoR2;
using EntityStates.ImpMonster;
using UnityEngine;
using EntityStates;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using R2API;
using SeamstressMod.Seamstress.Content;

namespace SeamstressMod.Seamstress.SkillStates
{
    public class SeamstressSpawnState : BaseState
    {
        private CameraRigController cameraController;

        public static float duration = 3f;

        private bool initCamera;

        private bool check = false;
        public override void OnEnter()
        {
            base.OnEnter();

            if (NetworkServer.active) characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);

            PlayAnimation("Body", "Spawn");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge > 1f && !check)
            {
                check = true;
                EffectData effectData = new EffectData();
                effectData.origin = transform.position;
                EffectManager.SpawnEffect(SeamstressAssets.spawnPrefab, effectData, false);

                Util.PlaySound("sfx_seamstress_spawn", gameObject);

                Transform modelTransform = GetModelTransform();
                if (modelTransform)
                {
                    TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = 0.6f;
                    temporaryOverlay.animateShaderAlpha = true;
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDissolve.mat").WaitForCompletion();
                    temporaryOverlay.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());
                }
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

