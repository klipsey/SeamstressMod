using RoR2;
using RoR2.UI;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using SeamstressMod.Modules.Characters;
using System.Linq;
using SeamstressMod.SkillStates;
using SeamstressMod.Modules.BaseStates;

namespace SeamstressMod.Survivors.Seamstress
{
    public class SeamstressController : MonoBehaviour
    {
        public CharacterBody characterBody;

        public float butcheredConversion = 0;

        public int baseNeedleAmount;
        private bool hasEffectiveAuthority => characterBody.hasEffectiveAuthority;

        public HealthComponent healthComponent;

        public SkillLocator skillLocator;

        private static IEnumerable<CharacterBody.TimedBuff> timedBuffs; 

        public bool hasPlayed = false;

        public bool butchered;

        public void Start()
        {
            healthComponent = GetComponent<HealthComponent>();
            characterBody = GetComponent<CharacterBody>();
            skillLocator = GetComponent<SkillLocator>();
            Hook();
        }
        public void FixedUpdate()
        {
            IsButchered();
            ButcheredSound();
            CalculateBonusDamage();
            baseNeedleAmount = skillLocator.special.maxStock - 1;
        }
        private static void Hook()
        {
            On.RoR2.HealthComponent.Heal += new On.RoR2.HealthComponent.hook_Heal(HealthComponent_Heal);
        }
        private static float HealthComponent_Heal(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen = true)
        {
            if (self.body.HasBuff(SeamstressBuffs.butchered))
            {
                amount *= SeamstressStaticValues.healConversion;
            }
            var res = orig(self, amount, procChainMask, nonRegen);
            SeamstressController s = self.body.GetComponent<SeamstressController>();
            if (self.body.TryGetComponent<SeamstressController>(out s))
            {
                s.GetButcheredConversion((res/SeamstressStaticValues.healConversion)* 1 - SeamstressStaticValues.healConversion);
            }
            return res;
        }
        public void GetButcheredConversion(float healDamage)
        {
            butcheredConversion += healDamage;
        }
        public void ButcheredSound()
        {
            timedBuffs = characterBody.timedBuffs.Where((CharacterBody.TimedBuff b) => b.buffIndex == SeamstressBuffs.butchered.buffIndex);
            if (timedBuffs.Any())
            { 
                if (timedBuffs.FirstOrDefault().timer < 2f && !hasPlayed)
                {
                    Util.PlaySound("Play_nullifier_impact", characterBody.gameObject);
                    hasPlayed = true;
                }
            }
            hasPlayed = false;
        }
        public void IsButchered()
        {
            if (characterBody.HasBuff(SeamstressBuffs.butchered))
            {
                if(!butchered)
                {
                    skillLocator.utility = skillLocator.FindSkill("reapRecast"); ;
                }
                butchered = true;
                skillLocator.primary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texStingerIcon");
                skillLocator.secondary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texPistolIcon");
                skillLocator.special.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texStingerIcon");
            }
            else
            {
                if (butchered)
                {
                    if(skillLocator.utility == skillLocator.FindSkill("reapRecast"))
                    {
                        skillLocator.utility.ExecuteIfReady();
                    }
                    skillLocator.utility.AddOneStock();
                    skillLocator.utility = skillLocator.FindSkill("Utility");
                }
                butcheredConversion = characterBody.damage * 0.2f;
                butchered = false;
                skillLocator.primary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texPrimaryIcon");
                skillLocator.secondary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texSecondaryIcon");
                skillLocator.special.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texSpecialIcon");
                TemporaryOverlay component = GetComponent<TemporaryOverlay>();
                if ((bool)component)
                {
                    DestroyImmediate(component);
                    UnityEngine.Object.Instantiate<GameObject>(SeamstressAssets.reapEndEffect, characterBody.modelLocator.transform);
                    Util.PlaySound("Play_voidman_transform_return", characterBody.gameObject);
                }
            }
        }
        public void CalculateBonusDamage()
        {
            float healthMissing = (healthComponent.fullHealth + healthComponent.fullShield) - (healthComponent.health + healthComponent.shield);
            characterBody.baseDamage = 12f + (healthMissing * SeamstressStaticValues.passiveScaling);
        }
        public void Unhook()
        {
            On.RoR2.HealthComponent.Heal -= new On.RoR2.HealthComponent.hook_Heal(HealthComponent_Heal);

        }
        public void OnDestroy()
        {
            Unhook();
        }

    }
}
