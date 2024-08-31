using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using SeamstressMod.Seamstress.Content;
using static RoR2.DotController;

namespace SeamstressMod.Seamstress.Components
{
    public class SeamstressBleedVisualController : MonoBehaviour
    {
        private Transform bleedPosition;

        private GameObject stitchDot;

        private CharacterBody characterBody;
        private void Awake()
        {
            bleedPosition = transform;
            characterBody = GetComponent<CharacterBody>();
        }
        private void FixedUpdate()
        {
            if (!NetworkServer.active) return;
            if (characterBody.HasBuff(SeamstressBuffs.SeamstressInsatiableBuff) || characterBody.HasBuff(SeamstressBuffs.SeamstressBleedBuff))
            {
                if (!stitchDot) stitchDot = Instantiate(SeamstressAssets.bleedEffect, bleedPosition);
            }
            else if (!characterBody.HasBuff(SeamstressBuffs.SeamstressInsatiableBuff) || !characterBody.HasBuff(SeamstressBuffs.SeamstressBleedBuff))
            {
                DestroyVisual();
            }
        }

        public void DestroyVisual()
        {
            Destroy(stitchDot);
            stitchDot = null;
            Destroy(this);
        }
    }
}
