using System;
using System.Collections.Generic;
using System.Text;
using EntityStates;
using RoR2;
using UnityEngine;

namespace SeamstressMod.SkillStates.BaseStates
{
    public class ButcheredOverlayState : BaseState
    {
        private float duration;

        private TemporaryOverlay temporaryOverlay;

        public float bloodBathDuration = 6f;

        public override void OnEnter()
        {
            base.OnEnter();
            Transform modelTransform = GetModelTransform();
            if ((bool)modelTransform)
            {
                CharacterModel component = modelTransform.GetComponent<CharacterModel>();
                if ((bool)component)
                {
                    temporaryOverlay = base.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = bloodBathDuration;
                    temporaryOverlay.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matFullCrit");
                    temporaryOverlay.AddToCharacerModel(component);
                    duration = bloodBathDuration;
                }
            }
        }

        public override void OnExit()
        {
            if ((bool)temporaryOverlay)
            {
                EntityState.Destroy(temporaryOverlay);
            }
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && base.fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
        }
    }
}
