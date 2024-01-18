using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using SeamstressMod.Modules.Characters;
using System.Linq;

namespace SeamstressMod.Survivors.Seamstress
{
    public class SeamstressController : MonoBehaviour
    {
        public CharacterBody characterBody;

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
        }
        public void FixedUpdate()
        {
            IsButchered();
            ButcheredSound();
            CalculateBonusDamage();
            baseNeedleAmount = skillLocator.special.maxStock - 1;
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
                butchered = true;
                characterBody.skillLocator.primary.skillDef.icon = SeamstressSurvivor.assetBundle.LoadAsset<Sprite>("texStingerIcon");
                characterBody.skillLocator.secondary.skillDef.icon = SeamstressSurvivor.assetBundle.LoadAsset<Sprite>("texPistolIcon");
                characterBody.skillLocator.utility.skillDef.icon = SeamstressSurvivor.assetBundle.LoadAsset<Sprite>("texBoxingGlovesIcon");
                characterBody.skillLocator.special.skillDef.icon = SeamstressSurvivor.assetBundle.LoadAsset<Sprite>("texStingerIcon");
            }
            else
            {
                butchered = false;
                characterBody.skillLocator.primary.skillDef.icon = SeamstressSurvivor.assetBundle.LoadAsset<Sprite>("texPrimaryIcon");
                characterBody.skillLocator.secondary.skillDef.icon = SeamstressSurvivor.assetBundle.LoadAsset<Sprite>("texSecondaryIcon");
                characterBody.skillLocator.utility.skillDef.icon = SeamstressSurvivor.assetBundle.LoadAsset<Sprite>("texUtilityIcon");
                characterBody.skillLocator.special.skillDef.icon = SeamstressSurvivor.assetBundle.LoadAsset<Sprite>("texSpecialIcon");
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
            characterBody.baseDamage = 10f + (healthMissing * 0.1f);
        }
    }
}
