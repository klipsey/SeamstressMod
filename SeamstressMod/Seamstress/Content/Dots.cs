using RoR2;
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
    internal static class Dots
    {
        public static DotIndex SeamstressDot;

        public static DotIndex SeamstressBossDot;

        public static DotIndex SeamstressBleed;

        public static CustomDotBehaviour behave1;

        public static CustomDotBehaviour behave2;

        public static CustomDotBehaviour butcheredBehaviour;

        public static CustomDotVisual visual;
        internal static void Init()
        {
            behave1 = DelegateBehave;
            behave2 = DelegateBehave2;
            butcheredBehaviour = DelegateBehave3;
            visual = StitchVisual;
            RegisterDots();
        }
        public static void DelegateBehave(DotController self, DotStack dotStack)
        {
            if (dotStack.dotIndex == SeamstressDot)
            {
                dotStack.damage = Math.Max(1f, (self.victimBody.healthComponent.fullCombinedHealth - self.victimBody.healthComponent.health) * (SeamstressStaticValues.cutDotDamage * 0.2f));
                dotStack.damageType = DamageType.DoT;
            }
        }
        public static void DelegateBehave2(DotController self, DotStack dotStack)
        {
            if (dotStack.dotIndex == SeamstressBossDot)
            {
                dotStack.damage = Math.Max(1f, (self.victimBody.healthComponent.fullCombinedHealth - self.victimBody.healthComponent.health) * (SeamstressStaticValues.cutDotBossDamage * 0.2f));
                dotStack.damageType = DamageType.DoT;
            }
        }
        public static void DelegateBehave3(DotController self, DotStack dotStack)
        {
            if (dotStack.dotIndex == SeamstressBleed && self.victimBody.healthComponent.combinedHealth > self.victimBody.healthComponent.fullCombinedHealth * 0.05)
            {
                float currentBarrier = self.victimBody.healthComponent.barrier;
                float currentShield = self.victimBody.healthComponent.shield;
                dotStack.damage = Math.Max(1f, self.victimBody.healthComponent.health * 0.04f + self.victimBody.healthComponent.shield + self.victimBody.healthComponent.barrier);
                dotStack.damageType = DamageType.NonLethal | DamageType.BypassArmor | DamageType.BypassBlock | DamageType.DoT;
                self.victimBody.healthComponent.AddBarrier(currentBarrier);
                self.victimBody.healthComponent.shield = currentShield;
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
                if (characterBody.HasBuff(SeamstressBuffs.instatiable) || characterBody.HasBuff(SeamstressBuffs.cutBleed))
                {
                    if (!stitchDot) stitchDot = Instantiate(SeamstressAssets.stitchEffect, bleedPosition);
                }
                else if (!characterBody.HasBuff(SeamstressBuffs.instatiable) || !characterBody.HasBuff(SeamstressBuffs.cutBleed))
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
            SeamstressDot = RegisterDotDef(new DotDef
            {
                interval = 0.2f,
                damageCoefficient = 0f,
                damageColorIndex = DamageColorIndex.SuperBleed,
                associatedBuff = SeamstressBuffs.cutBleed,
                resetTimerOnAdd = false,
            }, behave1, visual);

            SeamstressBossDot = RegisterDotDef(new DotDef
            {
                interval = 0.2f,
                damageCoefficient = 0f,
                damageColorIndex = DamageColorIndex.SuperBleed,
                associatedBuff = SeamstressBuffs.cutBleed,
                resetTimerOnAdd = false,
            }, behave2, visual);

            SeamstressBleed = RegisterDotDef(new DotDef
            {
                interval = 0.2f,
                damageCoefficient = 0f,
                damageColorIndex = DamageColorIndex.SuperBleed,
                associatedBuff = SeamstressBuffs.instatiable,
                resetTimerOnAdd = true,
            }, butcheredBehaviour, visual);
        }
    }
}
