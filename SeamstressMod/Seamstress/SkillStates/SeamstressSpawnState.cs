using RoR2;
using EntityStates.ImpMonster;
using UnityEngine;
using EntityStates;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using R2API;

namespace SeamstressMod.Seamstress.SkillStates
{
    public class SeamstressSpawnState : BaseState
    {
        private CameraRigController cameraController;

        public static float duration = 4f;

        private bool initCamera;

        private bool check = false;
        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active) characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge > 0.56f && !check)
            {
                check = true;
                EffectData effectData = new EffectData();
                effectData.origin = transform.position;
                effectData.scale = 0.1f;
                GameObject PLEASE = EntityStates.ImpBossMonster.SpawnState.spawnEffectPrefab.InstantiateClone("Spawn");
                GameObject PLEASE2 = EntityStates.ImpBossMonster.DeathState.initialEffect.InstantiateClone("Spawn2");
                EffectManager.SpawnEffect(PLEASE, effectData, false);
                EffectManager.SpawnEffect(PLEASE2, effectData, false);

                Util.PlaySound(BlinkState.beginSoundString, gameObject);

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

            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
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

