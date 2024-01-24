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

        public static CustomDotBehaviour behave;
        internal static void Init()
        {
            behave = DelegateBehave;
            RegisterDots();
        }
        public static void DelegateBehave(RoR2.DotController self,  RoR2.DotController.DotStack dotStack)
        {
            if (dotStack.dotIndex == SeamstressDot) 
            {
                dotStack.damage = (self.victimBody.healthComponent.health * SeamstressStaticValues.stitchedDamage);
            }
        }
        public static void RegisterDots()
        {
            SeamstressDot = DotAPI.RegisterDotDef(new DotController.DotDef
            {
                interval = 1.5f,
                damageCoefficient = 0f,
                damageColorIndex = DamageColorIndex.SuperBleed,
                associatedBuff = SeamstressBuffs.stitched,
                resetTimerOnAdd = false,
            }, (CustomDotBehaviour)behave, (CustomDotVisual)null);
        }
    }
}
