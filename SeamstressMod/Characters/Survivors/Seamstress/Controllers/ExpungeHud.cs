using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2.UI;
using UnityEngine.UI;
using RoR2;

namespace SeamstressMod.Survivors.Seamstress
{
    public class ExpungeHud : MonoBehaviour
    {
        private PlayerCharacterMasterController pcmc;

        public SeamstressController target;
        public HUD hud;
        public Text expungeText;
        public Outline expungeOutline;

        public void Awake()
        {
        }
        public void FixedUpdate()
        {
            if (!this.target)
            {
                if (!this.pcmc)
                {
                    this.pcmc = (this.hud.targetMaster ? this.hud.targetMaster.GetComponent<PlayerCharacterMasterController>() : null);
                }
                if (this.pcmc && this.pcmc.master.hasBody)
                {
                    SeamstressController seamstressController = this.pcmc.master.GetBody().GetComponent<SeamstressController>();
                    if (seamstressController) this.SetTarget(seamstressController);
                }
            }
            else
            {
                ActivateExpungeHud();
                UpdateExpunge();
            }
        }

        public void SetTarget(SeamstressController ok)
        {
            this.target = ok;
        }
        public void ActivateExpungeHud()
        {
            float duration = this.target.bd;
            if(duration > 0f) 
            {
                expungeText.enabled = true;
                expungeOutline.enabled = true;
            }
            else
            {
                expungeText.enabled = false;
                expungeOutline.enabled = false;
            }
        }
        public void UpdateExpunge()
        {
            float num = this.target.bd;
            if(num > 0f) 
            {
                this.expungeText.text = Mathf.Round(num).ToString();
            }
            else
            {
                this.expungeText.text = "0";
            }
        }
    }
}