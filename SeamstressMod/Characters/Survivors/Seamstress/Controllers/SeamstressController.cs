using RoR2;
using System.Collections.Generic;
using SeamstressMod.Survivors.Seamstress;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.AddressableAssets;
using RoR2.EntityLogic;
using UnityEngine.Networking;

namespace SeamstressMod.Survivors.Seamstress
{
    public class SeamstressController : MonoBehaviour
    {
        public CharacterBody characterBody;

        public float butcheredConversion;

        public float needleRegen;

        public int baseNeedleAmount;
        private bool hasEffectiveAuthority => characterBody.hasEffectiveAuthority;

        public HealthComponent healthComponent;

        public SkillLocator skillLocator;

        public float butcheredDuration;

        public bool hasPlayed;

        public bool fuckYou;

        public bool butchered;

        public float butcheredDurationPercent;

        public int needleCount;

        public TemporaryOverlay component;
        public void Awake()
        {
            healthComponent = GetComponent<HealthComponent>();
            characterBody = GetComponent<CharacterBody>();
            skillLocator = GetComponent<SkillLocator>();
            butcheredConversion = 0f;
            butcheredDuration = 0f;
            Hook();
        }
        public void Start()
        {
            needleRegen = SeamstressStaticValues.needleGainInterval;
            hasPlayed = false;
            fuckYou = false;
            butchered = false;
            butcheredDurationPercent = Seamstress.SeamstressStaticValues.butcheredDuration / 10f;
        }
        public void FixedUpdate()
        {
            if(butcheredDuration > 0f) 
            {
                butcheredDuration -= Time.deltaTime;
            }
            passiveNeedleRegen();
            ButcheredSound();
        }
        private static void Hook()
        {
            On.RoR2.HealthComponent.Heal += new On.RoR2.HealthComponent.hook_Heal(HealthComponent_Heal);
            On.RoR2.CharacterModel.UpdateOverlays += new On.RoR2.CharacterModel.hook_UpdateOverlays(CharacterModel_UpdateOverlays);
            On.RoR2.CharacterBody.AddTimedBuff_BuffDef_float_int += CharacterBody_AddTimedBuff_BuffDef_float_int;
        }
        private static void CharacterBody_AddTimedBuff_BuffDef_float_int(On.RoR2.CharacterBody.orig_AddTimedBuff_BuffDef_float_int orig, CharacterBody self, BuffDef buffDef, float duration, int maxStacks)
        {
            orig.Invoke(self, buffDef, duration, maxStacks);
            if(NetworkServer.active)
            {
                foreach (var timedBuff in self.timedBuffs.Where(b => b.buffIndex == SeamstressBuffs.butchered.buffIndex && b.timer == duration))
                {
                    SeamstressController s = self.GetComponent<SeamstressController>();
                    if (self.TryGetComponent<SeamstressController>(out s))
                    {
                        s.GetButcheredDuration(timedBuff.timer);
                    }
                }
            }
        }
        //butchered overlay
        private static void CharacterModel_UpdateOverlays(On.RoR2.CharacterModel.orig_UpdateOverlays orig, CharacterModel self)
        {
            orig.Invoke(self);
            if (!self || !self.body)
            {
                return;
            }
            if (self.body.HasBuff(SeamstressBuffs.butchered) && self.body.GetComponent<SeamstressController>().fuckYou == false)
            {
                TemporaryOverlay temporaryOverlay = self.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = SeamstressStaticValues.butcheredDuration;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0.4f);
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = SeamstressAssets.butcheredOverlayMat;
                temporaryOverlay.AddToCharacerModel(self);
                self.body.GetComponent<SeamstressController>().fuckYou = true;
            }
        }
        //calculate expunge healing
        private static float HealthComponent_Heal(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen = true)
        {
            if (self.body.HasBuff(SeamstressBuffs.butchered) && self.body.baseNameToken == "KENKO_SEAMSTRESS_NAME")
            {
                amount *= SeamstressStaticValues.healConversion;
            }
            var res = orig(self, amount, procChainMask, nonRegen);
            SeamstressController s = self.body.GetComponent<SeamstressController>();
            if (self.body.TryGetComponent<SeamstressController>(out s) && self.body.HasBuff(SeamstressBuffs.butchered))
            {
                s.GetButcheredConversion((res/SeamstressStaticValues.healConversion)* 1 - SeamstressStaticValues.healConversion);
            }
            return res;
        }
        //needle regen
        private void passiveNeedleRegen()
        {
            if (characterBody.GetBuffCount(SeamstressBuffs.needles) < SeamstressStaticValues.maxNeedleAmount + skillLocator.special.maxStock - 1)
            {
                if (!characterBody.HasBuff(SeamstressBuffs.needleCountDownBuff))
                {
                    if(NetworkServer.active)
                    {
                        characterBody.AddTimedBuff(SeamstressBuffs.needleCountDownBuff, SeamstressStaticValues.needleGainInterval);
                        characterBody.AddBuff(SeamstressBuffs.needles);
                    }
                    Util.PlaySound("Play_treeBot_m1_hit_heal", characterBody.gameObject);
                }
            }
        }
        public void GetButcheredConversion(float healDamage)
        {
            butcheredConversion += healDamage;
        }
        public void GetButcheredDuration(float butcherTheseNuts)
        {
            butcheredDuration = butcherTheseNuts;
        }
        //butchered controller
        //butchered end sound
        public void ButcheredSound()
        {
            if (butcheredDuration > 0)
            {
                if (butcheredDuration < 2f && !hasPlayed)
                {
                    Util.PlaySound("Play_nullifier_impact", characterBody.gameObject);
                    hasPlayed = true;
                }
            }
            hasPlayed = false;
        }
        private static void Unhook()
        {
            On.RoR2.HealthComponent.Heal -= new On.RoR2.HealthComponent.hook_Heal(HealthComponent_Heal);
            On.RoR2.CharacterModel.UpdateOverlays -= new On.RoR2.CharacterModel.hook_UpdateOverlays(CharacterModel_UpdateOverlays);
            On.RoR2.CharacterBody.AddTimedBuff_BuffDef_float_int -= CharacterBody_AddTimedBuff_BuffDef_float_int;
        }
        public void OnDestroy()
        {
            Unhook();
        }

    }
}
