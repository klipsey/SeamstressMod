using RoR2;
using R2API;
using System;
using System.Collections.Generic;
using System.Text;
using static R2API.DotAPI;
using static RoR2.DotController;
using UnityEngine;

namespace SeamstressMod.Survivors.Seamstress
{
    internal static class Dots
    {
        public static DotController.DotIndex SeamstressDot;

        public static DotController.DotIndex SeamstressBossDot;

        public static bool visualTracker = false;

        public static GameObject stitchDot;

        public static CustomDotBehaviour behave1;

        public static CustomDotBehaviour behave2;

        public static CustomDotVisual visual;
        internal static void Init()
        {
            behave1 = DelegateBehave;
            behave2 = DelegateBehave2;
            visual = stitchVisual;
            RegisterDots();
        }
        public static void DelegateBehave(RoR2.DotController self,  RoR2.DotController.DotStack dotStack)
        {
            if (dotStack.dotIndex == SeamstressDot) 
            {
                dotStack.damage = Math.Max(1f, (self.victimBody.healthComponent.fullCombinedHealth - self.victimBody.healthComponent.health) * SeamstressStaticValues.cutDotDamage);
            }
        }
        public static void DelegateBehave2(RoR2.DotController self, RoR2.DotController.DotStack dotStack) 
        {
            if (dotStack.dotIndex == SeamstressBossDot)
            {
                dotStack.damage = Math.Max(1f, (self.victimBody.healthComponent.fullCombinedHealth - self.victimBody.healthComponent.health) * SeamstressStaticValues.cutDotBossDamage);
            }
        }

        public static void stitchVisual(RoR2.DotController self)
        {
            if (!self.victimBody)
            {
                return;
            }
            self.transform.position = self.victimBody.corePosition;
            if (self.victimBody.HasBuff(SeamstressBuffs.stitched) && !visualTracker)
            {
                visualTracker = true;
                stitchDot = UnityEngine.Object.Instantiate(SeamstressAssets.stitchEffect, self.transform);
            }
            else if(!self.victimBody.HasBuff(SeamstressBuffs.stitched) && visualTracker)
            {
                UnityEngine.Object.Destroy(stitchDot);
                stitchDot = null;
            }
        }
        public static void RegisterDots()
        {
            SeamstressDot = DotAPI.RegisterDotDef(new DotController.DotDef
            {
                interval = 1f,
                damageCoefficient = 0f,
                damageColorIndex = DamageColorIndex.SuperBleed,
                associatedBuff = SeamstressBuffs.stitched,
                resetTimerOnAdd = false,
            }, (CustomDotBehaviour)behave1, (CustomDotVisual)visual);

            SeamstressBossDot = DotAPI.RegisterDotDef(new DotController.DotDef
            {
                interval = 1f,
                damageCoefficient = 0f,
                damageColorIndex = DamageColorIndex.SuperBleed,
                associatedBuff = SeamstressBuffs.stitched,
                resetTimerOnAdd = false,
            }, (CustomDotBehaviour)behave2, (CustomDotVisual)visual);
        }
    }
}
