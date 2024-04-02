using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;
using TMPro;
using SeamstressMod.Seamstress.Content;

namespace SeamstressMod.Seamstress.Components
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
            fillBar = transform.Find("ExpBarRoot").GetChild(0).GetChild(0).GetComponent<Image>();
        }

        private void Update()
        {
            if (!target)
            {
                if (!playerCharacterMasterController)
                {
                    playerCharacterMasterController = targetHUD.targetMaster ? targetHUD.targetMaster.GetComponent<PlayerCharacterMasterController>() : null;
                }

                if (playerCharacterMasterController && playerCharacterMasterController.master.hasBody)
                {
                    SeamstressController fuckYou = playerCharacterMasterController.master.GetBody().GetComponent<SeamstressController>();
                    if (fuckYou) SetTarget(fuckYou);
                }
            }
            else
            {
                UpdateDisplay();
            }
        }

        public void SetTarget(SeamstressController controller)
        {
            target = controller;
        }

        private void UpdateDisplay()
        {
            if (fillRectTransform)
            {
                float fill = Util.Remap(target.fiendMeter, 0f, target.healthCoefficient, 0f, 1f);
                fillRectTransform.anchorMin = new Vector2(0f, 0f);
                fillRectTransform.anchorMax = new Vector2(fill, 1f);
                fillRectTransform.sizeDelta = new Vector2(1f, 1f);
            }

            if (fillBar)
            {
                if (target.draining)
                {
                    fillBar.color = new Color(1f, 0f, 46f / 255f);
                }
                else
                {
                    fillBar.color = new Color(152f / 255f, 12f / 255f, 37f / 255f);
                }
            }
        }
    }
}