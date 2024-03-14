using RoR2;
using R2API;
using System;
using System.Collections.Generic;
using System.Text;
using static R2API.DotAPI;
using static RoR2.DotController;
using UnityEngine;
using UnityEngine.Networking;

namespace SeamstressMod.Survivors.Seamstress
{
    internal static class Dots
    {
        public static DotController.DotIndex SeamstressDot;

        public static DotController.DotIndex SeamstressBossDot;

        public static DotController.DotIndex ButcheredDot;

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
        public static void DelegateBehave(RoR2.DotController self,  RoR2.DotController.DotStack dotStack)
        {
            if (dotStack.dotIndex == SeamstressDot) 
            {
                dotStack.damage = Math.Max(1f, (self.victimBody.healthComponent.fullCombinedHealth - self.victimBody.healthComponent.health) * (SeamstressStaticValues.cutDotDamage * 0.2f));
                dotStack.damageType = DamageType.DoT;
            }
        }
        public static void DelegateBehave2(RoR2.DotController self, RoR2.DotController.DotStack dotStack) 
        {
            if (dotStack.dotIndex == SeamstressBossDot)
            {
                dotStack.damage = Math.Max(1f, (self.victimBody.healthComponent.fullCombinedHealth - self.victimBody.healthComponent.health) * (SeamstressStaticValues.cutDotBossDamage * 0.2f));
                dotStack.damageType = DamageType.DoT;
            }
        }
        public static void DelegateBehave3(RoR2.DotController self, RoR2.DotController.DotStack dotStack)
        {
            if (dotStack.dotIndex == ButcheredDot)
            {
                float currentBarrier = self.victimBody.healthComponent.barrier;
                float currentShield = self.victimBody.healthComponent.shield;
                dotStack.damage = Math.Max(1f, (self.victimBody.healthComponent.health * 0.04f + self.victimBody.healthComponent.shield + self.victimBody.healthComponent.barrier));
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
                if(characterBody.HasBuff(SeamstressBuffs.butchered) || characterBody.HasBuff(SeamstressBuffs.cutBleed))
                {
                    if (!stitchDot) stitchDot = UnityEngine.Object.Instantiate(SeamstressAssets.stitchEffect, bleedPosition);
                }
                else if (!characterBody.HasBuff(SeamstressBuffs.butchered) ||! characterBody.HasBuff(SeamstressBuffs.cutBleed))
                {
                    UnityEngine.Object.Destroy(stitchDot);
                    stitchDot = null;
                    UnityEngine.Object.Destroy(this);
                }
            }
        }
        public static void StitchVisual(RoR2.DotController self)
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
            SeamstressDot = DotAPI.RegisterDotDef(new DotController.DotDef
            {
                interval = 0.2f,
                damageCoefficient = 0f,
                damageColorIndex = DamageColorIndex.SuperBleed,
                associatedBuff = SeamstressBuffs.cutBleed,
                resetTimerOnAdd = false,
            }, (CustomDotBehaviour)behave1, (CustomDotVisual)visual);

            SeamstressBossDot = DotAPI.RegisterDotDef(new DotController.DotDef
            {
                interval = 0.2f,
                damageCoefficient = 0f,
                damageColorIndex = DamageColorIndex.SuperBleed,
                associatedBuff = SeamstressBuffs.cutBleed,
                resetTimerOnAdd = false,
            }, (CustomDotBehaviour)behave2, (CustomDotVisual)visual);

            ButcheredDot = DotAPI.RegisterDotDef(new DotController.DotDef
            {
                interval = 0.2f,
                damageCoefficient = 0f,
                damageColorIndex = DamageColorIndex.SuperBleed,
                associatedBuff = SeamstressBuffs.butchered,
                resetTimerOnAdd = true,
            }, (CustomDotBehaviour)butcheredBehaviour, (CustomDotVisual)visual);
        }
    }
}
