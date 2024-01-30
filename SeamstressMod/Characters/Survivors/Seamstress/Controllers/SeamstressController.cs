using RoR2;
using System.Collections.Generic;
using SeamstressMod.Survivors.Seamstress;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.AddressableAssets;

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

        private static IEnumerable<CharacterBody.TimedBuff> timedBuffs; 

        public bool hasPlayed;

        public bool fuckYou;

        public bool butchered;

        public int needleCount;

        public TemporaryOverlay component;
        public void Awake()
        {
            healthComponent = GetComponent<HealthComponent>();
            characterBody = GetComponent<CharacterBody>();
            skillLocator = GetComponent<SkillLocator>();
            butcheredConversion = 0f;
            Hook();
        }
        public void Start()
        {
            needleRegen = SeamstressStaticValues.needleGainInterval;
            hasPlayed = false;
            fuckYou = false;
            butchered = false;
        }
        public void FixedUpdate()
        {
            passiveNeedleRegen();
            IsButchered();
            ButcheredSound();
            CalculateBonusDamage();
            NeedleDisplayCount();
            if (NeedleHUD.expungeHealing.GetComponent<Text>())
            {
                NeedleHUD.expungeHealing.GetComponent<Text>().text = Mathf.Round(butcheredConversion).ToString();
            }
        }
        private static void Hook()
        {
            On.RoR2.HealthComponent.Heal += new On.RoR2.HealthComponent.hook_Heal(HealthComponent_Heal);
            On.RoR2.CharacterModel.UpdateOverlays += new On.RoR2.CharacterModel.hook_UpdateOverlays(CharacterModel_UpdateOverlays);
        }
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
        private void passiveNeedleRegen()
        {
            if (characterBody.GetBuffCount(SeamstressBuffs.needles) < SeamstressStaticValues.maxNeedleAmount + skillLocator.special.maxStock - 1)
            {
                if (!characterBody.HasBuff(SeamstressBuffs.needleCountDownBuff))
                {
                    characterBody.AddTimedBuff(SeamstressBuffs.needleCountDownBuff, SeamstressStaticValues.needleGainInterval);
                    characterBody.AddBuff(SeamstressBuffs.needles);
                    Util.PlaySound("Play_treeBot_m1_hit_heal", characterBody.gameObject);
                }
            }
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
        private void IsButchered()
        {
            if (characterBody.HasBuff(SeamstressBuffs.butchered) && !butchered)
            {
                NeedleHUD.expungeHealing.GetComponent<Text>().enabled = true;
                NeedleHUD.expungeHealing.GetComponent<Outline>().enabled = true;
                Transform modelTransform = this.characterBody.modelLocator.modelTransform;
                if(modelTransform)
                {
                    TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = 1f;
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/matOnFire.mat").WaitForCompletion();
                    temporaryOverlay.inspectorCharacterModel = modelTransform.GetComponent<CharacterModel>();
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay.animateShaderAlpha = true;
                }
                butchered = true;
                skillLocator.primary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texStingerIcon");
                skillLocator.secondary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texPistolIcon");
                skillLocator.special.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texScepterSpecialIcon");
                //skillLocator.secondary.cooldownScale = 1 - (SeamstressStaticValues.cutCooldownReduction / 4f);
            }
            if(!characterBody.HasBuff(SeamstressBuffs.butchered) && butchered)
            {
                butchered = false;
                UnityEngine.Object.Instantiate<GameObject>(SeamstressAssets.reapEndEffect, characterBody.modelLocator.transform);
                Util.PlaySound("Play_voidman_transform_return", characterBody.gameObject);
                fuckYou = false;
                skillLocator.primary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texPrimaryIcon");
                skillLocator.secondary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texSecondaryIcon");
                skillLocator.special.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texSpecialIcon");
                //skillLocator.secondary.cooldownScale *= 1 - (SeamstressStaticValues.cutCooldownReduction / 4f);
            }
            if (!characterBody.HasBuff(SeamstressBuffs.butchered) && skillLocator.utility == skillLocator.FindSkill("reapRecast"))
            {
                skillLocator.utility.ExecuteIfReady();
            }
        }
        public void CalculateBonusDamage()
        {
            float healthMissing = (healthComponent.fullHealth + healthComponent.fullShield) - (healthComponent.health + healthComponent.shield);
            characterBody.baseDamage = 10f + (healthMissing * SeamstressStaticValues.passiveScaling);
        }
        #region needlehud
        public void NeedleDisplayCount()
        {
            baseNeedleAmount = skillLocator.special.maxStock - 1;
            needleCount = characterBody.GetBuffCount(SeamstressBuffs.needles) - baseNeedleAmount;
            switch (needleCount)
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
        #endregion
        private static void Unhook()
        {
            On.RoR2.HealthComponent.Heal -= new On.RoR2.HealthComponent.hook_Heal(HealthComponent_Heal);
            On.RoR2.CharacterModel.UpdateOverlays -= new On.RoR2.CharacterModel.hook_UpdateOverlays(CharacterModel_UpdateOverlays);
        }
        public void OnDestroy()
        {
            Unhook();
        }

    }
}
