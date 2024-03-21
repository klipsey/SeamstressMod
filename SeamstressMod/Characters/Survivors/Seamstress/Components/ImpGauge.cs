using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;
using TMPro;

namespace SeamstressMod.Survivors.Seamstress
{
    public class ImpGauge : MonoBehaviour
    {
        public HUD targetHUD;
        public SeamstressController target;
        public RectTransform fillRectTransform;

        private Image fillBar;
        private PlayerCharacterMasterController playerCharacterMasterController;

        private void Awake()
        {
            this.fillBar = this.transform.Find("ExpBarRoot").GetChild(0).GetChild(0).GetComponent<Image>();
        }

        private void Update()
        {
            if (!this.target)
            {
                if (!this.playerCharacterMasterController)
                {
                    this.playerCharacterMasterController = (this.targetHUD.targetMaster ? this.targetHUD.targetMaster.GetComponent<PlayerCharacterMasterController>() : null);
                }

                if (this.playerCharacterMasterController && this.playerCharacterMasterController.master.hasBody)
                {
                    SeamstressController fuckYou = this.playerCharacterMasterController.master.GetBody().GetComponent<SeamstressController>();
                    if (fuckYou) this.SetTarget(fuckYou);
                }
            }
            else
            {
                this.UpdateDisplay();
            }
        }

        public void SetTarget(SeamstressController jhhhh)
        {
            this.target = jhhhh;
        }

        private void UpdateDisplay()
        {
            if (this.fillRectTransform)
            {
                float fill = Util.Remap(this.target.ImpGaugeAmountPercent() * 100, 0f, 100f, 0f, 1f);
                this.fillRectTransform.anchorMin = new Vector2(0f, 0f);
                this.fillRectTransform.anchorMax = new Vector2(fill, 1f);
                this.fillRectTransform.sizeDelta = new Vector2(1f, 1f);
            }

            if (this.fillBar)
            {
                if (this.target.drainGauge)
                {
                    this.fillBar.color = new Color(1f, 0f, 46f / 255f);
                }
                else
                {
                    this.fillBar.color = new Color(152f / 255f, 12f / 255f, 37f / 255f);
                }
            }
        }
    }
}