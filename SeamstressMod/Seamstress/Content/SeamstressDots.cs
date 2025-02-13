﻿using RoR2;
using R2API;
using System;
using System.Collections.Generic;
using System.Text;
using static R2API.DotAPI;
using static RoR2.DotController;
using UnityEngine;
using UnityEngine.Networking;

namespace SeamstressMod.Seamstress.Content
{
    internal static class SeamstressDots
    {
        public static DotIndex SeamstressSelfBleed;

        public static CustomDotBehaviour selfDamageBehaviour;

        public static CustomDotVisual visual;
        internal static void Init()
        {
            selfDamageBehaviour = SelfBleedDelegate;
            visual = StitchVisual;
            RegisterDots();
        }
        public static void SelfBleedDelegate(DotController self, DotStack dotStack)
        {
            if (dotStack.dotIndex == SeamstressSelfBleed)
            {
                dotStack.damage = (self.victimBody.healthComponent.fullCombinedHealth - (self.victimBody.healthComponent.fullCombinedHealth -
                    (self.victimBody.healthComponent.health + self.victimBody.healthComponent.shield))) * 0.05f;
                dotStack.damageType = DamageType.NonLethal | DamageType.BypassArmor | DamageType.BypassBlock | DamageType.DoT;
            }
        }
        public class BleedController : MonoBehaviour
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
                    Destroy(stitchDot);
                    stitchDot = null;
                    Destroy(this);
                }
            }
        }
        public static void StitchVisual(DotController self)
        {
            if (!self.victimBody)
            {
                return;
            }
            BleedController component = self.victimBody.gameObject.GetComponent<BleedController>();
            if (component == null) self.victimBody.gameObject.AddComponent<BleedController>();
        }
        public static void RegisterDots()
        {
            SeamstressSelfBleed = RegisterDotDef(new DotDef
            {
                interval = 0.2f,
                damageCoefficient = 0f,
                damageColorIndex = DamageColorIndex.Bleed,
                associatedBuff = SeamstressBuffs.SeamstressInsatiableBuff,
                resetTimerOnAdd = true,
            }, selfDamageBehaviour, visual);
        }
    }
}
