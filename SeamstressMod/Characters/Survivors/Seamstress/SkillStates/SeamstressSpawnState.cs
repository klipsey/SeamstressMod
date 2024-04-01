using RoR2;
using EntityStates.ImpMonster;
using UnityEngine;
using EntityStates;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using SeamstressMod.Survivors.Seamstress;
using R2API;

namespace SeamstressMod.SkillStates
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
            if (NetworkServer.active) base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge > 0.56f && !this.check)
            {
                this.check = true;
                EffectData effectData = new EffectData();
                effectData.origin = base.transform.position;
                effectData.scale = 0.1f;
                GameObject PLEASE = EntityStates.ImpBossMonster.SpawnState.spawnEffectPrefab.InstantiateClone("Spawn");
                GameObject PLEASE2 = EntityStates.ImpBossMonster.DeathState.initialEffect.InstantiateClone("Spawn2");
                EffectManager.SpawnEffect(PLEASE, effectData, false);
                EffectManager.SpawnEffect(PLEASE2, effectData, false); 

                Util.PlaySound(EntityStates.ImpMonster.BlinkState.beginSoundString, this.gameObject);

                Transform modelTransform = this.GetModelTransform();
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

            if (!this.cameraController)
            {
                if (base.characterBody && base.characterBody.master)
                {
                    if (base.characterBody.master.playerCharacterMasterController)
                    {
                        if (base.characterBody.master.playerCharacterMasterController.networkUser)
                        {
                            this.cameraController = base.characterBody.master.playerCharacterMasterController.networkUser.cameraRigController;
                        }
                    }
                }
            }
            else
            {
                if (!this.initCamera)
                {
                    this.initCamera = true;
                    ((RoR2.CameraModes.CameraModePlayerBasic.InstanceData)this.cameraController.cameraMode.camToRawInstanceData[this.cameraController]).SetPitchYawFromLookVector(-base.characterDirection.forward);
                }
            }

            if (base.fixedAge >= SeamstressSpawnState.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (NetworkServer.active) base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}

