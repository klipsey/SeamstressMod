using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;


namespace SeamstressMod.Survivors.Seamstress
{
    public class NeedleHudController : MonoBehaviour
    {
        private PlayerCharacterMasterController pcmc;

        public SeamstressController target;
        public HUD hud;
        public Image needleImage;

        public void Awake()
        {
        }
        public void Update()
        {
            if (!target)
            {
                if (!this.pcmc)
                {
                    this.pcmc = (this.hud.targetMaster ? this.hud.targetMaster.GetComponent<PlayerCharacterMasterController>() : null);
                    Log.Debug(pcmc);
                }
                if (this.pcmc && this.pcmc.master.hasBody)
                {
                    SeamstressController seamstressController = this.pcmc.master.GetBody().GetComponent<SeamstressController>();
                    if (seamstressController) this.SetTarget(seamstressController);
                }
            }
            else
            {
                HudColor();
                NeedleDisplayCount();
            }

        }
        public void SetTarget(SeamstressController ok)
        {
            this.target = ok;
        }
        public void HudColor()
        {
            float time = this.target.bd;
            float percent = this.target.butcheredDurationPercent;
            #region hudColorTracking
            if (time > 9f * percent)
            {
                Color newColor = Color.white;

                needleImage.color = newColor;
            }
            if (time <= 9f * percent && time > 8f * percent)
            {
                Color newColor = new Color(245f / 255f, 232 / 255f, 231 / 255f);
                newColor.a = 1;
                needleImage.color = newColor;
            }
            if (time <= 8f * percent && time > 7f * percent)
            {
                Color newColor = new Color(237f / 255f, 209f / 255f, 207f / 255f);
                newColor.a = 1;
                needleImage.color = newColor;
            }
            if (time <= 7f * percent && time > 6f * percent)
            {
                Color newColor = new Color(227f / 255f, 187f / 255f, 183f / 255f);
                newColor.a = 1;
                needleImage.color = newColor;
            }
            if (time <= 6f * percent && time > 5f * percent)
            {
                Color newColor = new Color(217f / 255f, 165f / 255f, 160f / 255f);
                newColor.a = 1;
                needleImage.color = newColor;
            }
            if (time <= 5f * percent && time > 4f * percent)
            {
                Color newColor = new Color(205f / 255f, 143f / 255f, 138f / 255f);
                newColor.a = 1;
                needleImage.color = newColor;
            }
            if (time <= 4f * percent && time > 3f * percent)
            {
                Color newColor = new Color(193f / 255f, 121f / 255f, 116f / 255f);
                newColor.a = 1;
                needleImage.color = newColor;
            }
            if (time <= 3f * percent && time > 2f * percent)
            {
                Color newColor = new Color(181f / 255f, 100f / 255f, 95f / 255f);
                newColor.a = 1;
                needleImage.color = newColor;
            }
            if (time <= 2f * percent && time > 1f * percent)
            {
                Color newColor = new Color(168f / 255f, 78f / 255f, 74f / 255f);
                newColor.a = 1;
                needleImage.color = newColor;
            }
            if (time <= 1f * percent && time > 0f * percent)
            {
                Color newColor = new Color(154f / 255f, 55f / 255f, 55f / 255f);
                newColor.a = 1;
                needleImage.color = newColor;
            }
            if (time <= 0f)
            {
                Color newColor = Color.white;
                needleImage.color = newColor;
            }
            #endregion
        }
        public void NeedleDisplayCount()
        {
            int count = this.target.needleCount;
            Log.Debug(count + " MY NEEDLEEESSSS! ");
            switch (count - 1) 
            {
                case -1:
                    this.gameObject.SetActive(false);
                    break;
                case 0:
                    if (this.gameObject.name == "Needle0") this.gameObject.SetActive(true);
                    break;
                case 1:
                    if (this.gameObject.name == "Needle1") this.gameObject.SetActive(true);
                    break;
                case 2:
                    if (this.gameObject.name == "Needle2") this.gameObject.SetActive(true);
                    break;
                case 3:
                    if (this.gameObject.name == "Needle3") this.gameObject.SetActive(true);
                    break;
                case 4:
                    if (this.gameObject.name == "Needle4") this.gameObject.SetActive(true);
                    break;
                case 5:
                    if (this.gameObject.name == "Needle5") this.gameObject.SetActive(true);
                    break;
                case 6:
                    if (this.gameObject.name == "Needle6") this.gameObject.SetActive(true);
                    break;
                case 7:
                    if (this.gameObject.name == "Needle7") this.gameObject.SetActive(true);
                    break;
                case 8:
                    if (this.gameObject.name == "Needle8") this.gameObject.SetActive(true);
                    break;
                case 9:
                    if (this.gameObject.name == "Needle9") this.gameObject.SetActive(true);
                    break;
                default:
                    Log.Debug("Oh poop");
                    this.gameObject.SetActive(false);
                    break;
            }
        }

    }
}