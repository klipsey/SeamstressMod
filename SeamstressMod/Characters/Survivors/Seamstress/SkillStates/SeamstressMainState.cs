using UnityEngine;
using RoR2;
using UnityEngine.UI;
using EntityStates;
using SeamstressMod.Survivors.Seamstress;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using System.Linq;
using System.Collections.Generic;

namespace SeamstressMod.SkillStates
{
    public class SeamstressMainState : GenericCharacterMain
    {
        private SeamstressController fard;

        private static IEnumerable<CharacterBody.TimedBuff> butcheredDuration;

        private bool shittingMyself;

        private float bd = 0f;
        public override void OnEnter()
        {
            base.OnEnter();
            this.fard = this.GetComponent<SeamstressController>();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active)
            {
                butcheredDuration = this.characterBody.timedBuffs.Where((CharacterBody.TimedBuff b) => b.buffIndex == SeamstressBuffs.butchered.buffIndex);
                if (this.characterBody.HasBuff(SeamstressBuffs.butchered))
                {
                    this.shittingMyself = true;
                }
                else
                {
                    this.shittingMyself = false;
                }
            }
            IsButchered();
            if (bd > 0f)
            {
                bd -= Time.fixedDeltaTime;
            }
            if (base.isAuthority) 
            {
                CalculateBonusDamage();
                NeedleDisplayCount();
            }
            ButcheredSound();
        }
        public void ButcheredSound()
        {
            if (shittingMyself)
            {
                if (bd < 2f && !fard.hasPlayed)
                {
                    Util.PlaySound("Play_nullifier_impact", characterBody.gameObject);
                    fard.hasPlayed = true;
                }
            }
            fard.hasPlayed = false;
        }
        public void IsButchered()
        {
            //run during butchered
            if (this.shittingMyself)
            {
                //run when butchered starts
                if (!this.fard.butchered)
                {
                    //set bd for timer max
                    if (SeamstressPlugin.networktimedbuffsInstalled)
                    {
                        bd = butcheredDuration.FirstOrDefault().timer;
                    }
                    else
                    {
                        bd = SeamstressStaticValues.butcheredDuration;
                    }
                    this.fard.butcheredDurationPercent = bd / 10f;
                    if (base.isAuthority)
                    {
                        NeedleHUD.expungeHealing.GetComponent<Text>().enabled = true;
                        NeedleHUD.expungeHealing.GetComponent<Outline>().enabled = true;
                        this.skillLocator.primary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texStingerIcon");
                        this.skillLocator.secondary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texPistolIcon");
                        this.skillLocator.special.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texScepterSpecialIcon");
                    }
                    if(NetworkServer.active)
                    {
                        Transform modelTransform = this.characterBody.modelLocator.modelTransform;
                        if (modelTransform)
                        {
                            #region overlay
                            TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                            temporaryOverlay.duration = 1f;
                            temporaryOverlay.destroyComponentOnEnd = true;
                            temporaryOverlay.originalMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/matOnFire.mat").WaitForCompletion();
                            temporaryOverlay.inspectorCharacterModel = modelTransform.GetComponent<CharacterModel>();
                            temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                            temporaryOverlay.animateShaderAlpha = true;
                            #endregion
                        }
                    }
                    this.fard.butchered = true;
                }
            }
            //run when butchered ends
            if (!shittingMyself && this.fard.butchered)
            {
                this.fard.butchered = false;
                UnityEngine.Object.Instantiate<GameObject>(SeamstressAssets.reapEndEffect, this.characterBody.modelLocator.transform);
                Util.PlaySound("Play_voidman_transform_return", this.characterBody.gameObject);
                this.fard.fuckYou = false;
                if (base.isAuthority)
                {
                    if (base.skillLocator.utility == base.skillLocator.FindSkill("reapRecast"))
                    {
                        base.skillLocator.utility.ExecuteIfReady();
                    }
                    this.skillLocator.primary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texPrimaryIcon");
                    this.skillLocator.secondary.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texSecondaryIcon");
                    this.skillLocator.special.skillDef.icon = SeamstressSurvivor.instance.assetBundle.LoadAsset<Sprite>("texSpecialIcon");

                }
            }
            if(base.isAuthority)
            {
                if (NeedleHUD.expungeHealing.GetComponent<Text>())
                {
                    NeedleHUD.expungeHealing.GetComponent<Text>().text = Mathf.Round(this.fard.butcheredConversion).ToString();
                }
                if (butcheredDuration.Any())
                {
                    HudColor();
                }
                else
                {
                    #region fuck you
                    Color newColor = Color.white;
                    newColor.a = 1;
                    NeedleHUD.needleImgZero.color = newColor;
                    NeedleHUD.needleImgOne.color = newColor;
                    NeedleHUD.needleImgTwo.color = newColor;
                    NeedleHUD.needleImgThree.color = newColor;
                    NeedleHUD.needleImgFour.color = newColor;
                    NeedleHUD.needleImgFive.color = newColor;
                    NeedleHUD.needleImgSix.color = newColor;
                    NeedleHUD.needleImgSeven.color = newColor;
                    NeedleHUD.needleImgEight.color = newColor;
                    NeedleHUD.needleImgNine.color = newColor;
                    #endregion
                }
            }
        }
        public void CalculateBonusDamage()
        {
            float healthMissing = (this.healthComponent.fullHealth + this.healthComponent.fullShield) - (this.healthComponent.health + this.healthComponent.shield);
            this.characterBody.baseDamage = 10f + (healthMissing * SeamstressStaticValues.passiveScaling);
        }
        public void HudColor()
        {
            #region hudColorTracking
            if (bd > 9f * this.fard.butcheredDurationPercent)
            {
                Color newColor = Color.white;
                NeedleHUD.needleImgZero.color = newColor;
                NeedleHUD.needleImgOne.color = newColor;
                NeedleHUD.needleImgTwo.color = newColor;
                NeedleHUD.needleImgThree.color = newColor;
                NeedleHUD.needleImgFour.color = newColor;
                NeedleHUD.needleImgFive.color = newColor;
                NeedleHUD.needleImgSix.color = newColor;
                NeedleHUD.needleImgSeven.color = newColor;
                NeedleHUD.needleImgEight.color = newColor;
                NeedleHUD.needleImgNine.color = newColor;
            }
            if (bd <= 9f * this.fard.butcheredDurationPercent && bd > 8f * this.fard.butcheredDurationPercent)
            {
                Color newColor = new Color(245f / 255f, 232 / 255f, 231 / 255f);
                newColor.a = 1;
                NeedleHUD.needleImgZero.color = newColor;
                NeedleHUD.needleImgOne.color = newColor;
                NeedleHUD.needleImgTwo.color = newColor;
                NeedleHUD.needleImgThree.color = newColor;
                NeedleHUD.needleImgFour.color = newColor;
                NeedleHUD.needleImgFive.color = newColor;
                NeedleHUD.needleImgSix.color = newColor;
                NeedleHUD.needleImgSeven.color = newColor;
                NeedleHUD.needleImgEight.color = newColor;
                NeedleHUD.needleImgNine.color = newColor;
            }
            if (bd <= 8f * this.fard.butcheredDurationPercent && bd > 7f * this.fard.butcheredDurationPercent)
            {
                Color newColor = new Color(237f / 255f, 209f / 255f, 207f / 255f);
                newColor.a = 1;
                NeedleHUD.needleImgZero.color = newColor;
                NeedleHUD.needleImgOne.color = newColor;
                NeedleHUD.needleImgTwo.color = newColor;
                NeedleHUD.needleImgThree.color = newColor;
                NeedleHUD.needleImgFour.color = newColor;
                NeedleHUD.needleImgFive.color = newColor;
                NeedleHUD.needleImgSix.color = newColor;
                NeedleHUD.needleImgSeven.color = newColor;
                NeedleHUD.needleImgEight.color = newColor;
                NeedleHUD.needleImgNine.color = newColor;
            }
            if (bd <= 7f * this.fard.butcheredDurationPercent && bd > 6f * this.fard.butcheredDurationPercent)
            {
                Color newColor = new Color(227f / 255f, 187f / 255f, 183f / 255f);
                newColor.a = 1;
                NeedleHUD.needleImgZero.color = newColor;
                NeedleHUD.needleImgOne.color = newColor;
                NeedleHUD.needleImgTwo.color = newColor;
                NeedleHUD.needleImgThree.color = newColor;
                NeedleHUD.needleImgFour.color = newColor;
                NeedleHUD.needleImgFive.color = newColor;
                NeedleHUD.needleImgSix.color = newColor;
                NeedleHUD.needleImgSeven.color = newColor;
                NeedleHUD.needleImgEight.color = newColor;
                NeedleHUD.needleImgNine.color = newColor;
            }
            if (bd <= 6f * this.fard.butcheredDurationPercent && bd > 5f * this.fard.butcheredDurationPercent)
            {
                Color newColor = new Color(217f / 255f, 165f / 255f, 160f / 255f);
                newColor.a = 1;
                NeedleHUD.needleImgZero.color = newColor;
                NeedleHUD.needleImgOne.color = newColor;
                NeedleHUD.needleImgTwo.color = newColor;
                NeedleHUD.needleImgThree.color = newColor;
                NeedleHUD.needleImgFour.color = newColor;
                NeedleHUD.needleImgFive.color = newColor;
                NeedleHUD.needleImgSix.color = newColor;
                NeedleHUD.needleImgSeven.color = newColor;
                NeedleHUD.needleImgEight.color = newColor;
                NeedleHUD.needleImgNine.color = newColor;
            }
            if (bd <= 5f * this.fard.butcheredDurationPercent && bd > 4f * this.fard.butcheredDurationPercent)
            {
                Color newColor = new Color(205f / 255f, 143f / 255f, 138f / 255f);
                newColor.a = 1;
                NeedleHUD.needleImgZero.color = newColor;
                NeedleHUD.needleImgOne.color = newColor;
                NeedleHUD.needleImgTwo.color = newColor;
                NeedleHUD.needleImgThree.color = newColor;
                NeedleHUD.needleImgFour.color = newColor;
                NeedleHUD.needleImgFive.color = newColor;
                NeedleHUD.needleImgSix.color = newColor;
                NeedleHUD.needleImgSeven.color = newColor;
                NeedleHUD.needleImgEight.color = newColor;
                NeedleHUD.needleImgNine.color = newColor;
            }
            if (bd <= 4f * this.fard.butcheredDurationPercent && bd > 3f * this.fard.butcheredDurationPercent)
            {
                Color newColor = new Color(193f / 255f, 121f / 255f, 116f / 255f);
                newColor.a = 1;
                NeedleHUD.needleImgZero.color = newColor;
                NeedleHUD.needleImgOne.color = newColor;
                NeedleHUD.needleImgTwo.color = newColor;
                NeedleHUD.needleImgThree.color = newColor;
                NeedleHUD.needleImgFour.color = newColor;
                NeedleHUD.needleImgFive.color = newColor;
                NeedleHUD.needleImgSix.color = newColor;
                NeedleHUD.needleImgSeven.color = newColor;
                NeedleHUD.needleImgEight.color = newColor;
                NeedleHUD.needleImgNine.color = newColor;
            }
            if (bd <= 3f * this.fard.butcheredDurationPercent && bd > 2f * this.fard.butcheredDurationPercent)
            {
                Color newColor = new Color(181f / 255f, 100f / 255f, 95f / 255f);
                newColor.a = 1;
                NeedleHUD.needleImgZero.color = newColor;
                NeedleHUD.needleImgOne.color = newColor;
                NeedleHUD.needleImgTwo.color = newColor;
                NeedleHUD.needleImgThree.color = newColor;
                NeedleHUD.needleImgFour.color = newColor;
                NeedleHUD.needleImgFive.color = newColor;
                NeedleHUD.needleImgSix.color = newColor;
                NeedleHUD.needleImgSeven.color = newColor;
                NeedleHUD.needleImgEight.color = newColor;
                NeedleHUD.needleImgNine.color = newColor;
            }
            if (bd <= 2f * this.fard.butcheredDurationPercent && bd > 1f * this.fard.butcheredDurationPercent)
            {
                Color newColor = new Color(168f / 255f, 78f / 255f, 74f / 255f);
                newColor.a = 1;
                NeedleHUD.needleImgZero.color = newColor;
                NeedleHUD.needleImgOne.color = newColor;
                NeedleHUD.needleImgTwo.color = newColor;
                NeedleHUD.needleImgThree.color = newColor;
                NeedleHUD.needleImgFour.color = newColor;
                NeedleHUD.needleImgFive.color = newColor;
                NeedleHUD.needleImgSix.color = newColor;
                NeedleHUD.needleImgSeven.color = newColor;
                NeedleHUD.needleImgEight.color = newColor;
                NeedleHUD.needleImgNine.color = newColor;
            }
            if (bd <= 1f * this.fard.butcheredDurationPercent && bd > 0f * this.fard.butcheredDurationPercent)
            {
                Color newColor = new Color(154f / 255f, 55f / 255f, 55f / 255f);
                newColor.a = 1;
                NeedleHUD.needleImgZero.color = newColor;
                NeedleHUD.needleImgOne.color = newColor;
                NeedleHUD.needleImgTwo.color = newColor;
                NeedleHUD.needleImgThree.color = newColor;
                NeedleHUD.needleImgFour.color = newColor;
                NeedleHUD.needleImgFive.color = newColor;
                NeedleHUD.needleImgSix.color = newColor;
                NeedleHUD.needleImgSeven.color = newColor;
                NeedleHUD.needleImgEight.color = newColor;
                NeedleHUD.needleImgNine.color = newColor;
            }
            #endregion
        }
        public void NeedleDisplayCount()
        {
            #region needlehud
            this.fard.baseNeedleAmount = skillLocator.special.maxStock - 1;
            this.fard.needleCount = this.characterBody.GetBuffCount(SeamstressBuffs.needles) - this.fard.baseNeedleAmount;
            switch (this.fard.needleCount)
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
            #endregion
        }
    }
}
