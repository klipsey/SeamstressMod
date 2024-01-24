using RoR2;
using R2API;
using System;
using System.Collections.Generic;
using System.Text;
using static R2API.DotAPI;
using static RoR2.DotController;

namespace SeamstressMod.Survivors.Seamstress
{
    internal static class Dots
    {
        public static DotController.DotIndex SeamstressDot;

        public static DotController.DotIndex SeamstressDotWeak;

        public static CustomDotBehaviour behave1;

        public static CustomDotBehaviour behave2;
        internal static void Init()
        {
            behave1 = DelegateBehave;
            RegisterDots();
        }
        public static void DelegateBehave(RoR2.DotController self,  RoR2.DotController.DotStack dotStack)
        {
            if (dotStack.dotIndex == SeamstressDot) 
            {
                dotStack.damage = Math.Max(1f, (self.victimBody.healthComponent.fullCombinedHealth - self.victimBody.healthComponent.health) * SeamstressStaticValues.stitchedDamage);
            }
        }
        public static void DelegateBehave2(RoR2.DotController self, RoR2.DotController.DotStack dotStack)
        {
            if (dotStack.dotIndex == SeamstressDot)
            {
                dotStack.damage = Math.Max(1f, (self.victimBody.healthComponent.fullCombinedHealth - self.victimBody.healthComponent.health) * SeamstressStaticValues.stitchedBossDamage);
            }
        }
        public static void RegisterDots()
        {
            SeamstressDot = DotAPI.RegisterDotDef(new DotController.DotDef
            {
                interval = 0.5f,
                damageCoefficient = 0f,
                damageColorIndex = DamageColorIndex.SuperBleed,
                associatedBuff = SeamstressBuffs.stitched,
                resetTimerOnAdd = false,
            }, (CustomDotBehaviour)behave1, (CustomDotVisual)null);

            SeamstressDotWeak = DotAPI.RegisterDotDef(new DotController.DotDef
            {
                interval = 0.5f,
                damageCoefficient = 0f,
                damageColorIndex = DamageColorIndex.SuperBleed,
                associatedBuff = SeamstressBuffs.stitched,
                resetTimerOnAdd = false,
            }, (CustomDotBehaviour)behave1, (CustomDotVisual)null);
        }
    }
}
