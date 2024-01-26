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

        public int needleCount;
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
            NeedleDisplayCount();
        }
        private static void Hook()
        {
            On.RoR2.HealthComponent.Heal += new On.RoR2.HealthComponent.hook_Heal(HealthComponent_Heal);
        }
        public void NeedleDisplayCount()
        {
            baseNeedleAmount = skillLocator.special.maxStock - 1;
            needleCount = characterBody.GetBuffCount(SeamstressBuffs.needles) - baseNeedleAmount;
            switch(needleCount)
            {
                case 0:
                    NeedleHUD.needleZero.SetActive(false);
                    NeedleHUD.needleOne.SetActive(false);
                    NeedleHUD.needleTwo.SetActive(false);
                    NeedleHUD.needleThree.SetActive(false);
                    NeedleHUD.needleFour.SetActive(false);
                    NeedleHUD.needleFive.SetActive(false);
                    NeedleHUD.needleSix.SetActive(false);
                    NeedleHUD.needleSeven.SetActive(false);
                    NeedleHUD.needleEight.SetActive(false);
                    NeedleHUD.needleNine.SetActive(false);
                    break;
                case 1:
                    NeedleHUD.needleZero.SetActive(true);
                    NeedleHUD.needleOne.SetActive(false);
                    NeedleHUD.needleTwo.SetActive(false);
                    NeedleHUD.needleThree.SetActive(false);
                    NeedleHUD.needleFour.SetActive(false);
                    NeedleHUD.needleFive.SetActive(false);
                    NeedleHUD.needleSix.SetActive(false);
                    NeedleHUD.needleSeven.SetActive(false);
                    NeedleHUD.needleEight.SetActive(false);
                    NeedleHUD.needleNine.SetActive(false);
                    break;
                case 2:
                    NeedleHUD.needleZero.SetActive(true);
                    NeedleHUD.needleOne.SetActive(true);
                    NeedleHUD.needleTwo.SetActive(false);
                    NeedleHUD.needleThree.SetActive(false);
                    NeedleHUD.needleFour.SetActive(false);
                    NeedleHUD.needleFive.SetActive(false);
                    NeedleHUD.needleSix.SetActive(false);
                    NeedleHUD.needleSeven.SetActive(false);
                    NeedleHUD.needleEight.SetActive(false);
                    NeedleHUD.needleNine.SetActive(false);
                    break;
                case 3:
                    NeedleHUD.needleZero.SetActive(true);
                    NeedleHUD.needleOne.SetActive(true);
                    NeedleHUD.needleTwo.SetActive(true);
                    NeedleHUD.needleThree.SetActive(false);
                    NeedleHUD.needleFour.SetActive(false);
                    NeedleHUD.needleFive.SetActive(false);
                    NeedleHUD.needleSix.SetActive(false);
                    NeedleHUD.needleSeven.SetActive(false);
                    NeedleHUD.needleEight.SetActive(false);
                    NeedleHUD.needleNine.SetActive(false);
                    break;
                case 4:
                    NeedleHUD.needleZero.SetActive(true);
                    NeedleHUD.needleOne.SetActive(true);
                    NeedleHUD.needleTwo.SetActive(true);
                    NeedleHUD.needleThree.SetActive(true);
                    NeedleHUD.needleFour.SetActive(false);
                    NeedleHUD.needleFive.SetActive(false);
                    NeedleHUD.needleSix.SetActive(false);
                    NeedleHUD.needleSeven.SetActive(false);
                    NeedleHUD.needleEight.SetActive(false);
                    NeedleHUD.needleNine.SetActive(false);
                    break;
                case 5:
                    NeedleHUD.needleZero.SetActive(true);
                    NeedleHUD.needleOne.SetActive(true);
                    NeedleHUD.needleTwo.SetActive(true);
                    NeedleHUD.needleThree.SetActive(true);
                    NeedleHUD.needleFour.SetActive(true);
                    NeedleHUD.needleFive.SetActive(false);
                    NeedleHUD.needleSix.SetActive(false);
                    NeedleHUD.needleSeven.SetActive(false);
                    NeedleHUD.needleEight.SetActive(false);
                    NeedleHUD.needleNine.SetActive(false);
                    break;
                case 6:
                    NeedleHUD.needleZero.SetActive(true);
                    NeedleHUD.needleOne.SetActive(true);
                    NeedleHUD.needleTwo.SetActive(true);
                    NeedleHUD.needleThree.SetActive(true);
                    NeedleHUD.needleFour.SetActive(true);
                    NeedleHUD.needleFive.SetActive(true);
                    NeedleHUD.needleSix.SetActive(false);
                    NeedleHUD.needleSeven.SetActive(false);
                    NeedleHUD.needleEight.SetActive(false);
                    NeedleHUD.needleNine.SetActive(false);
                    break;
                case 7:
                    NeedleHUD.needleZero.SetActive(true);
                    NeedleHUD.needleOne.SetActive(true);
                    NeedleHUD.needleTwo.SetActive(true);
                    NeedleHUD.needleThree.SetActive(true);
                    NeedleHUD.needleFour.SetActive(true);
                    NeedleHUD.needleFive.SetActive(true);
                    NeedleHUD.needleSix.SetActive(true);
                    NeedleHUD.needleSeven.SetActive(false);
                    NeedleHUD.needleEight.SetActive(false);
                    NeedleHUD.needleNine.SetActive(false);
                    break;
                case 8:
                    NeedleHUD.needleZero.SetActive(true);
                    NeedleHUD.needleOne.SetActive(true);
                    NeedleHUD.needleTwo.SetActive(true);
                    NeedleHUD.needleThree.SetActive(true);
                    NeedleHUD.needleFour.SetActive(true);
                    NeedleHUD.needleFive.SetActive(true);
                    NeedleHUD.needleSix.SetActive(true);
                    NeedleHUD.needleSeven.SetActive(true);
                    NeedleHUD.needleEight.SetActive(false);
                    NeedleHUD.needleNine.SetActive(false);
                    break;
                case 9:
                    NeedleHUD.needleZero.SetActive(true);
                    NeedleHUD.needleOne.SetActive(true);
                    NeedleHUD.needleTwo.SetActive(true);
                    NeedleHUD.needleThree.SetActive(true);
                    NeedleHUD.needleFour.SetActive(true);
                    NeedleHUD.needleFive.SetActive(true);
                    NeedleHUD.needleSix.SetActive(true);
                    NeedleHUD.needleSeven.SetActive(true);
                    NeedleHUD.needleEight.SetActive(true);
                    NeedleHUD.needleNine.SetActive(false);
                    break;
                case 10:
                    NeedleHUD.needleZero.SetActive(true);
                    NeedleHUD.needleOne.SetActive(true);
                    NeedleHUD.needleTwo.SetActive(true);
                    NeedleHUD.needleThree.SetActive(true);
                    NeedleHUD.needleFour.SetActive(true);
                    NeedleHUD.needleFive.SetActive(true);
                    NeedleHUD.needleSix.SetActive(true);
                    NeedleHUD.needleSeven.SetActive(true);
                    NeedleHUD.needleEight.SetActive(true);
                    NeedleHUD.needleNine.SetActive(true);
                    break;

            }
                
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
            if (characterBody.HasBuff(SeamstressBuffs.butchered) && !butchered)
            {
                butchered = true;
                skillLocator.primary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texStingerIcon");
                skillLocator.secondary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texPistolIcon");
                skillLocator.special.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texStingerIcon");
                skillLocator.secondary.cooldownScale = 1 - (SeamstressStaticValues.cutCooldownReduction / 4f);
            }
            else if(characterBody.HasBuff(SeamstressBuffs.butchered) && !butchered)
            {
                if(skillLocator.utility == skillLocator.FindSkill("reapRecast"))
                { 
                    skillLocator.utility.ExecuteIfReady();
                }
                butcheredConversion = characterBody.damage * 0.2f;
                butchered = false;
                skillLocator.primary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texPrimaryIcon");
                skillLocator.secondary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texSecondaryIcon");
                skillLocator.special.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texSpecialIcon");
                skillLocator.secondary.cooldownScale *= 1 - (SeamstressStaticValues.cutCooldownReduction / 4f);
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
