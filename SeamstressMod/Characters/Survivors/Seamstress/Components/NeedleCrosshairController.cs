using System;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.UI;
using RoR2.UI;
using RoR2;

namespace SeamstressMod.Survivors.Seamstress
{
    public class NeedleCrosshairController : MonoBehaviour
    {
        [Serializable]
        public struct NeedleSpriteDisplay
        {
            public GameObject target;

            public int minimumStockCountToBeValid;

            public int maximumStockCountToBeValid;
        }
        public NeedleSpriteDisplay[] needleSpriteDisplays;
        public RectTransform rectTransform { get; private set; }

        public HudElement hudElement { get; private set; }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            hudElement = GetComponent<HudElement>();
            SetSkillStockDisplays();
        }
        private void SetSkillStockDisplays()
        {
            if (!hudElement.targetCharacterBody)
            {
                return;
            }
            for (int i = 0; i < needleSpriteDisplays.Length; i++)
            {
                bool active = false;
                NeedleSpriteDisplay needleSpriteDisplay = needleSpriteDisplays[i];
                int skill = hudElement.targetCharacterBody.GetBuffCount(SeamstressBuffs.needles);
                if (skill > 0 && skill >= needleSpriteDisplay.minimumStockCountToBeValid && (skill <= needleSpriteDisplay.maximumStockCountToBeValid || needleSpriteDisplay.maximumStockCountToBeValid < 0))
                {
                    active = true;
                }
                needleSpriteDisplay.target.SetActive(active);
            }
        }

        private void LateUpdate()
        {
            SetSkillStockDisplays();
        }
    }

}
